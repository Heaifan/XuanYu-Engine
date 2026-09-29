function Issue-Release([string]$RepoRoot,$Payload){
    $s=State $RepoRoot; if(!$s -or !$s.active){Fail COORDINATOR_BLOCKED 'No active Coordinator wave.'}
    if([string]::IsNullOrWhiteSpace([string]$s.coordinatorScope) -or [string]$s.coordinatorScope -ne 'xye'){Fail COORDINATOR_BLOCKED 'Coordinator scope is invalid.'}
    if([string]$s.mode -match 'blocked|stopped'){Fail COORDINATOR_BLOCKED "Coordinator mode is $($s.mode)."}
    if((UnknownDirty $RepoRoot).Count){Fail UNKNOWN_DIRTY 'UnknownDirty prevents Work Release.'}
    $c=Current $RepoRoot; if($c.Head -ne [string]$s.baselineHead){Fail BASELINE_HEAD_CHANGED 'Candidate HEAD differs from baseline.'}
    if(!(VersionEvent $RepoRoot $Payload.VersionEventId)){Fail VERSION_EVENT_MISSING 'Version Event is not registered.'}
    $token=[guid]::NewGuid().ToString('N'); $r=[pscustomobject]@{schema='XYT-WR/1';status='ACTIVE';token=$token;taskId=$Payload.TaskId;laneId=$Payload.LaneId;ownershipSet=@($Payload.OwnershipSet);candidateBaselineId=[string]$s.waveId;candidateFingerprint=(Fingerprint $RepoRoot @($Payload.OwnershipSet));baselineHead=$c.Head;versionEventId=$Payload.VersionEventId;issuedAt=(Get-Date).ToUniversalTime().ToString('o')}
    Write-Json (JsonPath $RepoRoot 'work-release.json') $r | Out-Null; $r
}
