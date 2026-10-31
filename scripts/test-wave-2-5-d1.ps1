$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
function Run([string]$file, [string[]]$parameters) {
    & $file @parameters
    if ($LASTEXITCODE -ne 0) { throw "failed: $file" }
}

Run (Join-Path $PSScriptRoot 'xye-build.ps1') @('-Project','XuanYu.Engine.slnx','-Configuration','Debug')
Run (Join-Path $PSScriptRoot 'xye-dotnet.ps1') @('test','XuanYu.World.Tests/XuanYu.World.Tests.csproj','--no-build','--no-restore','-p:IsTestProject=true','--filter','FullyQualifiedName~D1Consumer')
Run "powershell" @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", "$PSScriptRoot/test-wave-2.5-input-convergence.ps1")
Write-Host "WAVE-2.5-D1 Consumer Migration Baseline: PASS"
