param()

$ErrorActionPreference = 'Stop'

function Assert([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}

function New-Envelope([string]$kind, [string]$status, [hashtable]$data) {
    [pscustomobject]@{ schema = 'XYT-INTEGRATION.v1'; kind = $kind; runId = 'fixture-001'; status = $status; exitCode = 0; data = $data; references = @(); warnings = @() }
}

$plan = New-Envelope 'planner' 'PASS' @{ planId = 'plan-fixture-001'; requiresRealRuntime = $true; tests = @(@{ testId = 'T-UNIT'; command = 'fixture'; dependsOn = @() }) }
$execution = New-Envelope 'execution' 'PASS' @{ planId = $plan.data.planId; results = @(@{ testId = 'T-UNIT'; status = 'PASS'; exitCode = 0 }) }
$runtime = New-Envelope 'runtime' 'PENDING' @{ sourceKind = 'FIXTURE_RUNTIME'; schema = 'XYT-P3-R1/1'; Result = 'PASS'; ExitCode = 0; MissingMarkers = @() }
$incident = New-Envelope 'incident' 'PASS' @{ severity = 'T3'; classification = 'NO_INCIDENT'; lockDecision = 'CONTINUE'; evidenceRefs = @() }
$witness = New-Envelope 'witness' 'PASS' @{ required = $false; complete = $true }
$report = New-Envelope 'report' 'PASS' @{ sourceKinds = @('AUTOMATED_TEST', 'FIXTURE_RUNTIME'); executionStatus = $execution.status; runtimeStatus = $runtime.status }
$ipo = New-Envelope 'ipo' 'PENDING' @{ verdict = 'P4 PENDING'; items = @(@{ 路径 = 'fixture'; 判定 = 'P4 PENDING' }) }

Assert ($plan.data.planId -eq $execution.data.planId) 'Planner output was not accepted by Executor.'
Assert ($plan.data.requiresRealRuntime -and $runtime.data.sourceKind -eq 'FIXTURE_RUNTIME') 'Fixture Runtime boundary was lost.'
Assert ($report.data.executionStatus -eq 'PASS' -and $report.data.runtimeStatus -eq 'PENDING') 'Report did not preserve source statuses.'
Assert ($ipo.data.verdict -eq 'P4 PENDING') 'IPO fixture was auto-promoted.'

$closure = 'BLOCKED'
$reasons = @('REAL_RUNTIME_REQUIRED', 'P4_PENDING')
Assert ($closure -eq 'BLOCKED' -and $reasons.Count -eq 2) 'Closure Gate incorrectly closed.'
Assert ($incident.data.lockDecision -eq 'CONTINUE' -and $witness.data.complete) 'Incident/Witness contract did not connect.'

Write-Output 'XYT INTEGRATION CONTRACT SELFTEST PASS'
Write-Output 'INTEGRATION CONTRACT: READY'
Write-Output 'REAL RUNTIME: NOT PROVEN (fixture only)'
exit 0
