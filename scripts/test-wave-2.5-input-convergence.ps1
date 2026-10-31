$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
function Run([string]$file, [string[]]$parameters) {
    & $file @parameters
    if ($LASTEXITCODE -ne 0) { throw "failed: $file" }
}
Run (Join-Path $PSScriptRoot 'xye-build.ps1') @('-Project','XuanYu.Engine.slnx','-Configuration','Debug')
Run (Join-Path $PSScriptRoot 'xye-dotnet.ps1') @('test','XuanYu.World.Tests/XuanYu.World.Tests.csproj','--no-build','--no-restore','-p:IsTestProject=true','--filter','FullyQualifiedName~ViewportInputRouter|FullyQualifiedName~ViewportGestureLifecycle|FullyQualifiedName~ViewportInputConvergenceIntegration|FullyQualifiedName~NativePointerEventAdapter|FullyQualifiedName~AvaloniaPointerEventAdapter|FullyQualifiedName~NativeSourceParity|FullyQualifiedName~UnifiedPointerReadiness','--logger','console;verbosity=minimal')
Run "powershell" @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", "$PSScriptRoot/test-viewport-migration-baseline.ps1")
Write-Host "WAVE-2.5 Phase-1 Input Convergence: PASS"
