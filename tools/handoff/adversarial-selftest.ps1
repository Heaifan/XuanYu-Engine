[CmdletBinding()]
param()
$test = Join-Path $PSScriptRoot 'handoff.selftest.ps1'
& powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $test
exit $LASTEXITCODE
