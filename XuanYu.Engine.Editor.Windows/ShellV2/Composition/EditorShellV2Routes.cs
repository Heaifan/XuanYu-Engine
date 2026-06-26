using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Navigation;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Focus;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Editor.Windows.Shell.Input.Transform;
using XuanYu.Engine.Editor.Windows.Shell.Input.Picking;
using XuanYu.Engine.Editor.Windows.Shell.Input;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>创建 EditorShellV2 所需的路由实例。不包含旧 Shell 面板引用的路由。</summary>
static class EditorShellV2Routes
{
    public static void Create(EditorShellV2Context ctx)
    {
        // ─── 核心状态 ├───────────────────────────────────
        ctx.Selection = new EditorSelectionRoute();
        ctx.Pointer = new TransformPointerRoute();
        ctx.SceneToolInput = new EditorSceneToolInputRoute();
        ctx.TransformApply = new EditorTransformApplyRoute();

        // ─── 视口导航 ────────────────────────────────────
        ctx.Navigation = new ViewportNavigationRoute();
        ctx.Camera = new ViewportCameraRoute();
        ctx.Focus = new ViewportFocusSelectionRoute();

        // ─── Picking ─────────────────────────────────────
        ctx.Pick = new ViewportPointerPickRoute();
        ctx.PickInput = new EditorPickInputRoute();
        ctx.GroundHover = new EditorGroundHoverInputRoute();
        ctx.GroundPlacement = new EditorGroundPlacementRoute();

        // ─── 输入 ├───────────────────────────────────────
        ctx.ViewportInput = new EditorViewportInputRoute();
    }
}
