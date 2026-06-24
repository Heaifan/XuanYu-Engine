using XuanYu.Engine.Editor.ViewportGround;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;

namespace XuanYu.Engine.Editor.Windows.Shell.Input.Picking;

/// <summary>GroundHoverRoute 的请求。只携带地面悬停所需的字段。</summary>
public sealed record EditorGroundHoverInputRequest(
    int X, int Y,
    Scene3dSessionLifecycle Lifecycle,
    EditorGroundPointerState GroundPointerState,
    ViewportNavigationRoute NavigationRoute,
    Action<string> SetGroundPosition,
    Action<string> SetCurrentSelection);
