$ErrorActionPreference='Stop'
function Wave-ScopePatterns([string]$Scope){
    if($Scope -eq 'governance'){return @('AGENTS.md','docs/dev-rules.md','docs/governance/**','scripts/governance/**','tools/governance/**','tools/handoff/**')}
    return @()
}
function Wave-NormalPath([string]$Path){$Path.Replace('\','/').Trim().TrimStart('/')}
function Wave-PatternBase([string]$Pattern){$p=Wave-NormalPath $Pattern;if($p -eq '**'){return ''};if($p.EndsWith('/**')){return $p.Substring(0,$p.Length-2)};return $p}
function Wave-PatternsOverlap([string]$Left,[string]$Right){
    $a=Wave-PatternBase $Left;$b=Wave-PatternBase $Right
    if(!$a -or !$b){return $true};if($a -eq $b){return $true}
    if($Left.EndsWith('/**') -and $b.StartsWith($a,[StringComparison]::OrdinalIgnoreCase)){return $true}
    if($Right.EndsWith('/**') -and $a.StartsWith($b,[StringComparison]::OrdinalIgnoreCase)){return $true}
    return $false
}
function Wave-TaskTouchesScope($Task,[string[]]$Patterns){
    $scopes=@($Task.WriteScope);if(!$scopes.Count){return $true}
    foreach($scope in $scopes){foreach($pattern in $Patterns){if(Wave-PatternsOverlap ([string]$scope) $pattern){return $true}}}
    return $false
}
function Wave-IsStaleAncestor([string]$Root,$State,[string]$Branch){
    if([string]::IsNullOrWhiteSpace([string]$State.branch) -or [string]$State.branch -ceq $Branch -or [string]::IsNullOrWhiteSpace([string]$State.baselineHead)){return $false}
    & git -C $Root cat-file -e "$($State.baselineHead)^{commit}" 2>$null;if($LASTEXITCODE -ne 0){return $false}
    & git -C $Root merge-base --is-ancestor $State.baselineHead HEAD 2>$null
    return $LASTEXITCODE -eq 0
}
function Assert-WaveRecoveryScope([string]$Root,[string]$Scope,$State,$Tasks){
    $patterns=Wave-ScopePatterns $Scope;if(!$patterns.Count){throw "WAVE_INIT_DENIED: WAVE_SCOPE_UNPROVABLE / $Scope"}
    if($null -ne $State.PSObject.Properties['laneStates']){foreach($lane in $State.laneStates.PSObject.Properties){if([string]$lane.Value.state -notin @('FROZEN','CLOSED')){throw "WAVE_INIT_DENIED: ACTIVE_LANES_EXIST / $($lane.Name)"}}}
    if($null -ne $State.PSObject.Properties['activeTasks']){$projected=@($State.activeTasks|%{if($_ -is [string]){[string]$_}else{[string]$_.TaskId}}|sort);$actual=@($Tasks|? Status -eq 'ACTIVE'|%{[string]$_.TaskId}|sort);if(($projected -join "`n") -cne ($actual -join "`n")){throw 'WAVE_INIT_DENIED: TASK_STATE_PROJECTION_MISMATCH / Active Task projection differs.'}}
    foreach($task in @($Tasks|? Status -in @('ACTIVE','REGISTERED'))){if(Wave-TaskTouchesScope $task $patterns){throw "WAVE_INIT_DENIED: LIVE_SCOPE_CONFLICT / $($task.TaskId)"}}
    $freeze=Join-Path $Root '.git/xye-handoff/candidate-freeze.json'
    if(Test-Path -LiteralPath $freeze -PathType Leaf){$candidate=Get-Content -Raw $freeze|ConvertFrom-Json;$owner=@($Tasks|? TaskId -ceq ([string]$candidate.ownerTaskId));if($owner.Count -ne 1 -or !(($owner[0].WriteScope).Count)){throw 'WAVE_INIT_DENIED: CANDIDATE_OWNER_UNKNOWN / Frozen Candidate ownership cannot be proved.'};if(Wave-TaskTouchesScope $owner[0] $patterns){throw "WAVE_INIT_DENIED: CANDIDATE_SCOPE_CONFLICT / $($candidate.candidateId)"}}
}
