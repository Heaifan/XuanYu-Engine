$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
. (Join-Path $PSScriptRoot 'version-event-gate.lib.ps1')
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-version-event-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $temp -Force | Out-Null

function New-Row {
    param([string]$Id = 'XYT-A', [string]$Status = 'RESERVED', [string]$Applied = '', [string]$Candidate = 'CAND-A')
    [pscustomobject]@{
        EventId = $Id
        TaskId = 'TASK-A'
        Type = 'FIX'
        Name = 'DEM'
        BaselineVersion = 'v0.3.0.0-r1'
        AppliedVersion = $Applied
        Status = $Status
        AcceptanceEvidence = if ($Status -eq 'APPLIED') { 'acceptance-1' } else { '' }
        AcceptedTime = '2026-09-29T00:00:00Z'
        CandidateId = $Candidate
        CommitId = 'abc1234'
    }
}

function New-GovernanceRow([string]$Id = 'GOV-A', [string]$Baseline = 'v0.3.0.9-fix', [string]$Applied = $Baseline, [string]$Status = 'RESERVED', [string]$Commit = '-') {
    [pscustomobject]@{ EventId=$Id; TaskId='GOV-TASK'; Type='GOVERNANCE'; Name='Governance recovery'; BaselineVersion=$Baseline; AppliedVersion=$Applied; Status=$Status; AcceptanceEvidence='source-only governance maintenance'; AcceptedTime=$(if($Status -eq 'APPLIED'){'2026-10-09T00:00:00Z'}else{''}); CandidateId='-'; CommitId=$Commit }
}

function Invoke-Case {
    param([string]$Name, [object[]]$Rows, [hashtable]$Arguments, [string]$Expected)
    $path = Join-Path $temp "$Name.tsv"
    $Rows | Export-Csv $path -Delimiter "`t" -NoTypeInformation -Encoding UTF8
    $result = Get-XytVersionEventGateResult $root $path $Arguments.ChangeType $Arguments.EventId $Arguments.CurrentVersion $Arguments.CandidateId $Arguments.CandidateFingerprint $Arguments.FormalAcceptance $false
    $text = "$($result.Status) $($result.Reason)"
    if ($text -notlike "*$Expected*") {
        throw "$Name expected $Expected got $text"
    }
    return $result
}

function New-Arguments {
    param([string]$ChangeType, [string]$EventId, [string]$CurrentVersion, [string]$CandidateId, [string]$CandidateFingerprint, [bool]$FormalAcceptance)
    return @{
        ChangeType = $ChangeType
        EventId = $EventId
        CurrentVersion = $CurrentVersion
        CandidateId = $CandidateId
        CandidateFingerprint = $CandidateFingerprint
        FormalAcceptance = $FormalAcceptance
    }
}

[void](Invoke-Case 'missing' @() (New-Arguments 'FIX' '' 'v0.3.0.0-r1' 'CAND-A' '' $true) 'MISSING EVENT GATE')
[void](Invoke-Case 'feature-missing' @() (New-Arguments 'FEATURE' '' 'v0.3.0.0-r1' 'CAND-F' '' $true) 'MISSING EVENT GATE')
[void](Invoke-Case 'governance-missing' @() (New-Arguments 'GOVERNANCE' '' 'v0.3.0.9-fix' '' '' $false) 'MISSING EVENT GATE')
[void](Invoke-Case 'governance-case-mismatch' @(New-GovernanceRow) (New-Arguments 'GOVERNANCE' 'gov-a' 'v0.3.0.9-fix' '' '' $false) 'MISSING EVENT GATE')
[void](Invoke-Case 'pending' @(New-Row) (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.0-r1' 'CAND-A' '' $false) 'VERSION EVENT: RESERVED')
[void](Invoke-Case 'no-bump' @(New-Row 'XYT-A' 'ACCEPTED') (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.0-r1' 'CAND-A' '' $true) 'MISSING BUMP GATE')
$one = New-Row 'XYT-A' 'APPLIED' 'v0.3.0.1-fix'
$accepted = Invoke-Case 'accepted' @($one) (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.1-fix' 'CAND-A' 'CAND-A@v0.3.0.1-fix' $true) 'PASS'
if ($accepted.FixCounter -ne 1) { throw "Accepted Fix counter expected 1 got $($accepted.FixCounter)" }
$two = New-Row 'XYT-C' 'APPLIED' 'v0.3.0.2-fix' 'CAND-C'
$two.BaselineVersion = 'v0.3.0.1-fix'
$consecutive = Invoke-Case 'consecutive' @($one, $two) (New-Arguments 'FIX' 'XYT-C' 'v0.3.0.2-fix' 'CAND-C' 'CAND-C@v0.3.0.2-fix' $true) 'PASS'
if ($consecutive.FixCounter -ne 2) { throw "Second Fix counter expected 2 got $($consecutive.FixCounter)" }
$wrong = New-Row 'XYT-A' 'APPLIED' 'v0.3.0.4-fix'
[void](Invoke-Case 'wrong' @($wrong) (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.4-fix' 'CAND-A' 'CAND-A@v0.3.0.4-fix' $true) 'MISSING BUMP GATE')
[void](Invoke-Case 'fingerprint' @($one) (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.1-fix' 'CAND-A' 'CAND-A@v0.3.0.2-fix' $true) 'CANDIDATE VERSION BINDING')
[void](Invoke-Case 'duplicate' @($one, $one) (New-Arguments 'FIX' 'XYT-A' 'v0.3.0.1-fix' 'CAND-A' 'CAND-A@v0.3.0.1-fix' $true) 'DUPLICATE GATE')
$gov = Invoke-Case 'governance-reserved' @(New-GovernanceRow) (New-Arguments 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false) 'GOVERNANCE EVENT'
if ($gov.FixCounter -ne 0 -or $gov.FeatureCounter -ne 0) { throw 'Governance event incorrectly advanced a product counter' }
[void](Invoke-Case 'governance-stale-baseline' @(New-GovernanceRow 'GOV-A' 'v0.3.0.8-fix') (New-Arguments 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false) 'GOVERNANCE BASELINE')
$noEvidence = New-GovernanceRow; $noEvidence.AcceptanceEvidence = ''
[void](Invoke-Case 'governance-no-evidence' @($noEvidence) (New-Arguments 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false) 'GOVERNANCE EVENT EVIDENCE')
[void](Invoke-Case 'governance-product-bump' @(New-GovernanceRow 'GOV-A' 'v0.3.0.9-fix' 'v0.3.0.10-fix') (New-Arguments 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false) 'GOVERNANCE VERSION ADVANCE')
[void](Invoke-Case 'governance-wrong-type' @(New-GovernanceRow) (New-Arguments 'FIX' 'GOV-A' 'v0.3.0.9-fix' '' '' $false) 'CHANGE TYPE')
$phaseRepo = Join-Path $temp 'phase-repo'
New-Item -ItemType Directory $phaseRepo | Out-Null
New-Item -ItemType Directory (Join-Path $phaseRepo 'tools\governance') -Force | Out-Null
Copy-Item (Join-Path $root 'tools\governance\version-lib.ps1') (Join-Path $phaseRepo 'tools\governance\version-lib.ps1')
git -C $phaseRepo init -q
git -C $phaseRepo config user.email governance-test@example.invalid
git -C $phaseRepo config user.name GovernanceTest
git -C $phaseRepo config core.autocrlf false
git -C $phaseRepo add tools/governance/version-lib.ps1
git -C $phaseRepo commit -qm 'test fixture'
'clean' | Set-Content (Join-Path $phaseRepo 'state.txt')
git -C $phaseRepo add state.txt
git -C $phaseRepo commit -qm baseline
'candidate' | Set-Content (Join-Path $phaseRepo 'state.txt')
$phaseLedger = Join-Path $temp 'governance-phase.tsv'
@(New-GovernanceRow) | Export-Csv $phaseLedger -Delimiter "`t" -NoTypeInformation -Encoding UTF8
$pre = Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $false 'PRE-COMMIT'
if ($pre.Status -ne 'PASS') { throw "PRE-COMMIT rejected a dirty prepared Candidate: $($pre.Reason)" }
$preCleanFlag = Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true 'PRE-COMMIT'
if ($preCleanFlag.Status -ne 'BLOCKED' -or $preCleanFlag.Reason -notlike '*POST-COMMIT ONLY*') { throw 'PRE-COMMIT accepted the POST-COMMIT RequireClean flag' }
$preparedCommit=(git -C $phaseRepo rev-parse HEAD).Trim()
@(New-GovernanceRow 'GOV-A' 'v0.3.0.9-fix' 'v0.3.0.9-fix' 'APPLIED' $preparedCommit) | Export-Csv $phaseLedger -Delimiter "`t" -NoTypeInformation -Encoding UTF8
$post = Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true 'POST-COMMIT'
if ($post.Status -ne 'BLOCKED' -or $post.Reason -notlike '*POST-COMMIT IDENTITY*') { throw 'POST-COMMIT accepted a dirty worktree' }
$autoPost = Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true
if ($autoPost.Status -ne 'BLOCKED' -or $autoPost.Reason -notlike '*POST-COMMIT IDENTITY*') { throw 'AUTO did not map -RequireClean to POST-COMMIT' }
git -C $phaseRepo add state.txt; git -C $phaseRepo commit -qm 'prepared governance result'; $sourceCommit=(git -C $phaseRepo rev-parse HEAD).Trim()
@(New-GovernanceRow 'GOV-A' 'v0.3.0.9-fix' 'v0.3.0.9-fix' 'APPLIED' $sourceCommit) | Export-Csv $phaseLedger -Delimiter "`t" -NoTypeInformation -Encoding UTF8
$postApplied = Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true 'POST-COMMIT'
if ($postApplied.Status -ne 'PASS') { throw "POST-COMMIT rejected an applied event tied to an ancestor commit: $($postApplied.Reason)" }
$missingCommit=New-GovernanceRow 'GOV-A' 'v0.3.0.9-fix' 'v0.3.0.9-fix' 'APPLIED'
@( $missingCommit ) | Export-Csv $phaseLedger -Delimiter "`t" -NoTypeInformation -Encoding UTF8
$postMissingCommit=Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true 'POST-COMMIT'
if ($postMissingCommit.Status -ne 'BLOCKED' -or $postMissingCommit.Reason -notlike '*GOVERNANCE COMMIT IDENTITY*') { throw 'POST-COMMIT accepted APPLIED without a source CommitId' }
$notAncestor=New-GovernanceRow 'GOV-A' 'v0.3.0.9-fix' 'v0.3.0.9-fix' 'APPLIED' ('f'*40)
@( $notAncestor ) | Export-Csv $phaseLedger -Delimiter "`t" -NoTypeInformation -Encoding UTF8
$postWrongCommit=Get-XytVersionEventGateResult $phaseRepo $phaseLedger 'GOVERNANCE' 'GOV-A' 'v0.3.0.9-fix' '' '' $false $true 'POST-COMMIT'
if ($postWrongCommit.Status -ne 'BLOCKED' -or $postWrongCommit.Reason -notlike '*GOVERNANCE COMMIT IDENTITY*') { throw 'POST-COMMIT accepted a nonexistent source CommitId' }
Write-Output 'VERSION EVENT GATE SELFTEST PASS'
