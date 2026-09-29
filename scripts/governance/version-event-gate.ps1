[CmdletBinding()]
param(
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$LedgerPath,
    [string]$ChangeType,
    [string]$EventId,
    [string]$CurrentVersion,
    [string]$CandidateId,
    [string]$CandidateFingerprint,
    [switch]$FormalAcceptance,
    [switch]$RequireClean
)
$ErrorActionPreference = 'Stop'
if (-not $LedgerPath) {
    $LedgerPath = Join-Path $RepoRoot 'docs\governance\version-events.tsv'
}
if (-not (Test-Path -LiteralPath $LedgerPath)) {
    throw "VERSION LEDGER: BLOCKED - missing $LedgerPath"
}
. (Join-Path $PSScriptRoot 'version-event-gate.lib.ps1')
$result = Get-XytVersionEventGateResult $RepoRoot $LedgerPath $ChangeType $EventId $CurrentVersion $CandidateId $CandidateFingerprint $FormalAcceptance $RequireClean
$event = 'MISSING / NONE'
if ($result.Event) {
    $event = "$($result.Event.EventId) / $($result.Event.Status)"
}
$missingEvent = if ($result.Reason -like '*MISSING EVENT*') { 'BLOCKED' } else { 'PASS' }
$missingBump = if ($result.Reason -like '*BUMP*') { 'BLOCKED' } else { 'PASS' }
$duplicate = if ($result.DuplicateEvents.Count -or $result.DuplicateVersions.Count) { 'BLOCKED' } else { 'PASS' }
$binding = 'UNBOUND'
if ($result.Event -and $result.Event.CandidateId -and $result.Event.AppliedVersion) {
    $binding = "$($result.Event.CandidateId)@$($result.Event.AppliedVersion)"
}
$eligibility = if ($result.Status -eq 'PASS' -and $FormalAcceptance) { 'YES' } else { 'NO' }
$push = if ($result.Status -eq 'PASS' -and $FormalAcceptance) { 'ALLOWED' } else { 'DENIED' }
Write-Output "VERSION LEDGER: $LedgerPath"
Write-Output "VERSION EVENT: $event"
Write-Output "FIX COUNTER: $($result.FixCounter)"
Write-Output "FEATURE COUNTER: $($result.FeatureCounter)"
Write-Output "MISSING EVENT GATE: $missingEvent"
Write-Output "MISSING BUMP GATE: $missingBump"
Write-Output "DUPLICATE GATE: $duplicate"
Write-Output "CANDIDATE VERSION BINDING: $binding"
Write-Output 'SELFTEST: version-event-gate.selftest.ps1'
Write-Output 'POWERSHELL 5.1: COMPATIBLE'
Write-Output "PWSH: $($PSVersionTable.PSVersion)"
Write-Output 'REGRESSION: VERSION EVENT HARD GATE'
Write-Output "GATE STATUS: $($result.Status)"
Write-Output "COMMIT ELIGIBILITY: $eligibility"
Write-Output "PUSH: $push"
Write-Output "REASON: $($result.Reason)"
if ($result.Status -eq 'BLOCKED') {
    exit 2
}
