[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'task-dirty-classifier.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('dirty-classifier-' + [guid]::NewGuid().ToString('N'))
function Assert([bool]$ok, [string]$message) { if (!$ok) { throw "SELFTEST FAILED: $message" } }
function Write-Json([string]$path, $value) {
    New-Item -ItemType Directory -Force (Split-Path $path) | Out-Null
    $value | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $path -Encoding UTF8
}
function Invoke-Classifier([string]$task, [string]$path) {
    $r = & pwsh -NoLogo -NoProfile -File $scriptPath -RepositoryRoot $root -CurrentTaskId $task -DirtyPath $path -TaskRegistryPath (Join-Path $root 'registry.json') -DependencyStatePath (Join-Path $root 'dependency-state.json') 2>&1
    Assert ($LASTEXITCODE -eq 0) "classifier failed: $($r -join "`n")"
    $r -join "`n" | ConvertFrom-Json
}
try {
    New-Item -ItemType Directory -Force $root | Out-Null
    $tasks = @(
        [pscustomobject]@{ TaskId='XYT'; Owner='owner-x'; Status='ACTIVE'; WriteScope=@('XuanYu.World/**'); ExpectedDependencies=@('XuanYu.Render.Vulkan') },
        [pscustomobject]@{ TaskId='SRP'; Owner='owner-s'; Status='ACTIVE'; WriteScope=@('XuanYu.Render.Vulkan/**'); ExpectedDependencies=@() },
        [pscustomobject]@{ TaskId='RELEASED'; Owner='owner-r'; Status='RELEASED'; WriteScope=@('released/**'); ExpectedDependencies=@() },
        [pscustomobject]@{ TaskId='A'; Owner='owner-a'; Status='ACTIVE'; WriteScope=@('conflict/**'); ExpectedDependencies=@() },
        [pscustomobject]@{ TaskId='B'; Owner='owner-b'; Status='ACTIVE'; WriteScope=@('conflict/**'); ExpectedDependencies=@() },
        [pscustomobject]@{ TaskId='FRIEND'; Owner='owner-f'; Status='ACTIVE'; WriteScope=@('friendly/**'); ExpectedDependencies=@() }
    )
    Write-Json (Join-Path $root 'registry.json') ([pscustomobject]@{ tasks=$tasks })
    Write-Json (Join-Path $root 'dependency-state.json') ([pscustomobject]@{ files=@(
        [pscustomobject]@{ file='released/item.cs'; owner='RELEASED'; status='RELEASED' }
    ) })
    $r = Invoke-Classifier 'XYT' 'XuanYu.World/map.cs'; Assert ($r.Classification -eq 'FRIENDLY_ACTIVE' -and $r.FinalEvidenceEligible -eq 'YES') 'friendly active'
    $r = Invoke-Classifier 'XYT' 'unclaimed/item.cs'; Assert ($r.Classification -eq 'UNAUTHORIZED_DIRTY' -and $r.RecoveryWindow -eq 'RECOVERY_WINDOW') 'unauthorized'
    $r = Invoke-Classifier 'XYT' 'XuanYu.Render.Vulkan/shader.cs'; Assert ($r.Classification -eq 'FRIENDLY_ACTIVE' -and $r.OwnerTaskId -eq 'SRP') 'foreign active owner'
    $r = Invoke-Classifier 'XYT' 'released/item.cs'; Assert ($r.Classification -eq 'FRIENDLY_COMPLETED' -and $r.CodingAllowed -eq 'YES') 'completed'
    $r = Invoke-Classifier 'XYT' 'conflict/item.cs'; Assert ($r.Classification -eq 'UNAUTHORIZED_DIRTY' -and $r.CodingAllowed -eq 'NO') 'ambiguous'
    $r = Invoke-Classifier 'XYT' 'unexplained/item.cs'; Assert ($r.Classification -eq 'UNAUTHORIZED_DIRTY' -and $r.CandidateAllowed -eq $false -and $r.ReleaseAllowed -eq $false) 'unauthorized'
    $r = Invoke-Classifier 'XYT' 'foreign/item.cs'; Assert ($r.Classification -eq 'UNAUTHORIZED_DIRTY') 'unclaimed foreign'
    $lines = (Get-Content -LiteralPath $scriptPath).Count; Assert ($lines -le 100) "classifier is $lines lines"
    $lines = (Get-Content -LiteralPath $PSCommandPath).Count; Assert ($lines -le 100) "selftest is $lines lines"
    'TASK DIRTY CLASSIFIER SELFTEST PASS 7/7'
} finally { if (Test-Path $root) { Remove-Item -LiteralPath $root -Recurse -Force } }
