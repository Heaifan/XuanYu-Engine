[CmdletBinding()]param([Parameter(Mandatory)][string]$RepositoryRoot,[Parameter(Mandatory)][string]$CandidatePath,[Parameter(Mandatory)][string]$RepositoryFactsPath,[string]$BeforeFingerprintPath,[string]$AfterFingerprintPath)
$ErrorActionPreference='Stop';. (Join-Path $PSScriptRoot 'candidate-scope.ps1')
function Json([string]$p){Get-Content -Raw -LiteralPath $p|ConvertFrom-Json}
$candidate=Json $CandidatePath;$repo=Json $RepositoryFactsPath;$closure=@(Candidate-Closure $candidate)
$active=@(Candidate-Items $repo ActiveTasks|?{$_.Status -eq 'ACTIVE'});$dirty=@(Candidate-Items $repo DirtyRecords);$released=@(Candidate-Items $repo ReleasedRecords)
$scopedWriters=@($active|?{$_.TaskId -ne $candidate.OwnerTaskId -and (Candidate-TaskHits $_ $closure)})
$scopedDirty=@($dirty|?{Candidate-InClosure ([string]$_.Path) $closure});$allTemps=@($dirty+$released)
$consumed=@($scopedDirty|?{$_.Classification -eq 'FRIENDLY_ACTIVE_DEPENDENCY' -or ($_.Classification -eq 'FRIENDLY_ACTIVE_DIRTY' -and $_.OwnerTaskId -ne $candidate.OwnerTaskId)})
$unknown=@($scopedDirty|?{$_.Classification -in @('UNKNOWN_DIRTY','UNKNOWN_TEMP')});$conflicts=@($scopedDirty|?{$_.Classification -in @('OWNERSHIP_CONFLICT','CONFLICT_TEMP')})
$releasedUsed=@($allTemps|?{(Candidate-InClosure ([string]$_.Path) $closure) -and $_.Classification -eq 'FRIENDLY_RELEASED_DIRTY' -and -not $_.Absorbed})
$changed='NO';if($BeforeFingerprintPath -and $AfterFingerprintPath){$changed=if((Json $BeforeFingerprintPath).Digest -ne (Json $AfterFingerprintPath).Digest){'YES'}else{'NO'}}
$blocked=($consumed.Count+$unknown.Count+$conflicts.Count+$releasedUsed.Count+$scopedWriters.Count -gt 0) -or $changed -eq 'YES'
[pscustomobject]@{RepositoryActiveTasks=$active.Count;RepositoryTEMP=$allTemps.Count;RepositoryUnknownTemp=[int]$repo.RepositoryUnknownTemp;CandidateScopedActiveWriters=$scopedWriters.Count;ConsumedActiveForeignTemp=$consumed.Count;CandidateScopedUnknownTemp=$unknown.Count;CandidateScopedOwnershipConflict=$conflicts.Count;CandidateChangedDuringCertification=$changed;FinalEvidenceEligibility=if($blocked){'NO'}else{'YES'};CertificationAllowed=if($blocked){'NO'}else{'YES'};ClosureFiles=$closure.Count;FriendlyReleasedTemp=$releasedUsed.Count}|ConvertTo-Json -Depth 8
