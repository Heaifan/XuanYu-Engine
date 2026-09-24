$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$guard = Join-Path $repo "scripts/arch-a-guard-viewport.ps1"
$fixtureRoot = Join-Path ([IO.Path]::GetTempPath()) ("viewport-guard-" + [guid]::NewGuid())

function Write-Fixture([string]$relative, [string]$content) {
    $path = Join-Path $script:fixtureRoot ($relative -replace '/', '\')
    New-Item (Split-Path $path) -ItemType Directory -Force | Out-Null
    Set-Content -LiteralPath $path -Value $content -Encoding utf8
}

function New-BaseFixture {
    New-Item $script:fixtureRoot -ItemType Directory -Force | Out-Null
    Write-Fixture "XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs" "class VulkanNativeHost : NativeControlHost {}"
    Write-Fixture "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs" "const int WS_CHILD=1; CreateWindowEx(); RegisterClass(); DestroyWindow();"
    Write-Fixture "XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs" "ShouldUseOverlayLayer = false;"
    Write-Fixture "XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs" "OwnerHwnd; TopMost;"
    Write-Fixture "XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs" "Observed pointer;"
    Write-Fixture "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj" "<Project />"
    Write-Fixture "XuanYu.Render.Abstractions/Surface.cs" "VkSurfaceKHR surface;"
    Write-Fixture "XuanYu.Render.Vulkan/Device.cs" "Swapchain Device ExternalMemoryHandle;"
    Write-Fixture "XuanYu.World.Tests/XuanYu.World.Tests.csproj" "<Project />"
}

function Invoke-Fixture([string]$name, [string]$relative, [string]$content, [bool]$mustFail) {
    Remove-Item $script:fixtureRoot -Recurse -Force -ErrorAction SilentlyContinue
    New-BaseFixture
    if ($relative) { Write-Fixture $relative $content }
    & pwsh -NoProfile -File $guard -Root $script:fixtureRoot *> $null
    $failed = $LASTEXITCODE -ne 0
    if ($failed -ne $mustFail) { throw "$name fixture expected fail=$mustFail, got fail=$failed" }
    Write-Host "${name}: $($(if($failed){'FAIL'}else{'PASS'})) as expected"
}

try {
    Invoke-Fixture "positive baseline" "" "" $false
    Invoke-Fixture "positive spike isolation" "XuanYu.Viewport.CompositionSpike/Spike.cs" "CompositionDrawingSurface ImportImage" $false
    Invoke-Fixture "positive test Spike reference" "XuanYu.World.Tests/XuanYu.World.Tests.csproj" "XuanYu.Viewport.CompositionSpike" $false
    Invoke-Fixture "second NativeControlHost" "XuanYu.Editor.UI/Viewport/ExtraHost.cs" "class ExtraHost : NativeControlHost {}" $true
    Invoke-Fixture "second WS_CHILD creator" "XuanYu.Editor.UI/Viewport/Extra.cs" "const int WS_CHILD=1; CreateWindowEx();" $true
    Invoke-Fixture "fourth native allowlist file" "XuanYu.Editor.UI/Diagnostic/Extra.cs" "ShouldUseOverlayLayer = false;" $true
    Invoke-Fixture "production Spike reference" "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj" "XuanYu.Viewport.CompositionSpike" $true
    Invoke-Fixture "production CPU readback" "XuanYu.Editor.UI/Viewport/Composition.cs" "ReadPixels();" $true
    Invoke-Fixture "renderer Avalonia Window" "XuanYu.Render.Vulkan/Bad.cs" "using Avalonia.Controls; Window window;" $true
}
finally { Remove-Item $fixtureRoot -Recurse -Force -ErrorAction SilentlyContinue }
exit 0
