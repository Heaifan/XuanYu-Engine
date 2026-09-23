# ARCH-VIEWPORT-R1: freeze Avalonia-owned Editor UI and allow only Vulkan surface interop.

$viewportUiRoot = "XuanYu.Editor.UI/Viewport"
$diagnosticRoot = "XuanYu.Editor.UI/Diagnostic"
$renderRoots = @("XuanYu.Render.Abstractions", "XuanYu.Render.Vulkan")

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

# Native child HWND creation is a platform adapter detail, not a general Viewport UI capability.
foreach ($file in Get-GuardFiles $viewportUiRoot) {
    $allowed = $file -eq "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs"
    $nativeCreation = @("CreateWindowEx", "RegisterClass", "DestroyWindow")
    if (!$allowed) { Assert-GuardNotContains $file $nativeCreation "Viewport native window creation" }
    Assert-GuardNotContains $file @(
        "new Popup", "new Window", "<Popup", "<Window", "ShowDialog", "ToolTip"
    ) "Viewport Editor UI ownership"
}

# Renderer layers may consume native render-neutral handles, but cannot create or reference Editor UI.
foreach ($root in $renderRoots) {
    foreach ($file in Get-GuardFiles $root) {
        Assert-GuardNotContains $file @(
            "using Avalonia", "Avalonia.", "using System.Windows.Forms", "System.Windows.Forms",
            "NativeControlHost", "new Popup", "new Window", "<Popup", "<Window", "ShowDialog",
            "CreateWindowEx", "RegisterClass", "user32"
        ) "Renderer UI ownership"
    }
}

# Diagnostic may observe PopupRoot/native facts through the two explicit probe files only.
foreach ($file in Get-GuardFiles $diagnosticRoot) {
    $probe = $file -in @(
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs",
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs"
    )
    if (!$probe) {
        Assert-GuardNotContains $file @(
            "CreateWindowEx", "RegisterClass", "DestroyWindow", "SetCapture", "ReleaseCapture",
            'DllImport("user32', "TryGetPlatformHandle", "Hwnd"
        ) "Diagnostic native UI ownership"
    }
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
