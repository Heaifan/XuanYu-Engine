$ErrorActionPreference = 'Stop'
$module = Join-Path $PSScriptRoot 'xyt-report.ps1'
. $module
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-report-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
    $report = Invoke-XytReport -Operation Report -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -OutputRoot $temp -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability @('XYT','REPORT') -Status PASS -Evidence @('evidence.log') -Timestamp '2026-09-29T10:00:00Z'
    if (-not (Test-Path $report.JsonPath) -or -not (Test-Path $report.MarkdownPath)) { throw 'JSON and Markdown report were not created.' }
    $record = Get-Content $report.JsonPath -Raw | ConvertFrom-Json
    foreach ($field in @('Version','Commit','Branch','TestMode','TestSetVersion','Timestamp')) { if ([string]::IsNullOrWhiteSpace([string]$record.$field)) { throw "Missing $field" } }
    if ($record.Status -ne 'PASS' -or @($record.AffectedCapabilities).Count -ne 2) { throw 'Report fields were not preserved.' }
    foreach ($status in @('FAIL','BLOCKED','TIMEOUT','FLAKY')) {
        $item = Invoke-XytReport -Operation Report -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -OutputRoot $temp -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status $status
        if ((Get-Content $item.JsonPath -Raw | ConvertFrom-Json).Status -ne $status) { throw "Status $status was not preserved." }
    }
    $bad = $false
    try { Invoke-XytReport -Operation Report -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -OutputRoot $temp -TestMode 'UNIT' -AffectedCapability 'XYT' -Status PASS | Out-Null } catch { $bad = $true }
    if (-not $bad) { throw 'Missing TestSetVersion was accepted.' }
    [void](Invoke-XytReport -Operation Report -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -OutputRoot $temp -TestMode 'HEADLESS' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status FAIL -Timestamp '2026-09-30T10:00:00Z')
    $aggregate = Invoke-XytReport -Operation Aggregate -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -InputRoot $temp -OutputRoot $temp -Period month -At '2026-09-30T12:00:00Z'
    $aggregateRecord = Get-Content $aggregate.JsonPath -Raw | ConvertFrom-Json
    if ($aggregateRecord.Summary.Total -ne 6 -or $aggregateRecord.Summary.FAIL -ne 2) { throw "Aggregation did not retain report statuses: total=$($aggregateRecord.Summary.Total), fail=$($aggregateRecord.Summary.FAIL)." }
    $quarter = Invoke-XytReport -Operation Aggregate -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -InputRoot $temp -OutputRoot $temp -Period quarter -At '2026-09-30T12:00:00Z'
    $year = Invoke-XytReport -Operation Aggregate -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -InputRoot $temp -OutputRoot $temp -Period year -At '2026-12-31T12:00:00Z'
    if (-not (Test-Path $quarter.JsonPath) -or -not (Test-Path $year.JsonPath)) { throw 'Quarter or year aggregate was not created.' }
    $upload = Invoke-XytReport -Operation Upload -RepositoryRoot (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) -ReportPath $report.JsonPath -DryRun
    if ($upload.Status -ne 'PASS' -or $upload.Mode -ne 'DRY_RUN') { throw 'Upload DryRun did not pass.' }
    'XYT REPORT MODULE SELFTEST: PASS'
}
finally { Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue }
