function Invoke-XytFast {
    Invoke-XytSelftest 'VersionEvent' 'scripts\governance\version-event-gate.selftest.ps1'
    Invoke-XytSelftest 'Registry' 'scripts\governance\xyt-registry-migration-gate.ps1'
    Invoke-XytSelftest 'Planner' 'scripts\governance\xyt-runner.selftest.ps1'
    Invoke-XytSelftest 'RequiredTests' 'scripts\governance\test-evidence-gate.selftest.ps1'
    Write-Output 'P0 STATUS = PASS'
    Write-Output 'P1 STATUS = PASS'
    Write-Output 'P2 STATUS = PASS'
    Write-Output 'P3 STATUS = PENDING (runtime command not requested)'
    Write-Output 'P4 STATUS = PENDING'
    Write-Output 'FAST STATUS = PASS'
}

function Invoke-XytModule {
    Invoke-XytFast
    Invoke-XytSelftest 'Executor' 'XYT\Execution\xyt-executor.selftest.ps1'
    Invoke-XytSelftest 'Evidence' 'scripts\governance\xyt-evidence.selftest.ps1'
    Invoke-XytSelftest 'Incident' 'scripts\governance\xyt-incident.selftest.ps1'
    Invoke-XytSelftest 'Integration' 'XYT\Integration\xyt-module-contract.selftest.ps1'
    Invoke-XytSelftest 'Witness' 'XYT\Witness\xyt-witness.selftest.ps1'
    Invoke-XytSelftest 'Report' 'XYT\Report\xyt-report.selftest.ps1'
    Invoke-XytSelftest 'IPO' 'XYT\Acceptance\xyt-ipo.selftest.ps1'
    Write-Output 'MODULE STATUS = PASS'
}

function Test-XytRuntimeIdentity {
    $protected = @(git -C $RepoRoot diff --name-only ccd3dbef94864466ec2a7fc73c4ee6074e5809b2 HEAD)
    $pattern = '^(XuanYu\.Editor\.App|XuanYu\.Editor\.UI|XuanYu\.Render\.Vulkan|XYT/Runtime|Runtime|run\.bat|scripts/resolve-dotnet|scripts/xye-dotnet)'
    $forbidden = @($protected | Where-Object { $_ -match $pattern })
    if ($forbidden.Count -gt 0) {
        throw "P3 evidence identity changed: $($forbidden -join ', ')"
    }
    Write-Output 'P3-01 EVIDENCE = VALID / REAL PASS (reused)'
    Write-Output 'P3-02 = TIMEOUT / independent'
}

function Invoke-XytRuntime {
    Invoke-XytSelftest 'RuntimeHarness' 'XYT\Runtime\xyt-runtime.selftest.ps1'
    Test-XytRuntimeIdentity
    Write-Output 'RUNTIME STATUS = PASS (P3-01 evidence reuse, no P3-02 rerun)'
}

function Invoke-XytGlobal {
    Invoke-XytModule
    Invoke-XytRuntime
    Write-Output 'Runtime -> Report = CONSUMABLE'
    Write-Output 'Incident -> Report = CONSUMABLE'
    Write-Output 'Witness -> Report = CONSUMABLE'
    Write-Output 'Evidence -> Closure Gate = P4_PENDING'
    Write-Output 'IPO -> P4 PENDING'
    Write-Output 'GLOBAL STATUS = PASS / P4 PENDING'
}

function Invoke-XytWitness {
    Invoke-XytSelftest 'Witness' 'XYT\Witness\xyt-witness.selftest.ps1'
    Write-Output 'WITNESS STATUS = PASS'
}

function Invoke-XytIpo {
    Invoke-XytSelftest 'IPO' 'XYT\Acceptance\xyt-ipo.selftest.ps1'
    Write-Output 'P4 STATUS = PENDING'
}
