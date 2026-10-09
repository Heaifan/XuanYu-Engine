$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-integration-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
    $planPath = Join-Path $temp 'plan.json'; $resultPath = Join-Path $temp 'execution.json'
    [ordered]@{ planId='XYT-INTEGRATION'; tests=@([ordered]@{testId='T-UNIT'; command='Write-Output evidence-linked; exit 0'; timeoutSeconds=20}) } | ConvertTo-Json -Depth 6 | Set-Content $planPath
    $executor = Join-Path $PSScriptRoot '..\Execution\xyt-executor.ps1'
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $executor -PlanPath $planPath -OutputPath $resultPath | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'XYT executor did not finish the integration plan.' }
    $execution = Get-Content -Raw $resultPath | ConvertFrom-Json
    if ($execution.status -ne 'PASS' -or $execution.results[0].testId -ne 'T-UNIT') { throw 'Executor result did not retain the executed test.' }
    . (Join-Path $PSScriptRoot '..\Report\xyt-report.ps1')
    $report = Invoke-XytReport -Operation Report -RepositoryRoot $root -OutputRoot $temp -TestMode 'UNIT' -TestSetVersion 'TSET-XYT-v1.0' -AffectedCapability @('XYT') -Status $execution.status -Evidence @($resultPath)
    $record = Get-Content -Raw $report.JsonPath | ConvertFrom-Json
    $head = (& git -C $root rev-parse HEAD).Trim()
    if ($record.Status -ne $execution.status -or $record.Commit -ne $head -or @($record.Evidence) -notcontains $resultPath) { throw 'Report lost execution status, raw evidence, or current commit identity.' }
    $ipoPath = Join-Path $PSScriptRoot '..\Acceptance\xyt-ipo.ps1'; $ipoOut = Join-Path $temp 'ipo.json'
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $ipoPath -Capability 'mode-switch' -Version $record.Version -Commit $record.Commit -Branch $record.Branch -OutputPath $ipoOut | Out-Null
    if ($LASTEXITCODE -ne 0 -or (Get-Content -Raw $ipoOut | ConvertFrom-Json).Items[0].判定 -ne 'P4 PENDING') { throw 'IPO generator did not preserve user-owned P4 verdict.' }
    'XYT INTEGRATION CONTRACT SELFTEST PASS'
    'EXECUTOR -> REPORT -> IPO: PASS'
    'REAL RUNTIME: NOT PROVEN'
} finally { Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue }
