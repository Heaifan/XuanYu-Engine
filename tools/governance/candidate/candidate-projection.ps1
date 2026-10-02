$ErrorActionPreference = 'Stop'

function Get-CandidateGateProjection([string]$Root, $Facts, $Tasks, $Classes, $State) {
    $path = if ($State -and $State.PSObject.Properties['candidatePath']) {
        [string]$State.candidatePath
    } else { Join-Path $Root '.git\xye-handoff\candidate.json' }
    $active = @($Tasks | Where-Object Status -eq 'ACTIVE')
    $repoTemp = @($Facts.Dirty)
    $repoUnknown = @($Classes | Where-Object Classification -eq 'UNAUTHORIZED_DIRTY')
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) {
        return [pscustomobject]@{ RepositoryActiveTasks = $active.Count; RepositoryTEMP = $repoTemp.Count; RepositoryUnknownTemp = $repoUnknown.Count; CandidateScopedActiveWriters = 0; ConsumedActiveForeignTemp = 0; CandidateScopedUnknownTemp = 0; CandidateScopedOwnershipConflict = 0; CandidateChangedDuringCertification = 'NO'; FinalEvidenceEligibility = 'NOT_CONFIGURED'; CertificationAllowed = 'NOT_CONFIGURED' }
    }
    $candidate = Get-Content -Raw -LiteralPath $path | ConvertFrom-Json
    $closure = @(Candidate-Closure $candidate)
    $scoped = @($Classes | Where-Object { Candidate-InClosure ([string]$_.Path) $closure })
    $writers = @($active | Where-Object { $_.TaskId -ne $candidate.OwnerTaskId -and (Candidate-TaskHits $_ $closure) })
    $blocked = @($scoped | Where-Object { $_.Classification -in @('FOREIGN_KNOWN','UNAUTHORIZED_DIRTY','OWNERSHIP_CONFLICT','FRIENDLY_COMPLETED') }).Count + $writers.Count -gt 0
    [pscustomobject]@{ RepositoryActiveTasks = $active.Count; RepositoryTEMP = $repoTemp.Count; RepositoryUnknownTemp = $repoUnknown.Count; CandidateScopedActiveWriters = $writers.Count; ConsumedActiveForeignTemp = @($scoped | Where-Object Classification -eq 'FOREIGN_KNOWN').Count; CandidateScopedUnknownTemp = @($scoped | Where-Object Classification -eq 'UNAUTHORIZED_DIRTY').Count; CandidateScopedOwnershipConflict = @($scoped | Where-Object Classification -eq 'OWNERSHIP_CONFLICT').Count; CandidateChangedDuringCertification = 'NO'; FinalEvidenceEligibility = if ($blocked) { 'NO' } else { 'YES' }; CertificationAllowed = if ($blocked) { 'NO' } else { 'YES' } }
}
