$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
function Run([string]$file, [string[]]$parameters) {
    & $file @parameters
    if ($LASTEXITCODE -ne 0) { throw "failed: $file" }
}
Run "dotnet" @("restore", "XuanYu.Engine.slnx", "--disable-parallel")
Run "dotnet" @("build", "XuanYu.Engine.slnx", "--no-restore", "-m:1", "-nr:false", "-p:BuildInParallel=false", "-p:UseSharedCompilation=false")
Run "dotnet" @("test", "XuanYu.World.Tests/XuanYu.World.Tests.csproj", "--no-restore", "-p:IsTestProject=true", "-m:1", "-nr:false", "-p:UseSharedCompilation=false", "--filter", "FullyQualifiedName~ViewportInputRouter|FullyQualifiedName~ViewportGestureLifecycle|FullyQualifiedName~ViewportInputConvergenceIntegration|FullyQualifiedName~NativePointerEventAdapter|FullyQualifiedName~AvaloniaPointerEventAdapter|FullyQualifiedName~NativeSourceParity|FullyQualifiedName~UnifiedPointerReadiness", "--logger", "console;verbosity=minimal")
Run "powershell" @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", "$PSScriptRoot/test-viewport-migration-baseline.ps1")
Write-Host "WAVE-2.5 Phase-1 Input Convergence: PASS"
