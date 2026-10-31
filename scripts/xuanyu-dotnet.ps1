$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'xye-dotnet.ps1') @args
exit $LASTEXITCODE
