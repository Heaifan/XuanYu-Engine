$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $root 'xyt.ps1'
$outputRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('xyt-upload-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $outputRoot | Out-Null
try {
    $savedPreference = $ErrorActionPreference; $ErrorActionPreference = 'Continue'
    $missing = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry upload -ReportPath (Join-Path $outputRoot 'missing.json') -DryRun 2>&1
    $missingCode = $LASTEXITCODE; $ErrorActionPreference = $savedPreference
    if ($missingCode -eq 0) { throw 'Missing report upload unexpectedly succeeded.' }
    if (($missing -join "`n") -notlike '*REPORT UPLOAD BLOCKED*') { throw 'Missing report was not reported as blocked.' }
    $report = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status PASS -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($report -join "`n") }
    $path = Get-ChildItem $outputRoot -Filter '*.json' -Recurse | Select-Object -First 1
    $dry = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry upload -ReportPath $path.FullName -DryRun 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($dry -join "`n") }
    if (($dry -join "`n") -notlike '*UPLOAD DRY RUN: PASS*') { throw 'Upload dry-run did not pass.' }
    & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry aggregate -Period month -At '2026-09-30T12:00:00Z' -InputRoot $outputRoot -OutputRoot $outputRoot | Out-Null
    $aggregate = Get-ChildItem $outputRoot -Filter '*-month.json' -Recurse | Select-Object -First 1
    $withAggregate = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry upload -ReportPath $path.FullName -AggregatePath $aggregate.FullName -DryRun 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($withAggregate -join "`n") }
    'XYT UPLOAD SELFTEST PASS'
}
finally { Remove-Item -LiteralPath $outputRoot -Recurse -Force -ErrorAction SilentlyContinue }
