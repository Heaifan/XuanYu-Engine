$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
$knownPath = Join-Path $root "docs/architecture/viewport-migration-known-failures-r1.txt"
$resultRoot = Join-Path ([IO.Path]::GetTempPath()) ("xye-viewport-baseline-" + [guid]::NewGuid().ToString("N"))
$known = @(Get-Content -LiteralPath $knownPath | Where-Object { $_.Trim() })
$projects = @("XuanYu.Core.Tests/XuanYu.Core.Tests.csproj", "XuanYu.World.Tests/XuanYu.World.Tests.csproj")
$trxs = New-Object System.Collections.Generic.List[string]
$failed = New-Object System.Collections.Generic.List[string]
$passed = 0
$skipped = 0
$testExit = 0
New-Item -ItemType Directory -Path $resultRoot | Out-Null

try {
    foreach ($project in $projects) {
        & dotnet restore $project --disable-parallel
        if ($LASTEXITCODE -ne 0) { throw "restore failed: $project" }
        $name = [IO.Path]::GetFileNameWithoutExtension($project)
        $trx = Join-Path $resultRoot "$name.trx"
        & dotnet test $project --no-restore -p:IsTestProject=true -m:1 -nr:false -p:UseSharedCompilation=false `
            --logger "trx;LogFileName=$name.trx" --results-directory $resultRoot
        if ($LASTEXITCODE -ne 0) { $testExit = 1 }
        $trxs.Add($trx)
    }
    foreach ($trx in $trxs) {
        $run = [xml](Get-Content -LiteralPath $trx -Raw)
        $counters = $run.TestRun.ResultSummary.Counters
        $passed += [int]$counters.passed
        $skipped += [int]$counters.notExecuted
        foreach ($result in @($run.TestRun.Results.UnitTestResult | Where-Object outcome -eq "Failed")) {
            $failed.Add([string]$result.testName)
        }
    }
    $knownFailed = @($failed | Where-Object { $known -contains $_ })
    $newFailed = @($failed | Where-Object { $known -notcontains $_ })
    Write-Host "Passed: $passed"
    Write-Host "Failed: $($failed.Count)"
    Write-Host "Known Baseline: $($knownFailed.Count)"
    Write-Host "New Regression: $($newFailed.Count)"
    if ($knownFailed.Count -gt 0) { $knownFailed | ForEach-Object { Write-Host "KNOWN BASELINE FAILURE: $_" } }
    if ($newFailed.Count -gt 0) { $newFailed | ForEach-Object { Write-Error "NEW REGRESSION: $_" } }
    if ($skipped -gt 0) { throw "skipped/not executed tests: $skipped" }
    if ($passed -eq 0) { throw "zero tests executed" }
    if ($newFailed.Count -gt 0 -or ($testExit -ne 0 -and $failed.Count -eq 0)) {
        throw "viewport regression baseline failed"
    }
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot "arch-a-guard.ps1")
    if ($LASTEXITCODE -ne 0) { throw "ARCH-A failed (including ARCH-VIEWPORT-R1 and 5+100)" }
    git diff --check
    if ($LASTEXITCODE -ne 0) { throw "git diff --check failed" }
    Write-Host "ARCH-A: PASS"
    Write-Host "ARCH-VIEWPORT-R1: PASS (included by ARCH-A)"
    Write-Host "5+100: PASS (included by ARCH-A)"
    Write-Host "git diff --check: PASS"
    Write-Host "Viewport Migration Regression Baseline: PASS"
}
finally {
    Remove-Item -LiteralPath $resultRoot -Recurse -Force -ErrorAction SilentlyContinue
}
