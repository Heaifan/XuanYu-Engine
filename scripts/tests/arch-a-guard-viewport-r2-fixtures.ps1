$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$guard = Join-Path $repo "scripts/arch-a-guard-viewport.ps1"
$fixtureRoot = Join-Path $repo ("obj/viewport-guard-" + [guid]::NewGuid())
$silentHostSource = Get-Content -LiteralPath (Join-Path $repo "XuanYu.Editor.UI/Infrastructure/Resources/Host/SilentNativeWindow.cs") -Raw -Encoding utf8

function Write-Fixture([string]$relative, [string]$content) {
    $path = Join-Path $script:fixtureRoot ($relative -replace '/', '\')
    New-Item (Split-Path $path) -ItemType Directory -Force | Out-Null
    Set-Content -LiteralPath $path -Value $content -Encoding utf8
}

function New-BaseFixture {
    New-Item $script:fixtureRoot -ItemType Directory -Force | Out-Null
    Write-Fixture "XuanYu.Editor.UI/ViewportUI/Host/Win32Host/VulkanNativeHost.cs" "class VulkanNativeHost : NativeControlHost {}"
    Write-Fixture "XuanYu.Editor.UI/ViewportUI/Host/Win32Host/Win32ViewportHost.cs" "const int WS_CHILD=1; CreateWindowEx(); RegisterClass(); DestroyWindow();"
    Write-Fixture "XuanYu.Editor.UI/Infrastructure/Diagnostics/OverlayNative/DiagnosticOverlayHost.NativeOverlay.cs" "ShouldUseOverlayLayer = false;"
    Write-Fixture "XuanYu.Editor.UI/ViewportUI/Diagnostics/DiagnosticNativeCoordinateMapping.cs" "OwnerHwnd; TopMost;"
    Write-Fixture "XuanYu.Editor.UI/ViewportUI/Diagnostics/DiagnosticNativePointerProbe.cs" "Observed pointer;"
    Write-Fixture "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj" "<Project />"
    Write-Fixture "XuanYu.Render.Abstractions/Surface.cs" "VkSurfaceKHR surface;"
    Write-Fixture "XuanYu.Render.Vulkan/Device.cs" "Swapchain Device ExternalMemoryHandle;"
    Write-Fixture "XuanYu.World.Tests/XuanYu.World.Tests.csproj" "<Project />"
}

function Invoke-FixtureGuard {
    $previousErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        & pwsh -NoProfile -File $guard -Root $script:fixtureRoot *> $null
        return $LASTEXITCODE
    } finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }
}

function Invoke-Fixture([string]$name, [string]$relative, [string]$content, [bool]$mustFail) {
    Remove-Item $script:fixtureRoot -Recurse -Force -ErrorAction SilentlyContinue
    New-BaseFixture
    if ($relative) { Write-Fixture $relative $content }
    $exitCode = Invoke-FixtureGuard
    $failed = $exitCode -ne 0
    if ($failed -ne $mustFail) { throw "$name fixture expected fail=$mustFail, got fail=$failed" }
    Write-Host "${name}: $($(if($failed){'FAIL'}else{'PASS'})) as expected"
}

try {
    Invoke-Fixture "positive baseline" "" "" $false
    Invoke-Fixture "positive silent top-level HWND" "XuanYu.Editor.UI/Infrastructure/Resources/Host/SilentNativeWindow.cs" $silentHostSource $false
    Invoke-Fixture "silent host accidentally visible" "XuanYu.Editor.UI/Infrastructure/Resources/Host/SilentNativeWindow.cs" ($silentHostSource.Replace('WsPopup,', 'WsPopup | WsVisible,')) $true
    Invoke-Fixture "unexpected second native host" "XuanYu.Editor.UI/Infrastructure/Resources/Host/OtherWindow.cs" "CreateWindowEx();" $true
    Invoke-Fixture "positive spike isolation" "XuanYu.Viewport.CompositionSpike/Spike.cs" "CompositionDrawingSurface ImportImage" $false
    Invoke-Fixture "positive test Spike reference" "XuanYu.World.Tests/XuanYu.World.Tests.csproj" "XuanYu.Viewport.CompositionSpike" $false
    Invoke-Fixture "second NativeControlHost" "XuanYu.Editor.UI/ViewportUI/Host/ExtraHost.cs" "class ExtraHost : NativeControlHost {}" $true
    Invoke-Fixture "second WS_CHILD creator" "XuanYu.Editor.UI/ViewportUI/Host/Extra.cs" "const int WS_CHILD=1; CreateWindowEx();" $true
    Invoke-Fixture "fourth native allowlist file" "XuanYu.Editor.UI/ViewportUI/Diagnostics/Extra.cs" "ShouldUseOverlayLayer = false;" $true
    Invoke-Fixture "production Spike reference" "XuanYu.Editor.UI/XuanYu.Editor.UI.csproj" "XuanYu.Viewport.CompositionSpike" $true
    Invoke-Fixture "production CPU readback" "XuanYu.Editor.UI/ViewportUI/Host/Composition.cs" "ReadPixels();" $true
    Invoke-Fixture "renderer Avalonia Window" "XuanYu.Render.Vulkan/Bad.cs" "using Avalonia.Controls; Window window;" $true
}
finally { Remove-Item $fixtureRoot -Recurse -Force -ErrorAction SilentlyContinue }
exit 0
