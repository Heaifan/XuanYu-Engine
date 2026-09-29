[CmdletBinding()]
param([Parameter(Mandatory)][string]$PlanPath, [string]$OutputPath = '')
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'xyt-executor-attempt.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-result.ps1')
. (Join-Path $PSScriptRoot 'xyt-executor-scheduler.ps1')
$plan = Get-Content -Raw -LiteralPath $PlanPath | ConvertFrom-Json
$items = @($plan.tests); $ids = @($items | ForEach-Object testId)
if ($ids.Count -ne (@($ids | Sort-Object -Unique)).Count) { throw 'Test Plan contains duplicate testId' }
$execution = [ordered]@{
    schemaVersion='XYT-D-EXEC-v1.0'; planId=$plan.planId; startedAt=[datetime]::UtcNow
    results=@(Invoke-XytSchedule -Plan $plan)
}
$execution.endedAt = [datetime]::UtcNow
$execution.status = if (@($execution.results | Where-Object status -eq 'FAIL').Count) { 'FAIL' } elseif (@($execution.results | Where-Object status -eq 'TIMEOUT').Count) { 'TIMEOUT' } elseif (@($execution.results | Where-Object status -eq 'BLOCKED_BY').Count) { 'BLOCKED_BY' } elseif (@($execution.results | Where-Object status -eq 'UNCLASSIFIED').Count) { 'UNCLASSIFIED' } elseif (@($execution.results | Where-Object status -eq 'FLAKY').Count) { 'FLAKY' } else { 'PASS' }
$json = $execution | ConvertTo-Json -Depth 12
if ($OutputPath) { $json | Set-Content -LiteralPath $OutputPath -Encoding UTF8 }
$json
