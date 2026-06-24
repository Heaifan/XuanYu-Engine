using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Selection;
using XuanYu.Engine.Editor.ViewportGround;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Scene.Position;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Input.Picking;

/// <summary>视口点击 Picking 路由。执行射线-场景求交，决策选择/地面标记/放置模式。</summary>
public sealed class EditorPickInputRoute
{
    public EditorPickInputResult Pick(
        int x, int y,
        Scene3dSessionLifecycle lifecycle,
        ViewportPointerPickRoute pickRoute,
        ViewportRenderSceneStore renderScene,
        EditorSelectionRoute selection,
        EditorGroundPlacementState placement,
        EditorGroundPointerState groundPointer,
        Action<string?, EditorEntitySelectionOrigin> applySelection,
        Action<string> infoLog,
        Action<string> setStatusBar,
        Action refreshDiagnostics,
        Action<Vector3d> showGroundCursor,
        Action hideGroundCursor,
        Action<Vector3d> completePlacement,
        Action<VulkanScene3dFrameReason> scheduleFrame)
    {
        var s = lifecycle.State.Session;
        if (s is null || s.Status != VulkanScene3dSessionStatus.Active) return new(false, false, false);

        var snap = s.LastPresentedSnapshot;
        if (!snap.IsValid) return new(false, false, false);

        var pick = lifecycle.State.FrameRoute?.Snapshots.PresentedPick ?? PresentedScenePickSnapshot.None;
        var req = new ViewportPickRequest(x, y, snap, pick, renderScene.Current, SceneGroundPlane.Default);
        var result = pickRoute.Pick(req);

        if (placement.IsActive)
        {
            if (result.Kind == ViewportPickKind.Ground && result.GroundPosition is not null)
            { completePlacement(result.GroundPosition.Value); hideGroundCursor(); return new(false, false, true); }
            if (result.Kind == ViewportPickKind.Entity)
            { setStatusBar("请点击空白地面完成放置"); return new(false, false, false); }
            setStatusBar("当前位置未命中地面，请调整相机或点击其他区域");
            return new(false, false, false);
        }

        switch (result.Kind)
        {
            case ViewportPickKind.Entity when result.EntityId is not null:
                applySelection(result.EntityId.Value.Value.ToString(), EditorEntitySelectionOrigin.ViewportPicking);
                hideGroundCursor();
                infoLog($"已选择实体 {result.EntityId.Value.Value}");
                break;
            case ViewportPickKind.Ground when result.GroundPosition is not null:
                applySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                showGroundCursor(result.GroundPosition.Value);
                infoLog($"地面落点：X {result.GroundPosition.Value.X:F2}，Y {result.GroundPosition.Value.Y:F2}，Z {result.GroundPosition.Value.Z:F2}。");
                break;
            default:
                applySelection(null, EditorEntitySelectionOrigin.ViewportPicking);
                hideGroundCursor();
                break;
        }
        refreshDiagnostics();
        return new(result.Kind == ViewportPickKind.Entity, result.Kind == ViewportPickKind.Ground, false);
    }
}
