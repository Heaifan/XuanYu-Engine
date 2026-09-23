# ARCH-VIEWPORT-R1: freeze Avalonia-owned Editor UI and block legacy-route expansion.

$viewportUiRoot = "XuanYu.Editor.UI/Viewport"
$diagnosticRoot = "XuanYu.Editor.UI/Diagnostic"
$renderRoots = @("XuanYu.Render.Abstractions", "XuanYu.Render.Vulkan")
$temporaryLegacyAllowlist = @(
    "XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs",
    "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs",
    "XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs"
)

function Get-GuardFiles([string]$root) {
    @($(git ls-files "$root/*") + $(git ls-files --others --exclude-standard "$root/*")) |
        Where-Object { $_ -match '\.(cs|axaml)$' -and (Test-Path -LiteralPath $_) } |
        Sort-Object -Unique
}

function Get-GuardText([string]$path) { Get-Content -LiteralPath $path -Raw -Encoding utf8 }

function Assert-GuardNotContains([string]$path, [string[]]$needles, [string]$label) {
    $text = Get-GuardText $path
    foreach ($needle in $needles) {
        if ($text.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            Add-Failure "$label forbidden: $needle ($path)"
        }
    }
}

# NativeControlHost and child HWND creation are exact temporary allowlist entries.
foreach ($file in Get-GuardFiles $viewportUiRoot) {
    $allowedCreator = $file -eq "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs"
    $allowedHost = $file -eq "XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs"
    if (!$allowedCreator) {
        Assert-GuardNotContains $file @("CreateWindowEx", "RegisterClass", "DestroyWindow") "Viewport child HWND creator"
    }
    if (!$allowedHost) {
        Assert-GuardNotContains $file @("NativeControlHost") "Viewport NativeControlHost implementation"
    }
    Assert-GuardNotContains $file @(
        "new Popup", "new Window", "<Popup", "<Window", "ShowDialog", "ToolTip"
    ) "Viewport Editor UI ownership"
}

# Renderer layers may consume render-neutral handles and Vulkan interop, but cannot own Editor UI.
foreach ($root in $renderRoots) {
    foreach ($file in Get-GuardFiles $root) {
        Assert-GuardNotContains $file @(
            "using Avalonia", "Avalonia.", "using System.Windows.Forms", "System.Windows.Forms",
            "NativeControlHost", "new Popup", "new Window", "<Popup", "<Window", "ShowDialog",
            "CreateWindowEx", "RegisterClass", "user32"
        ) "Renderer UI ownership"
    }
}

# Diagnostic native inspection is migration-only. The old native overlay is one exact allowlist entry.
foreach ($file in Get-GuardFiles $diagnosticRoot) {
    $probe = $file -in @(
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs",
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs"
    )
    $legacyOverlay = $file -eq "XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs"
    if (!$probe) {
        $nativeUi = @(
            "CreateWindowEx", "RegisterClass", "DestroyWindow", "SetCapture", "ReleaseCapture",
            'DllImport("user32', "TryGetPlatformHandle", "Hwnd", "SetWindowPos", "SetWindowLong",
            "TopMost", "OwnerHwnd", "ShouldUseOverlayLayer = false"
        )
        if ($legacyOverlay) {
            $nativeUi = $nativeUi | Where-Object { $_ -ne "ShouldUseOverlayLayer = false" }
        }
        Assert-GuardNotContains $file $nativeUi "Diagnostic native UI ownership"
    }
}

foreach ($file in $temporaryLegacyAllowlist) {
    if (!(Test-Path -LiteralPath $file)) { Add-Failure "Temporary legacy allowlist file missing: $file" }
}

# The legacy WinForms skeleton is documented and disconnected; it must not become another app entry point.
$legacyProject = Get-GuardText "XuanYu.Editor.Win/XuanYu.Editor.Win.csproj"
$appProject = Get-GuardText "XuanYu.Editor.App/XuanYu.Editor.App.csproj"
if ($appProject -match "XuanYu\.Editor\.Win") {
    Add-Failure "Editor.App must not compose legacy XuanYu.Editor.Win"
}
if ($legacyProject -match '<OutputType>\s*(Exe|WinExe)\s*</OutputType>') {
    Add-Failure "Legacy XuanYu.Editor.Win must not be an executable entry point"
}

Write-Host "ARCH-VIEWPORT-R1 guard passed."
