[CmdletBinding()]
param([Parameter(Mandatory)][string]$PlanPath, [string]$OutputPath = '')
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'xyt-executor-errors.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-cycle.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-process.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-result.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-schedule.ps1')
try {
    $plan = Get-Content -Raw -LiteralPath $PlanPath | ConvertFrom-Json
    $items = @($plan.tests); $ids = @($items | ForEach-Object testId)
    if ($ids.Count -ne (@($ids | Sort-Object -Unique)).Count) { throw 'Test Plan contains duplicate testId' }
    $results = @(Invoke-XytSchedule $plan)
    $status = if (@($results | Where-Object status -eq 'FAIL').Count) { 'FAIL' } elseif (@($results | Where-Object status -eq 'TIMEOUT').Count) { 'TIMEOUT' } elseif (@($results | Where-Object status -eq 'BLOCKED_BY').Count) { 'BLOCKED_BY' } elseif (@($results | Where-Object status -eq 'UNCLASSIFIED').Count) { 'UNCLASSIFIED' } elseif (@($results | Where-Object status -eq 'FLAKY').Count) { 'FLAKY' } else { 'PASS' }
    $execution = [ordered]@{ schemaVersion='XYT-D-EXEC-v1.0'; planId=$plan.planId; startedAt=[datetime]::UtcNow; endedAt=[datetime]::UtcNow; status=$status; results=$results }
    $json = $execution | ConvertTo-Json -Depth 12
    if ($OutputPath) { $json | Set-Content -LiteralPath $OutputPath -Encoding UTF8 }
    $json
} catch [XytDependencyError] {
    $failure = [ordered]@{ schemaVersion='XYT-D-EXEC-v1.0'; status='ERROR'; errorCode=$_.Exception.ErrorCode; cyclePath=$_.Exception.CyclePath; message=$_.Exception.Message }
    $json = $failure | ConvertTo-Json -Depth 8
    if ($OutputPath) { $json | Set-Content -LiteralPath $OutputPath -Encoding UTF8 }
    $json
    exit 2
} catch { Write-Error $_; exit 1 }
