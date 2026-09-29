[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$executor = Join-Path $root 'scripts\governance\xyt-executor.ps1'
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-execution-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
    $plan = [ordered]@{
        planId = 'XYT-D-SELFTEST'
        tests = @(
            [ordered]@{ testId='parallel-a'; command='Start-Sleep -Milliseconds 300; exit 0'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='parallel-b'; command='Start-Sleep -Milliseconds 300; exit 0'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='serial'; dependsOn=@('parallel-a'); command='Start-Sleep -Milliseconds 50; exit 0'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='stable-fail'; command='exit 7'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='flaky'; command="if (`$env:XYT_ATTEMPT -eq '1') { exit 9 } else { exit 0 }"; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='timeout'; command='Start-Sleep -Seconds 3; exit 0'; maxDurationSeconds=3; timeoutSeconds=2 }
            [ordered]@{ testId='blocked'; dependsOn=@('stable-fail'); command='exit 0'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='sweep-pass'; command='exit 0'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='sweep-fail'; command='exit 5'; maxDurationSeconds=30; timeoutSeconds=10 }
            [ordered]@{ testId='cost-warning'; command='Start-Sleep -Milliseconds 250; exit 0'; maxDurationSeconds=0.05; timeoutSeconds=3 }
            [ordered]@{ testId='unclassified'; maxDurationSeconds=3; timeoutSeconds=3 }
        )
    }
    $planPath = Join-Path $temp 'plan.json'
    $resultPath = Join-Path $temp 'result.json'
    $plan | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $planPath -Encoding UTF8
    & $executor -PlanPath $planPath -OutputPath $resultPath | Out-Null
    $result = Get-Content -Raw $resultPath | ConvertFrom-Json
    $byId = @{}; foreach ($item in @($result.results)) { $byId[$item.testId] = $item }
    if ($byId['stable-fail'].status -ne 'FAIL' -or $byId['stable-fail'].attempts -ne 2) { throw 'stable FAIL contract failed' }
    if ($byId['stable-fail'].rootCause -ne 'UNKNOWN') { throw 'UNKNOWN root cause contract failed' }
    if ($byId['flaky'].status -ne 'FLAKY' -or $byId['flaky'].attempts -ne 2) { throw 'FLAKY contract failed' }
    if ($byId['timeout'].status -ne 'TIMEOUT') { throw 'TIMEOUT contract failed' }
    if ($byId['blocked'].status -ne 'BLOCKED_BY') { throw 'BLOCKED_BY contract failed' }
    if ($byId['sweep-pass'].status -ne 'PASS' -or $byId['sweep-fail'].status -ne 'FAIL') { throw 'failure sweep stopped early' }
    if (-not $byId['cost-warning'].costWarning) { throw 'cost warning contract failed' }
    if ($byId['unclassified'].status -ne 'UNCLASSIFIED') { throw 'UNCLASSIFIED contract failed' }
    $a = $byId['parallel-a']; $b = $byId['parallel-b']; $s = $byId['serial']
    if (-not ([datetime]$a.startedAt -lt [datetime]$b.endedAt -and [datetime]$b.startedAt -lt [datetime]$a.endedAt)) { throw 'parallel contract failed' }
    if ([datetime]$s.startedAt -lt [datetime]$a.endedAt) { throw 'dependency serial contract failed' }
    'XYT EXECUTOR SELFTEST: PASS'
    exit 0
}
finally { Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue }
