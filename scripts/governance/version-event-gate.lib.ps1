function Get-XytVersionEventGateResult {
    param([string]$RepoRoot, [string]$LedgerPath, [string]$ChangeType, [string]$EventId,
        [string]$CurrentVersion, [string]$CandidateId, [string]$CandidateFingerprint,
        [bool]$FormalAcceptance, [bool]$RequireClean, [string]$Phase = 'AUTO')
    . (Join-Path $RepoRoot 'tools\governance\version-lib.ps1')
    $rows = @(Import-Csv -Delimiter "`t" -LiteralPath $LedgerPath)
    $events = @($rows | ? { $_.EventId -and $_.Type -in @('FIX','FEATURE','STABILIZATION','GOVERNANCE') })
    $dupes = @($events | Group-Object EventId | ? Count -gt 1)
    $applied = @($events | ? { $_.Type -ne 'GOVERNANCE' -and $_.AppliedVersion } | Group-Object AppliedVersion | ? Count -gt 1)
    $status = 'PASS'
    $reason = 'No formal Version Event requested.'
    $event = $null
    if ($Phase -eq 'AUTO') { $Phase = if ($RequireClean) { 'POST-COMMIT' } else { 'PRE-COMMIT' } }
    function Block([string]$Message) {
        Set-Variable -Name status -Value 'BLOCKED' -Scope 1
        Set-Variable -Name reason -Value $Message -Scope 1
    }
    if ($Phase -notin @('PRE-COMMIT','POST-COMMIT')) { Block 'INVALID EVENT GATE PHASE: BLOCKED' }
    elseif ($RequireClean -and $Phase -ne 'POST-COMMIT') { Block 'INVALID EVENT GATE PHASE: -RequireClean IS POST-COMMIT ONLY' }
    elseif ($Phase -eq 'POST-COMMIT' -and !$RequireClean) { Block 'INVALID EVENT GATE PHASE: POST-COMMIT REQUIRES -RequireClean' }
    elseif (!$ChangeType -or $ChangeType -notin @('FIX','FEATURE','STABILIZATION','GOVERNANCE')) { Block 'MISSING EVENT GATE: BLOCKED' }
    elseif ([string]::IsNullOrWhiteSpace($EventId) -or $EventId -ceq 'EventId') { Block 'MISSING EVENT GATE: BLOCKED' }
    elseif ($dupes.Count -or $applied.Count) { Block 'DUPLICATE GATE: BLOCKED' }
    else {
        $event = @($events | ? { [string]$_.EventId -ceq $EventId })
        if ($event.Count -ne 1) {
            Block 'MISSING EVENT GATE: BLOCKED'
        }
        else {
            $event = $event[0]
        }
    }
    if ($status -eq 'PASS' -and $event -and $ChangeType -eq 'GOVERNANCE') {
        if ($event.Type -ne 'GOVERNANCE') { Block 'CHANGE TYPE: BLOCKED' }
        elseif (!$event.BaselineVersion -or $event.BaselineVersion -ne $CurrentVersion) { Block 'GOVERNANCE BASELINE: BLOCKED' }
        elseif ($event.AppliedVersion -and $event.AppliedVersion -ne $event.BaselineVersion) { Block 'GOVERNANCE VERSION ADVANCE: BLOCKED' }
        elseif ($event.CandidateId -and $event.CandidateId -ne '-') { Block 'GOVERNANCE CANDIDATE BINDING: BLOCKED' }
        elseif ($event.Status -notin @('RESERVED','PROVISIONAL','APPLIED')) { Block 'GOVERNANCE EVENT STATUS: BLOCKED' }
        elseif ([string]::IsNullOrWhiteSpace([string]$event.AcceptanceEvidence)) { Block 'GOVERNANCE EVENT EVIDENCE: BLOCKED' }
        elseif ($Phase -eq 'POST-COMMIT' -and $RequireClean -and (git -C $RepoRoot status --porcelain)) { Block 'POST-COMMIT IDENTITY: NO (DIRTY)' }
        else { $reason='GOVERNANCE EVENT: recorded without advancing product Process Version.' }
    }
    elseif ($status -eq 'PASS' -and $event) {
        if ($event.Type -ne $ChangeType) { Block 'CHANGE TYPE: BLOCKED' }
        elseif (!$FormalAcceptance -or $event.Status -in @('RESERVED','PROVISIONAL','INVESTIGATING','PENDING')) { $reason="VERSION EVENT: $($event.Status)" }
        elseif ($event.Status -ne 'APPLIED') { Block 'MISSING BUMP GATE: BLOCKED' }
        elseif (!$event.AppliedVersion) { Block 'MISSING BUMP GATE: BLOCKED' }
        else {
            $expected=Get-NextProcessVersion $event.BaselineVersion $event.Type
            if($event.AppliedVersion -ne $expected -or $CurrentVersion -ne $event.AppliedVersion){Block 'MISSING BUMP GATE: BLOCKED'}
            elseif(!$CandidateId -or $event.CandidateId -ne $CandidateId -or $CandidateFingerprint -ne "$CandidateId@$CurrentVersion"){Block 'CANDIDATE VERSION BINDING: BLOCKED'}
            elseif([string]::IsNullOrWhiteSpace($event.AcceptanceEvidence)){Block 'ACCEPTANCE EVIDENCE: BLOCKED'}
            elseif($Phase -eq 'POST-COMMIT' -and $RequireClean -and (git -C $RepoRoot status --porcelain)){Block 'POST-COMMIT IDENTITY: NO (DIRTY)'}
            else{$reason='Accepted Event applied with exact next version.'}
        }
    }
    [pscustomobject]@{
        Status = $status
        Reason = $reason
        Event = $event
        Events = $events
        DuplicateEvents = $dupes
        DuplicateVersions = $applied
        FixCounter = @($events | ? { $_.Type -eq 'FIX' -and $_.Status -eq 'APPLIED' }).Count
        FeatureCounter = @($events | ? { $_.Type -eq 'FEATURE' -and $_.Status -eq 'APPLIED' }).Count
    }
}
