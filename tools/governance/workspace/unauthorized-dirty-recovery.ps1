[CmdletBinding()]
param([Parameter(Mandatory)][ValidateSet('claim','sweep')][string]$Command,[Parameter(Mandatory)][string]$RepositoryRoot,[Parameter(Mandatory)][string]$Path,[string]$TaskId,[string]$Owner,[string]$WriteScope,[string]$Reason,[string]$Actor,[string]$BeforeHash,[datetime]$RecoveryEnds=(Get-Date).ToUniversalTime().AddHours(24))
$ErrorActionPreference='Stop';$root=[IO.Path]::GetFullPath($RepositoryRoot);$dir=Join-Path $root '.git\xye-handoff';$file=Join-Path $dir 'unauthorized-dirty-recovery.jsonl';New-Item -ItemType Directory -Force $dir|Out-Null
function Record($x){Add-Content -LiteralPath $file -Value ($x|ConvertTo-Json -Compress);$x|ConvertTo-Json -Compress}
if($Command -eq 'claim'){
 if(!$TaskId -or !$Owner -or !$WriteScope){throw 'CLAIM_BLOCKED TaskId, Owner, and WriteScope are required.'}
 $x=[ordered]@{Event='OWNER_CLAIM';Path=$Path;TaskId=$TaskId;Owner=$Owner;WriteScope=$WriteScope;Actor=$Owner;At=(Get-Date).ToUniversalTime().ToString('o');RecoveryEnds=$RecoveryEnds.ToUniversalTime().ToString('o');CandidateAllowed=$false;EvidenceAllowed=$false;ReleaseAllowed=$false;Reason='Owner claim recorded; recovery window remains non-certifying.'};Record $x;exit 0
}
if(!$Actor -or !$Reason -or !$BeforeHash){throw 'SWEEP_BLOCKED Actor, Reason, and BeforeHash are required.'}
$now=(Get-Date).ToUniversalTime();$x=[ordered]@{Event='UNAUTHORIZED_DIRTY_SWEEP';Path=$Path;Time=$now.ToString('o');Reason=$Reason;Actor=$Actor;BeforeHash=$BeforeHash;AfterResult='REMOVED_OR_ESCALATED';CandidateAllowed=$false;EvidenceAllowed=$false;ReleaseAllowed=$false};Record $x
