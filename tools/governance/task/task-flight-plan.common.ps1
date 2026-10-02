$ErrorActionPreference='Stop'
function Fail([string]$Code,[string]$Message){throw "$Code $Message"}
function Repo([string]$Path){if($Path){return (Resolve-Path -LiteralPath $Path).Path};return (git rev-parse --show-toplevel).Trim()}
function FlightDir([string]$R){Join-Path $R '.git\xye-handoff'}
function RegPath([string]$R){Join-Path (FlightDir $R) 'task-registry.json'}
function HistPath([string]$R){Join-Path (FlightDir $R) 'task-history.jsonl'}
function Now(){(Get-Date).ToUniversalTime().ToString('o')}
function Json([string]$P){if(!(Test-Path -LiteralPath $P)){return $null};Get-Content -Raw -LiteralPath $P|ConvertFrom-Json}
function ReadReg([string]$R){$x=Json (RegPath $R);if($null -eq $x){return [pscustomobject]@{tasks=@()}};if($null -eq $x.tasks){$x|Add-Member NoteProperty tasks @()};return $x}
function AtomicJson([string]$P,$Value){$d=Split-Path $P;New-Item -ItemType Directory -Force $d|Out-Null;$t="$P.$([guid]::NewGuid().ToString('N')).tmp";$enc=New-Object Text.UTF8Encoding -ArgumentList $false;[IO.File]::WriteAllText($t,($Value|ConvertTo-Json -Depth 20),$enc);Move-Item -LiteralPath $t -Destination $P -Force}
function SaveReg([string]$R,$Reg){$p=RegPath -R $R;AtomicJson -P $p -Value $Reg}
function Audit([string]$R,[string]$Event,$Task,[string]$Reason='', [string]$Coordinator=''){$e=[ordered]@{At=(Now);Event=$Event;TaskId=$Task.TaskId;Status=$Task.Status;Reason=$Reason;CloseReason=$Reason;Coordinator=$Coordinator};Add-Content -LiteralPath (HistPath $R) -Value ($e|ConvertTo-Json -Compress)}
function NormWorkspace([string]$P){([IO.Path]::GetFullPath($P)).Replace('\','/').TrimEnd('/').ToLowerInvariant()}
function NormScope([string]$P,[string]$W){$p=$P.Trim().Replace('\','/');if([IO.Path]::IsPathRooted($p)){$full=NormWorkspace $p;$base=NormWorkspace $W;if($full -eq $base){return '/'};if($full.StartsWith($base+'/')){return $full.Substring($base.Length+1)}};return ('/'+$p.TrimStart('/').TrimEnd('/')).ToLowerInvariant()}
function Scopes([string[]]$Paths,[string]$W){@($Paths|?{![string]::IsNullOrWhiteSpace($_)}|%{NormScope $_ $W}|sort -Unique)}
function Overlap([string]$A,[string]$B){$x=$A.TrimEnd('/');$y=$B.TrimEnd('/');return $x -eq $y -or $x.StartsWith($y+'/') -or $y.StartsWith($x+'/')}
function Conflict($Reg,[string[]]$Wanted,[string]$TaskId,[string]$WantedWorkspace){foreach($t in @($Reg.tasks|?{$_.Status -eq 'ACTIVE' -and $_.TaskId -ne $TaskId})){if((NormWorkspace ([string]$t.Workspace)) -ne (NormWorkspace $WantedWorkspace)){continue};foreach($a in $Wanted){foreach($b in @($t.WriteScope)){if(Overlap $a $b){return $t}}}};return $null}
function Head([string]$R){$h=git -C $R rev-parse HEAD 2>$null;if($LASTEXITCODE){Fail GIT_FAILED 'cannot read HEAD'};return ($h.Trim())}
