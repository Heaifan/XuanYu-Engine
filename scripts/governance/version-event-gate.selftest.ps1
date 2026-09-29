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
Write-Output 'VERSION EVENT GATE SELFTEST PASS'
