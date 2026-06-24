using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Overlay + Gizmo 相关字段。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private Overlay.VulkanOverlayResources? _overlayResources;
    private ViewportNavigation.ViewportNavigationElement _overlayHovered
        = ViewportNavigation.ViewportNavigationElement.None;
    private ViewportNavigation.ViewportNavigationElement _overlayActive
        = ViewportNavigation.ViewportNavigationElement.None;
    private Render.ViewportNavigation.ViewportNavigationLayout? _pendingOverlayLayout;
    private Overlay.PresentedNavigationOverlaySnapshot _lastPresentedOverlaySnapshot =
        Overlay.PresentedNavigationOverlaySnapshot.Empty;
    private int _overlayRevision;
    private int _lastOverlayVertexCount;
    private Render.ViewportNavigation.ViewportNavigationLayout _lastOverlayLayout = null!;
    private Overlay.VulkanOverlayVertex[]? _pendingGizmoVerts;
}
