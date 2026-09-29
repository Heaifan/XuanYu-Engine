$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $root 'xyt.ps1'
$outputRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('xyt-test-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $outputRoot | Out-Null
try {
    $result = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry test -TestCommand 'ver > nul' -TestMode 'UNIT' -TestSetVersion 'set-1' -AffectedCapability 'XYT' -OutputRoot $outputRoot 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($result -join "`n") }
    $json = Get-ChildItem $outputRoot -Filter '*.json' -Recurse | Select-Object -First 1
    $record = Get-Content $json.FullName -Raw | ConvertFrom-Json
    if ($record.Status -ne 'PASS') { throw "Expected automatic PASS, got $($record.Status)." }
    if (@($record.Evidence).Count -lt 1) { throw 'Automatic test report did not bind raw evidence.' }
    'XYT TEST SELFTEST PASS'
}
finally { Remove-Item -LiteralPath $outputRoot -Recurse -Force -ErrorAction SilentlyContinue }
