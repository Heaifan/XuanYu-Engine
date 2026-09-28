param([string]$Root = (Split-Path -Parent $PSScriptRoot))

$bootstrap = Join-Path $PSScriptRoot "architecture/guard-bootstrap.ps1"
. $bootstrap

$guardRoot = (Resolve-Path $Root).Path
$context = Initialize-GuardBootstrap -Root $guardRoot

. (Join-Path $PSScriptRoot "arch-a-guard-viewport-helpers.ps1")
Invoke-ViewportBoundaryGuard $guardRoot

Complete-GuardRun "ARCH-VIEWPORT-R2 guard completed." -Child
