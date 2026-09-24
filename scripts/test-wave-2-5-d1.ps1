$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
function Run([string]$file, [string[]]$parameters) {
    & $file @parameters
    if ($LASTEXITCODE -ne 0) { throw "failed: $file" }
}

Run "dotnet" @("restore", "XuanYu.Engine.slnx", "--disable-parallel")
Run "dotnet" @("build", "XuanYu.Engine.slnx", "--no-restore", "-m:1", "-nr:false", "-p:BuildInParallel=false", "-p:UseSharedCompilation=false")
Run "dotnet" @("test", "XuanYu.World.Tests/XuanYu.World.Tests.csproj", "--no-restore", "-p:IsTestProject=true", "-m:1", "-nr:false", "-p:UseSharedCompilation=false", "--filter", "FullyQualifiedName~D1Consumer")
Run "powershell" @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", "$PSScriptRoot/test-wave-2.5-input-convergence.ps1")
Write-Host "WAVE-2.5-D1 Consumer Migration Baseline: PASS"
