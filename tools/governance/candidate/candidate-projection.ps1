$ErrorActionPreference = 'Stop'

function Get-CandidateGateProjection([string]$Root, $Facts, $Tasks, $Classes, $State) {
    $path = if ($State -and $State.PSObject.Properties['candidatePath']) {
        [string]$State.candidatePath
    } else { Join-Path $Root '.git\xye-handoff\candidate.json' }
    $active = @($Tasks | Where-Object Status -eq 'ACTIVE')
    $repoTemp = @($Facts.Dirty)
    $repoUnknown = @($Classes | Where-Object Classification -eq 'UNAUTHORIZED_DIRTY')
    $global=Candidate-GlobalStatus $Facts;$wave=if($Facts.WaveStatus){[string]$Facts.WaveStatus}else{'UNKNOWN'}
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) {
        return [pscustomobject]@{ RepositoryActiveTasks = $active.Count; RepositoryTEMP = $repoTemp.Count; RepositoryUnknownTemp = $repoUnknown.Count; CandidateScopedActiveWriters = 0; ConsumedActiveForeignTemp = 0; CandidateScopedUnknownTemp = 0; CandidateScopedOwnershipConflict = 0; CandidateChangedDuringCertification = 'NO'; OpenDependencies=0;LineLimitViolations=0;WaveStatus=$wave;HistoricalBaselineDebtCount=@(Candidate-Items $Facts 'HistoricalBaselineDebt').Count;CandidateScopeStatus='NOT_CONFIGURED';GlobalBaselineStatus=$global;OverallGateStatus='CANDIDATE_NOT_CONFIGURED';CommitEligibility='NOT_CONFIGURED';FinalEvidenceEligibility='NOT_CONFIGURED';CertificationAllowed='NOT_CONFIGURED' }
    }
    $candidate = Get-Content -Raw -LiteralPath $path | ConvertFrom-Json
    $closure = @(Candidate-Closure $candidate)
    $scoped = @($Classes | Where-Object { Candidate-InClosure ([string]$_.Path) $closure })
    $writers = @($active | Where-Object { $_.TaskId -ne $candidate.OwnerTaskId -and (Candidate-TaskHits $_ $closure) })
    $open=Candidate-OpenDependencies $candidate;$line=Candidate-LineViolations $Root $candidate
    $blocked = @($scoped | Where-Object { $_.Classification -in @('FOREIGN_KNOWN','UNAUTHORIZED_DIRTY','OWNERSHIP_CONFLICT','FRIENDLY_COMPLETED') }).Count + $writers.Count + $open.Count + $line.Count -gt 0
    $outcome=Candidate-Outcome $blocked $global
    [pscustomobject]@{ RepositoryActiveTasks = $active.Count; RepositoryTEMP = $repoTemp.Count; RepositoryUnknownTemp = $repoUnknown.Count; CandidateScopedActiveWriters = $writers.Count; ConsumedActiveForeignTemp = @($scoped | Where-Object Classification -eq 'FOREIGN_KNOWN').Count; CandidateScopedUnknownTemp = @($scoped | Where-Object Classification -eq 'UNAUTHORIZED_DIRTY').Count; CandidateScopedOwnershipConflict = @($scoped | Where-Object Classification -eq 'OWNERSHIP_CONFLICT').Count; CandidateChangedDuringCertification = 'NO';OpenDependencies=$open.Count;LineLimitViolations=$line.Count;WaveStatus=$wave;HistoricalBaselineDebtCount=@(Candidate-Items $Facts 'HistoricalBaselineDebt').Count;CandidateScopeStatus=$outcome.CandidateScopeStatus;GlobalBaselineStatus=$outcome.GlobalBaselineStatus;OverallGateStatus=$outcome.OverallGateStatus;CommitEligibility=$outcome.CommitEligibility;FinalEvidenceEligibility = if ($blocked) { 'NO' } else { 'YES' }; CertificationAllowed = if ($blocked) { 'NO' } else { 'YES' } }
}
