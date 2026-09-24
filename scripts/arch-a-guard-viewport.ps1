param([string]$Root = (Split-Path -Parent $PSScriptRoot))

$guardRoot = (Resolve-Path $Root).Path
$standalone = !(Get-Command Add-Failure -ErrorAction SilentlyContinue)
if ($standalone) {
    $failures = New-Object System.Collections.Generic.List[string]
    function Add-Failure([string]$message) { $failures.Add($message) }
}

. (Join-Path $PSScriptRoot "arch-a-guard-viewport-helpers.ps1")
Invoke-ViewportBoundaryGuard $guardRoot

if ($standalone -and $failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}
if ($standalone) { Write-Host "ARCH-VIEWPORT-R2 guard passed." }
