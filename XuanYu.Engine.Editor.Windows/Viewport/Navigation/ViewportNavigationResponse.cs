using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Editor.Windows.Viewport.Navigation;

/// <summary>Overlay Pointer Press 的结果。</summary>
public sealed record ViewportNavigationPressResponse(
    ViewportNavigationPressResult Result,
    ViewportNavigationElement Element,
    ViewportCameraCommand? CameraCommand,
    string? LogMessage);

/// <summary>Overlay Pointer Move 的结果。</summary>
public sealed record ViewportNavigationMoveResponse(
    bool Handled,
    bool NeedsFrame,
    bool VisualStateChanged,
    ViewportNavigationElement Hovered,
    ViewportNavigationElement Active);

/// <summary>Overlay Pointer Release / Capture Lost 的结果。</summary>
public sealed record ViewportNavigationReleaseResponse(
    bool NeedsCleanupFrame);
