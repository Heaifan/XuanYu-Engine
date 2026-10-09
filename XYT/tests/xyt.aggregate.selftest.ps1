$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $root 'xyt.ps1'
$outputRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('xyt-aggregate-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $outputRoot | Out-Null
try {
    $savedPreference = $ErrorActionPreference; $ErrorActionPreference = 'Continue'
    $missing = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'UNIT' -AffectedCapability 'XYT' -Status PASS -OutputRoot $outputRoot 2>&1
    $missingCode = $LASTEXITCODE; $ErrorActionPreference = $savedPreference
    if ($missingCode -eq 0) { throw 'Missing TestSetVersion unexpectedly succeeded.' }
    if (($missing -join "`n") -notlike '*TestSetVersion is required*') { throw 'Missing TestSetVersion failure was not preserved.' }
    $one = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status PASS -Timestamp '2026-09-03T10:00:00Z' -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($one -join "`n") }
    $two = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'HEADLESS' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status FAIL -Timestamp '2026-09-30T10:00:00Z' -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($two -join "`n") }
    $three = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status BLOCKED -Timestamp '2026-10-01T10:00:00Z' -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($three -join "`n") }
    $four = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry report -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -Status PASS -Timestamp '2026-12-31T10:00:00Z' -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($four -join "`n") }
    $aggregate = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry aggregate -Period month -At '2026-09-30T12:00:00Z' -InputRoot $outputRoot -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($aggregate -join "`n") }
    $json = Get-ChildItem $outputRoot -Filter '*-month.json' -Recurse | Select-Object -First 1
    if ($null -eq $json) { throw 'Monthly aggregate JSON was not created.' }
    $document = Get-Content $json.FullName -Raw | ConvertFrom-Json
    if ($document.Summary.Total -ne 2) { throw "Expected 2 reports, got $($document.Summary.Total)." }
    if ($document.Summary.Fail -ne 1) { throw 'FAIL status was not retained in aggregate.' }
    & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry aggregate -Period quarter -At '2026-09-30T12:00:00Z' -InputRoot $outputRoot -OutputRoot $outputRoot | Out-Null
    & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry aggregate -Period year -At '2026-12-31T12:00:00Z' -InputRoot $outputRoot -OutputRoot $outputRoot | Out-Null
    $quarterPath = (Get-ChildItem $outputRoot -Filter '*-quarter.json' -Recurse | Select-Object -First 1).FullName
    $quarter = Get-Content $quarterPath -Raw | ConvertFrom-Json
    if ($quarter.Summary.Total -ne 2) { throw 'Quarter boundary included an adjacent-quarter report.' }
    $yearPath = (Get-ChildItem $outputRoot -Filter '*-year.json' -Recurse | Select-Object -First 1).FullName
    $year = Get-Content $yearPath -Raw | ConvertFrom-Json
    if ($year.Summary.Total -ne 4) { throw 'Year aggregate did not include all 2026 reports.' }
    'XYT AGGREGATE SELFTEST PASS'
}
finally { Remove-Item -LiteralPath $outputRoot -Recurse -Force -ErrorAction SilentlyContinue }
