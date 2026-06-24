using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;

public sealed record PresentedNavigationOverlaySnapshot
{
    public ViewportNavigationLayout? Layout { get; init; }
    public int ViewportWidth { get; init; }
    public int ViewportHeight { get; init; }
    public int CameraRevision { get; init; }
    public int PresentedFrameIndex { get; init; }
    public int OverlayRevision { get; init; }
    public bool IsAvailable => Layout is not null && ViewportWidth > 0 && ViewportHeight > 0;
    public static PresentedNavigationOverlaySnapshot Empty { get; } = new();
}
