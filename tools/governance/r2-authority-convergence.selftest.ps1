[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$tests = @(
    'candidate/candidate-scoped-gate.selftest.ps1',
    'coordinator/active-wave-migration.selftest.ps1',
    'ownership/dependency-handoff.selftest.ps1',
    'release/dirty-content-fingerprint.selftest.ps1',
    'task/task-dirty-classifier.selftest.ps1',
    'task/task-flight-plan.selftest.ps1',
    'task/task-flight-plan-lifecycle.selftest.ps1',
    'workspace/unauthorized-dirty-recovery.selftest.ps1'
)

$failed = [Collections.Generic.List[string]]::new()
foreach ($relative in $tests) {
    $path = Join-Path $PSScriptRoot $relative
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        $failed.Add("MISSING: $relative")
        continue
    }
    Write-Output "=== AUTHORITY $relative ==="
    & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $path
    if ($LASTEXITCODE -ne 0) { $failed.Add("FAIL: $relative") }
}

if ($failed.Count -gt 0) {
    $failed | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output ('R2 AUTHORITY CONVERGENCE SELFTEST PASS {0}/{0}' -f $tests.Count)
exit 0
