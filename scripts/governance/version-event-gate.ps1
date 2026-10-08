[CmdletBinding()]
param(
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$LedgerPath,
    [string]$ChangeType,
    [string]$EventId,
    [string]$CurrentVersion,
    [string]$CandidateId,
    [string]$CandidateFingerprint,
    [ValidateSet('AUTO','PRE-COMMIT','POST-COMMIT')][string]$Phase = 'AUTO',
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
if ($Phase -eq 'AUTO') { $Phase = if ($RequireClean) { 'POST-COMMIT' } else { 'PRE-COMMIT' } }
$result = Get-XytVersionEventGateResult $RepoRoot $LedgerPath $ChangeType $EventId $CurrentVersion $CandidateId $CandidateFingerprint $FormalAcceptance $RequireClean $Phase
$event = 'MISSING / NONE'
if ($result.Event) {
    $event = "$($result.Event.EventId) / $($result.Event.Status)"
}
$missingEvent = if ($result.Reason -like '*MISSING EVENT*') { 'BLOCKED' } else { 'PASS' }
$missingBump = if ($result.Reason -like '*BUMP*') { 'BLOCKED' } else { 'PASS' }
$duplicate = if ($result.DuplicateEvents.Count -or $result.DuplicateVersions.Count) { 'BLOCKED' } else { 'PASS' }
$binding = 'UNBOUND'
if ($result.Event -and $result.Event.Type -ne 'GOVERNANCE' -and $result.Event.CandidateId -and $result.Event.AppliedVersion) {
    $binding = "$($result.Event.CandidateId)@$($result.Event.AppliedVersion)"
}
$eligibility = if ($Phase -eq 'PRE-COMMIT') {
    if ($result.Status -eq 'PASS') { 'DEFERRED (EVENT PREFLIGHT ONLY)' } else { 'NO' }
} elseif ($result.Status -eq 'PASS') { 'POST-COMMIT IDENTITY: CLEAN' } else { 'NO' }
$push = 'DEFERRED (REMOTE TIP AND PUSH GATES REQUIRED)'
Write-Output "VERSION LEDGER: $LedgerPath"
Write-Output "GATE PHASE: $Phase"
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
