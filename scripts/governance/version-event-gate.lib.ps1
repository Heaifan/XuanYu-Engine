function Get-XytVersionEventGateResult {
    param([string]$RepoRoot, [string]$LedgerPath, [string]$ChangeType, [string]$EventId,
        [string]$CurrentVersion, [string]$CandidateId, [string]$CandidateFingerprint,
        [bool]$FormalAcceptance, [bool]$RequireClean)
    . (Join-Path $RepoRoot 'tools\governance\version-lib.ps1')
    $rows = @(Import-Csv -Delimiter "`t" -LiteralPath $LedgerPath)
    $events = @($rows | ? { $_.EventId -and $_.Type -in @('FIX','FEATURE') })
    $dupes = @($events | Group-Object EventId | ? Count -gt 1)
    $applied = @($events | ? AppliedVersion | Group-Object AppliedVersion | ? Count -gt 1)
    $status = 'PASS'
    $reason = 'No formal Version Event requested.'
    $event = $null
    function Block([string]$Message) {
        Set-Variable -Name status -Value 'BLOCKED' -Scope 1
        Set-Variable -Name reason -Value $Message -Scope 1
    }
    if (!$ChangeType -or $ChangeType -notin @('FIX','FEATURE')) { Block 'MISSING EVENT GATE: BLOCKED' }
    elseif (!$EventId) { Block 'MISSING EVENT GATE: BLOCKED' }
    elseif ($dupes.Count -or $applied.Count) { Block 'DUPLICATE GATE: BLOCKED' }
    else {
        $event = @($events | ? EventId -eq $EventId)
        if ($event.Count -ne 1) {
            Block 'MISSING EVENT GATE: BLOCKED'
        }
        else {
            $event = $event[0]
        }
    }
    if ($status -eq 'PASS' -and $event) {
        if ($event.Type -ne $ChangeType) { Block 'CHANGE TYPE: BLOCKED' }
        elseif (!$FormalAcceptance -or $event.Status -in @('RESERVED','PROVISIONAL','INVESTIGATING','PENDING')) { $reason="VERSION EVENT: $($event.Status)" }
        elseif ($event.Status -ne 'APPLIED') { Block 'MISSING BUMP GATE: BLOCKED' }
        elseif (!$event.AppliedVersion) { Block 'MISSING BUMP GATE: BLOCKED' }
        else {
            $expected=Get-NextProcessVersion $event.BaselineVersion $event.Type
            if($event.AppliedVersion -ne $expected -or $CurrentVersion -ne $event.AppliedVersion){Block 'MISSING BUMP GATE: BLOCKED'}
            elseif(!$CandidateId -or $event.CandidateId -ne $CandidateId -or $CandidateFingerprint -ne "$CandidateId@$CurrentVersion"){Block 'CANDIDATE VERSION BINDING: BLOCKED'}
            elseif([string]::IsNullOrWhiteSpace($event.AcceptanceEvidence)){Block 'ACCEPTANCE EVIDENCE: BLOCKED'}
            elseif($RequireClean -and (git -C $RepoRoot status --porcelain)){Block 'COMMIT ELIGIBILITY: NO (DIRTY)'}
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
