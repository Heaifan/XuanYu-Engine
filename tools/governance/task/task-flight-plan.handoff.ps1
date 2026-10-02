$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot '..\candidate\candidate-scope.ps1')
. (Join-Path $PSScriptRoot '..\candidate\candidate-projection.ps1')
function Read-TaskJson([string]$Path) { if (Test-Path -LiteralPath $Path -PathType Leaf) { return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json }; return $null }
function Task-Items($Value, [string]$Name) { if ($null -eq $Value) { return @() }; if ($Value.PSObject.Properties.Name -contains $Name) { return @($Value.$Name) }; return @($Value) }
function Task-Path([string]$Line) { $p = if ($Line.Length -gt 3) { $Line.Substring(3).Trim() } else { $Line.Trim() }; if ($p.Contains(' -> ')) { $p = ($p -split ' -> ')[-1] }; return $p.Trim('"') -replace '\\', '/' }
function Invoke-TaskClassifier([string]$Script, [string]$Root, [string]$Path) { $out = @(& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $Script -RepositoryRoot $Root -CurrentTaskId '__HANDOFF__' -DirtyPath $Path 2>$null); if ($out.Count) { try { $r=(($out -join "`n") | ConvertFrom-Json);$r|Add-Member NoteProperty Path $Path -Force;return $r } catch {} }; return $null }
function Get-TaskFlightReport([string]$Root, $Facts, $State) {
    $dir = Join-Path $Root '.git\xye-handoff'; $reg = Read-TaskJson (Join-Path $dir 'task-registry.json'); $tasks = Task-Items $reg 'tasks'
    $report = Read-TaskJson (Join-Path $dir 'task-reports.json'); $reports = Task-Items $report 'reports'
    $active = @($tasks | ? Status -eq 'ACTIVE'); $released = @($tasks | ? Status -eq 'RELEASED')
    $leaks = @($reports | ? { $_.Status -in @('COMPLETE','IMPLEMENTATION_COMPLETE','HANDOFF_READY') } | ? { $id=[string]$_.TaskId; @($active | ? { [string]$_.TaskId -eq $id }).Count })
    $classes = @(); $classifier = Join-Path $PSScriptRoot 'task-dirty-classifier.ps1'; $paths = @($Facts.Dirty | % { Task-Path ([string]$_) })
    if (Test-Path -LiteralPath $classifier) { if ($null -eq $reg -and !(Test-Path (Join-Path $dir 'dependency-state.json')) -and $paths.Count -gt 0) { $sample = Invoke-TaskClassifier $classifier $Root $paths[0]; $classes = @($paths | % { $sample }) } else { $paths | % { $classes += Invoke-TaskClassifier $classifier $Root $_ } } }
    $candidate=Get-CandidateGateProjection $Root $Facts $tasks $classes $State
    [pscustomobject]@{ Active=$active; Released=$released; Classes=$classes; Leaks=$leaks; LeakPresent=($leaks.Count -gt 0); CandidateGate=$candidate }
}
function Show-TaskFlightReport($Report) {
    Write-Host "ACTIVE TASKS: $($Report.Active.Count)"; $Report.Active | % { Write-Host "  $($_.TaskId) $($_.Owner) $($_.Status)" }
    Write-Host "RELEASED TASKS: $($Report.Released.Count)"; $Report.Released | % { Write-Host "  $($_.TaskId) $($_.Owner) $($_.Status)" }
    foreach ($name in @('FRIENDLY_ACTIVE','FRIENDLY_COMPLETED','FOREIGN_KNOWN','UNAUTHORIZED_DIRTY')) { Write-Host "$name`: $(@($Report.Classes | ? Classification -eq $name).Count)" }
    Write-Host "FinalEvidenceEligibility: $(if (@($Report.Classes | ? FinalEvidenceEligible -eq 'NO').Count -gt 0) { 'NO' } else { 'YES' })"
    Write-Host "ACTIVE TASK LEAK: $(if ($Report.LeakPresent) { 'PRESENT' } else { 'NONE' })"
    $g=$Report.CandidateGate
    Write-Host "REPOSITORY ACTIVE TASKS: $($g.RepositoryActiveTasks)";Write-Host "REPOSITORY TEMP: $($g.RepositoryTEMP)";Write-Host "REPOSITORY UNKNOWN TEMP: $($g.RepositoryUnknownTemp)"
    Write-Host "CANDIDATE-SCOPED ACTIVE WRITERS: $($g.CandidateScopedActiveWriters)";Write-Host "CONSUMED ACTIVE FOREIGN TEMP: $($g.ConsumedActiveForeignTemp)";Write-Host "CANDIDATE-SCOPED UNKNOWN TEMP: $($g.CandidateScopedUnknownTemp)";Write-Host "CANDIDATE-SCOPED OWNERSHIP CONFLICT: $($g.CandidateScopedOwnershipConflict)";Write-Host "CANDIDATE CHANGED DURING CERTIFICATION: $($g.CandidateChangedDuringCertification)";Write-Host "FINAL EVIDENCE ELIGIBILITY: $($g.FinalEvidenceEligibility)";Write-Host "CERTIFICATION ALLOWED: $($g.CertificationAllowed)"
}
function Assert-TaskFlightGate($Report) { if ($Report.LeakPresent) { Write-Host "TASK_STATE_LEAK PRESENT: $((@($Report.Leaks | % TaskId) -join ', '))"; Write-Host 'COMMIT_ELIGIBILITY = NO'; exit 1 } }
