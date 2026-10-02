[CmdletBinding()]
param()

function Resolve-HandclapLedgerPath([string]$LedgerPath) {
    if ([string]::IsNullOrWhiteSpace($LedgerPath)) {
        return (Join-Path (Get-Location) '.git\xye-handclap\events.jsonl')
    }
    return $LedgerPath
}

function Add-HandclapEvent {
    param([string]$LedgerPath, [Parameter(Mandatory)][object]$Event)
    $path = Resolve-HandclapLedgerPath $LedgerPath
    foreach ($name in 'schemaVersion', 'eventId', 'timestampUtc', 'kind', 'actor', 'scope') {
        $property = $Event.PSObject.Properties[$name]
        if ($null -eq $property -or [string]::IsNullOrWhiteSpace([string]$property.Value)) {
        throw "Handclap event requires $name."
        }
    }
    $parsed = [datetimeoffset]::MinValue
    if (-not [datetimeoffset]::TryParse([string]$Event.timestampUtc, [ref]$parsed) -or $parsed.Offset -ne [TimeSpan]::Zero) {
        throw 'Handclap event timestampUtc must be a UTC ISO-8601 timestamp.'
    }
    if (Get-Command Assert-HandclapEvent -CommandType Function -ErrorAction SilentlyContinue) {
        [void](Assert-HandclapEvent $Event)
    }
    $dir = Split-Path -Parent $path
    if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $line = $Event | ConvertTo-Json -Depth 20 -Compress
    [IO.File]::AppendAllText($path, $line + [Environment]::NewLine, (New-Object Text.UTF8Encoding($false)))
    return $Event
}

function Read-HandclapLedger {
    param([string]$LedgerPath)
    $path = Resolve-HandclapLedgerPath $LedgerPath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return @() }
    $lineNumber = 0
    foreach ($line in Get-Content -LiteralPath $path) {
        $lineNumber++
        if ([string]::IsNullOrWhiteSpace($line)) { throw "Handclap ledger line $lineNumber is empty." }
        try { $event = $line | ConvertFrom-Json -ErrorAction Stop }
        catch { throw "Handclap ledger line $lineNumber is invalid JSON: $($_.Exception.Message)" }
        if ($null -eq $event) { throw "Handclap ledger line $lineNumber is null JSON." }
        $timestampMatch = [regex]::Match($line, '"timestampUtc"\s*:\s*"([^"]*)"')
        if ($timestampMatch.Success) { $event.timestampUtc = $timestampMatch.Groups[1].Value }
        $event
    }
}
