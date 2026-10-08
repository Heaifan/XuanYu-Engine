[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$root=Join-Path ([IO.Path]::GetTempPath()) ('close-authority-'+[guid]::NewGuid().ToString('N'))
$script=Join-Path $PSScriptRoot 'close-authority.ps1'
try {
    New-Item -ItemType Directory -Force $root|Out-Null;git -C $root init -b main -q;git -C $root config user.email test@example.invalid;git -C $root config user.name test
    Set-Content (Join-Path $root 'seed') 'seed';git -C $root add seed;git -C $root commit -qm seed
    $dir=Join-Path $root '.git/xye-handoff';New-Item -ItemType Directory -Force $dir|Out-Null
    $state=[pscustomobject]@{waveId='WAVE';active=$true;coordinatorScope='governance';activeTasks=@('UNRELATED-ACTIVE');laneStates=[pscustomobject]@{}}
    $tasks=[pscustomobject]@{tasks=@([pscustomobject]@{TaskId='UNRELATED-ACTIVE';Lane='xye';Owner='C';Status='ACTIVE';WriteScope=@('src/**');ExpectedDependencies=@('RenderProjection OPEN')})}
    $freeze='{"candidateId":"OLD","status":"FROZEN_FOR_CERTIFICATION","productAcceptanceState":"P4_PENDING"}'
    $statePath=Join-Path $dir 'state.json';$taskPath=Join-Path $dir 'task-registry.json';$freezePath=Join-Path $dir 'candidate-freeze.json'
    $state|ConvertTo-Json -Depth 8|Set-Content $statePath;$tasks|ConvertTo-Json -Depth 8|Set-Content $taskPath;Set-Content $freezePath $freeze
    $beforeState=Get-Content $statePath -Raw;$beforeTasks=Get-Content $taskPath -Raw;$beforeFreeze=Get-Content $freezePath -Raw
    $output=@(& pwsh -NoLogo -NoProfile -File $script -Mode global-close -Scope governance -RepositoryRoot $root 2>&1);$code=$LASTEXITCODE;$text=$output -join "`n"
    if($code -eq 0 -or $text -notmatch 'ACTIVE_TASKS_EXIST'){throw "Global Close failed to preserve its strict ACTIVE Task guard: $code / $text"}
    if((Get-Content $statePath -Raw) -cne $beforeState -or (Get-Content $taskPath -Raw) -cne $beforeTasks -or (Get-Content $freezePath -Raw) -cne $beforeFreeze){throw 'blocked Global Close changed Wave, Task, or P4 Candidate state'}
    'CLOSE AUTHORITY GLOBAL SAFETY SELFTEST PASS'
} finally { $full=[IO.Path]::GetFullPath($root);$temp=[IO.Path]::GetFullPath([IO.Path]::GetTempPath());if($full.StartsWith($temp,[StringComparison]::OrdinalIgnoreCase)-and(Test-Path -LiteralPath $full)){Remove-Item -LiteralPath $full -Recurse -Force} }
