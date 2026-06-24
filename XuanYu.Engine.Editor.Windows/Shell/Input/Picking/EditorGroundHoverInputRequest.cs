using FluidWarfare.Editor.ViewportGround;
using FluidWarfare.Editor.Windows.Viewport.Navigation;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

namespace FluidWarfare.Editor.Windows.Shell.Input.Picking;

/// <summary>GroundHoverRoute 的请求。只携带地面悬停所需的字段。</summary>
public sealed record EditorGroundHoverInputRequest(
    int X, int Y,
    Scene3dSessionLifecycle Lifecycle,
    EditorGroundPointerState GroundPointerState,
    ViewportNavigationRoute NavigationRoute,
    Action<string> SetGroundPosition,
    Action<string> SetCurrentSelection);
