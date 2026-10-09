function Invoke-XytFast {
    Invoke-XytSelftest 'TestRouting' 'XYT\Registry\xyt-runner.selftest.ps1'
    Invoke-XytSelftest 'ExecutionEvidence' 'XYT\tests\xyt.test.selftest.ps1'
    Invoke-XytSelftest 'Reports' 'XYT\Report\xyt-report.selftest.ps1'
    Write-Output 'XYT FAST SELFTESTS = PASS'
}

function Invoke-XytModule {
    Invoke-XytFast
    $tests = @(
        'XYT\Execution\xyt-executor.selftest.ps1',
        'XYT\Execution\xyt-executor-deadlock.selftest.ps1',
        'XYT\Integration\xyt-module-contract.selftest.ps1',
        'XYT\Runtime\xyt-runtime.selftest.ps1',
        'XYT\Witness\xyt-witness.selftest.ps1',
        'XYT\Acceptance\xyt-ipo.selftest.ps1',
        'XYT\tests\xyt.aggregate.selftest.ps1',
        'XYT\tests\xyt.upload.selftest.ps1'
    )
    foreach ($test in $tests) { Invoke-XytSelftest 'XYT' $test }
    Write-Output 'XYT MODULE SELFTESTS = PASS'
}

function Invoke-XytRuntime {
    Invoke-XytSelftest 'RuntimeHarness' 'XYT\Runtime\xyt-runtime.selftest.ps1'
    Write-Output 'RUNTIME HARNESS SELFTEST = PASS'
    Write-Output 'REAL PRODUCT RUNTIME = NOT RUN'
}

function Invoke-XytGlobal {
    Invoke-XytModule
    Write-Output 'XYT AUTOMATED TESTS = PASS'
    Write-Output 'P3 REAL RUNTIME = NOT RUN'
    Write-Output 'P4 PRODUCT ACCEPTANCE = PENDING'
}

function Invoke-XytWitness {
    Invoke-XytSelftest 'Witness' 'XYT\Witness\xyt-witness.selftest.ps1'
}

function Invoke-XytIpo {
    Invoke-XytSelftest 'IPO' 'XYT\Acceptance\xyt-ipo.selftest.ps1'
    Write-Output 'P4 PRODUCT ACCEPTANCE = PENDING'
}
