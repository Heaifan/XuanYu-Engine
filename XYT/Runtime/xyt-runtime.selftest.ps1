$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
& (Join-Path $root 'XYT\Runtime\test_xyt-runtime.ps1')
exit $LASTEXITCODE
