function Get-GuardPath([string]$root, [string]$relative) {
    Join-Path $root ($relative -replace '/', '\')
}

function Get-GuardFiles([string]$root, [string]$relativeRoot) {
    $path = Get-GuardPath $root $relativeRoot
    if (!(Test-Path $path)) { return @() }
    Get-ChildItem $path -Recurse -File -Include *.cs,*.axaml |
        ForEach-Object { $_.FullName.Substring($root.Length + 1).Replace('\', '/') }
}

function Get-GuardProjects([string]$root) {
    Get-ChildItem $root -Recurse -File -Filter *.csproj |
        ForEach-Object { $_.FullName.Substring($root.Length + 1).Replace('\', '/') }
}

function Read-GuardText([string]$root, [string]$relative) {
    Get-Content -LiteralPath (Get-GuardPath $root $relative) -Raw -Encoding utf8
}

function Assert-GuardFiles([string]$root, [string]$relativeRoot,
    [string[]]$allowed, [string[]]$needles, [string]$label) {
    foreach ($file in Get-GuardFiles $root $relativeRoot) {
        if ($allowed -contains $file) { continue }
        $text = Read-GuardText $root $file
        foreach ($needle in $needles) {
            if ($text.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
                Add-Failure "$label forbidden: $needle ($file)"
            }
        }
    }
}

function Assert-GuardPathExists([string]$root, [string[]]$paths, [string]$label) {
    foreach ($path in $paths) {
        if (!(Test-Path (Get-GuardPath $root $path))) { Add-Failure "$label missing: $path" }
    }
}

function Invoke-ViewportBoundaryGuard([string]$root) {
    $allowlist = @(
        "XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs",
        "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs",
        "XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs")
    Assert-GuardPathExists $root $allowlist "Legacy allowlist file"

    $uiFiles = Get-GuardFiles $root "XuanYu.Editor.UI"
    foreach ($file in $uiFiles) {
        $text = Read-GuardText $root $file
        if ($text -match ":\s*NativeControlHost\b" -and
            $file -ne $allowlist[0]) { Add-Failure "Second NativeControlHost implementation: $file" }
    }
    Assert-GuardFiles $root "XuanYu.Editor.UI" @($allowlist[1]) @(
        "CreateWindowEx", "RegisterClass", "WS_CHILD", "DestroyWindow") "Viewport child HWND ownership"
    Assert-GuardFiles $root "XuanYu.Editor.UI/Viewport" @() @(
        "RenderTargetBitmap", "ReadPixels", "Readback") "Viewport CPU readback"
    Assert-GuardFiles $root "XuanYu.Editor.UI/Diagnostic" @($allowlist[2],
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs",
        "XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs") @(
        "CreateWindowEx", "RegisterClass", "DestroyWindow", "SetCapture", "ReleaseCapture",
        "SetWindowPos", "SetWindowLong", "TopMost", "OwnerHwnd", "ShouldUseOverlayLayer = false",
        "TryGetPlatformHandle", 'DllImport("user32' ) "Diagnostic native workaround"

    Assert-GuardFiles $root "XuanYu.Editor.UI" @() @(
        "CompositionDrawingSurface", "ICompositionGpuInterop", "ImportImage",
        "CompositionSurfaceVisual", "XuanYu.Viewport.CompositionSpike") "Production Composition Spike API"
    foreach ($project in Get-GuardProjects $root) {
        if ($project -match "Tests/|Spike") { continue }
        $text = Read-GuardText $root $project
        if ($text -match "XuanYu\.Viewport\.CompositionSpike|CompositionSpike") {
            Add-Failure "Production project references Composition Spike: $project"
        }
    }

    foreach ($renderRoot in @("XuanYu.Render.Abstractions", "XuanYu.Render.Vulkan")) {
        Assert-GuardFiles $root $renderRoot @() @(
            "using Avalonia", "Avalonia.", "NativeControlHost", "Popup", "Window", "Dialog",
            "CreateWindowEx", "RegisterClass", "user32") "Renderer UI ownership"
    }
}
