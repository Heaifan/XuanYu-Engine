[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('Claim','Check','Release')][string]$Action,
    [Parameter(Mandatory)][string]$Owner,
    [Parameter(Mandatory)][string[]]$Paths,
    [hashtable]$ExpectedHashes = @{}
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$gitDir = (& git -C $root rev-parse --absolute-git-dir).Trim()
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $gitDir -PathType Container)) {
    throw 'Cannot locate the canonical repository Git directory.'
}
$claims = Join-Path $gitDir 'xye-write-claims'
if ($Action -eq 'Claim' -and -not (Test-Path -LiteralPath $claims)) {
    New-Item -ItemType Directory -Path $claims | Out-Null
}

foreach ($relative in $Paths) {
    $full = [IO.Path]::GetFullPath((Join-Path $root $relative))
    if (-not $full.StartsWith($root.TrimEnd('\') + '\',[StringComparison]::OrdinalIgnoreCase)) {
        throw "Write path escapes canonical repository: $relative"
    }
    $actual = if (Test-Path -LiteralPath $full -PathType Leaf) {
        (Get-FileHash -LiteralPath $full -Algorithm SHA256).Hash
    } else { 'MISSING' }
    if ($ExpectedHashes.ContainsKey($relative) -and $ExpectedHashes[$relative] -ne $actual) {
        throw "Stale baseline for '$relative': expected $($ExpectedHashes[$relative]), found $actual. Refusing write."
    }
    $key = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes($relative.ToLowerInvariant()))).Replace('-','')
    $claim = Join-Path $claims ($key + '.json')
    if ($Action -eq 'Claim') {
        $record = [pscustomobject]@{ Owner=$Owner; Path=$relative; PID=$PID; ClaimedAt=[DateTime]::UtcNow.ToString('o') } |
            ConvertTo-Json -Compress
        try {
            $stream = [IO.File]::Open($claim,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None)
            try {
                $bytes = [Text.UTF8Encoding]::new($false).GetBytes($record)
                $stream.Write($bytes,0,$bytes.Length)
            } finally { $stream.Dispose() }
        }
        catch [IO.IOException] {
            $existing = Get-Content -LiteralPath $claim -Raw | ConvertFrom-Json
            if ($existing.Owner -ne $Owner) { throw "Write conflict for '$relative': owned by '$($existing.Owner)' (claim PID $($existing.PID))." }
        }
    } elseif ($Action -eq 'Check' -and (Test-Path -LiteralPath $claim)) {
        $existing = Get-Content -LiteralPath $claim -Raw | ConvertFrom-Json
        if ($existing.Owner -ne $Owner) { throw "Write conflict for '$relative': owned by '$($existing.Owner)' (claim PID $($existing.PID))." }
    } elseif ($Action -eq 'Release' -and (Test-Path -LiteralPath $claim)) {
        $existing = Get-Content -LiteralPath $claim -Raw | ConvertFrom-Json
        if ($existing.Owner -ne $Owner) { throw "Cannot release '$relative': owned by '$($existing.Owner)'." }
        Remove-Item -LiteralPath $claim -Force
    }
}
Write-Output "$Action accepted for $($Paths.Count) path(s); owner=$Owner"
