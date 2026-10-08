if(!(Get-Command Assert-OwnerAuthorization -ErrorAction SilentlyContinue)){. (Join-Path $PSScriptRoot '..\coordinator\authority-auth.ps1')}
function Issue-Release([string]$RepoRoot,$Payload,[string]$AuthorizationEvidenceUrl){
    $taskLock=Lock (JsonPath $RepoRoot 'task-lifecycle.lock');$lock=$null
    try{
    $lock=Lock (JsonPath $RepoRoot 'work-release.issue.lock')
    $c=Current $RepoRoot
    $s=State $RepoRoot; if(!$s -or !$s.active){Fail COORDINATOR_BLOCKED 'No active Coordinator wave.'}
    if([string]::IsNullOrWhiteSpace([string]$s.coordinatorScope) -or [string]$s.coordinatorScope -ne 'xye'){Fail COORDINATOR_BLOCKED 'Coordinator scope is invalid.'}
    if([string]$s.mode -match 'blocked|stopped'){Fail COORDINATOR_BLOCKED "Coordinator mode is $($s.mode)."}
    if((UnknownDirty $RepoRoot).Count){Fail UNAUTHORIZED_DIRTY 'Unauthorized dirty prevents Work Release.'}
    if($c.Head -ne [string]$s.baselineHead){Fail BASELINE_HEAD_CHANGED 'Candidate HEAD differs from baseline.'}
    if(!(VersionEvent $RepoRoot $Payload.VersionEventId)){Fail VERSION_EVENT_MISSING 'Version Event is not registered.'}
    $registry=Read-Json (JsonPath $RepoRoot 'task-registry.json');$tasks=@($registry.tasks|?{[string]$_.TaskId -ceq [string]$Payload.TaskId -and [string]$_.Status -ceq 'ACTIVE'});if($tasks.Count -ne 1){Fail TASK_NOT_ACTIVE 'Task Registry must contain exactly one ACTIVE task.'}
    $existing=Read-Json (JsonPath $RepoRoot 'work-release.json');if($existing -and [string]$existing.status -notin @('IDLE','REVOKED')){Fail RELEASE_ALREADY_ACTIVE 'Resolve the existing Work Release before issuing another.'}
    $auth=Assert-OwnerAuthorization $RepoRoot $AuthorizationEvidenceUrl 'work-release-issue' ([string]$Payload.TaskId) ([string]$Payload.VersionEventId) $c.Head;Consume-OwnerAuthorization $RepoRoot $auth
    $token=[guid]::NewGuid().ToString('N'); $r=[pscustomobject]@{schema='XYT-WR/1';status='ACTIVE';token=$token;taskId=$Payload.TaskId;laneId=$Payload.LaneId;ownershipSet=@($Payload.OwnershipSet);candidateBaselineId=[string]$s.waveId;candidateFingerprint=(Fingerprint $RepoRoot @($Payload.OwnershipSet));baselineHead=$c.Head;versionEventId=$Payload.VersionEventId;issuedAt=(Get-Date).ToUniversalTime().ToString('o')}
    Write-Json (JsonPath $RepoRoot 'work-release.json') $r | Out-Null; $r
    }finally{if($lock){$lock.Dispose()};$taskLock.Dispose()}
}
