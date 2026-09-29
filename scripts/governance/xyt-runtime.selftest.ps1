$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot); & (Join-Path $root 'XYT\Runtime\xyt-runtime.selftest.ps1'); exit $LASTEXITCODE
