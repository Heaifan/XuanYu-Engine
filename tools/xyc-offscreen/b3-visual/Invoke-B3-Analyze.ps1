param(
    [Parameter(Mandatory=$true)][string]$Bundle,
    [string]$Python = 'C:\Users\Heai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe'
)
$ErrorActionPreference = 'Stop'
$bundlePath = (Resolve-Path -LiteralPath $Bundle).Path
$script = Join-Path $PSScriptRoot 'analyze_b3.py'
& $Python $script $bundlePath
if ($LASTEXITCODE -ne 0) { throw "B3 evidence validation failed with exit code $LASTEXITCODE" }
