[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('Register','Invalidate','Plan','Closure')][string]$Operation,
    [Parameter(Mandatory)][string]$EvidencePath,
    [string]$ChangePath, [string]$Commit, [string]$Version, [string]$Capability,
    [string]$TestId, [string]$TestSetVersion, [string]$EvidenceLevel,
    [string]$CreatedAt, [string[]]$CriticalCapability, [ValidateSet('Development','Closure')][string]$Phase = 'Development',
    [ValidateSet('VALID','REVALIDATION_REQUIRED','EXPIRED','SUPERSEDED')][string]$Status = 'VALID'
)
$ErrorActionPreference = 'Stop'
$triggers = @('Capability Contract Changed','Render Pipeline Changed','Swapchain Lifecycle Changed','Input Route Changed','Terrain Pipeline Changed','Runtime Host Changed','Test Oracle Changed','Test Set Major Version Changed')

function Read-Records { if (-not (Test-Path -LiteralPath $EvidencePath)) { return @() }; @(Get-Content -LiteralPath $EvidencePath -Raw | ConvertFrom-Json) }
function Write-Records($records) { $records | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $EvidencePath -Encoding UTF8 }
function Require-Text([string]$name, [string]$value) { if ([string]::IsNullOrWhiteSpace($value)) { throw "$name is required." } }
function To-Utc([string]$value) { if ([string]::IsNullOrWhiteSpace($value)) { return [DateTime]::UtcNow.ToString('o') }; return ([DateTimeOffset]::Parse($value)).UtcDateTime.ToString('o') }
function Test-Change($record, $change) {
    if ($change.Trigger -notin $triggers) { return $false }
    if ($change.Capability -and $change.Capability -ne $record.Capability) { return $false }
    if ($change.Trigger -eq 'Test Set Major Version Changed') { return $change.TestSetVersion -and $change.TestSetVersion -ne $record.TestSetVersion }
    if ($change.Trigger -eq 'Test Oracle Changed') { return $true }
    return [bool]$change.Capability
}
function Register-Record {
    Require-Text 'Commit' $Commit; Require-Text 'Version' $Version; Require-Text 'Capability' $Capability
    Require-Text 'TestId' $TestId; Require-Text 'TestSetVersion' $TestSetVersion; Require-Text 'EvidenceLevel' $EvidenceLevel
    $records = @(Read-Records)
    $record = [ordered]@{ Status = $Status; Commit = $Commit; Version = $Version; Capability = $Capability; TestId = $TestId; TestSetVersion = $TestSetVersion; EvidenceLevel = $EvidenceLevel; CreatedAt = To-Utc $CreatedAt; InvalidationTrigger = $null }
    $records += [pscustomobject]$record; Write-Records $records
    [pscustomobject]@{ Status = 'REGISTERED'; Record = $record } | ConvertTo-Json -Depth 8
}
function Invalidate-Records {
    Require-Text 'ChangePath' $ChangePath
    $records = @(Read-Records); $changes = @(Get-Content -LiteralPath $ChangePath -Raw | ConvertFrom-Json); $changed = @()
    foreach ($record in $records) {
        foreach ($change in $changes) {
            if ($record.Status -eq 'VALID' -and (Test-Change $record $change)) { $record.Status = 'REVALIDATION_REQUIRED'; $record.InvalidationTrigger = $change.Trigger; $changed += $record; break }
        }
    }
    Write-Records $records
    [pscustomobject]@{ Status = 'REVALIDATION_REQUIRED'; Invalidated = @($changed); Records = $records } | ConvertTo-Json -Depth 8
}
function New-Plan {
    $records = @(Read-Records); $items = @($records | Where-Object { $_.Status -in @('REVALIDATION_REQUIRED','EXPIRED') } | ForEach-Object {
        [pscustomobject]@{ Capability = $_.Capability; TestId = $_.TestId; TestSetVersion = $_.TestSetVersion; EvidenceLevel = $_.EvidenceLevel; Reason = $_.InvalidationTrigger; Status = $_.Status }
    })
    [pscustomobject]@{ Kind = 'Revalidation Plan'; Strategy = 'MINIMUM_SUFFICIENT_TEST_SET'; Items = $items } | ConvertTo-Json -Depth 8
}
function Test-Closure {
    $records = @(Read-Records); $missing = @()
    foreach ($capabilityName in @($CriticalCapability)) {
        $valid = @($records | Where-Object { $_.Capability -eq $capabilityName -and $_.Status -eq 'VALID' })
        if ($valid.Count -eq 0) { $missing += $capabilityName }
    }
    $decision = if ($Phase -eq 'Development') { 'ALLOW_DEVELOPMENT' } elseif ($missing.Count -eq 0) { 'PASS' } else { 'BLOCKED' }
    [pscustomobject]@{ Kind = 'Evidence Closure'; Phase = $Phase; Decision = $decision; MissingCriticalEvidence = $missing } | ConvertTo-Json -Depth 8
}

switch ($Operation) {
    'Register' { Register-Record }
    'Invalidate' { Invalidate-Records }
    'Plan' { New-Plan }
    'Closure' { Test-Closure }
}
