using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using static XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction.TransformInteractionAction;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;

/// <summary>V2 MoveGizmo 接线。SceneTool press/drag/release/commit，不触发 Inspector/Diagnostics。</summary>
static class EditorShellV2SceneToolWiring
{
    public static void Wire(EditorShellV2Context ctx)
    {
        var panel = ctx.ViewportPanel;
        var schedule = ctx.FrameScheduler!;
        if (panel is null) return;

        panel.SceneToolPointerPressed += (x, y) =>
        {
            var sel = ctx.Selection.State.SelectedWorldEntity;
            if (sel is null) return ViewportSceneToolPressResult.NotHandled;

            var cam = ctx.Lifecycle.State.Session?.LastPresentedSnapshot;
            if (cam is not { IsValid: true }) return ViewportSceneToolPressResult.NotHandled;

            var gizmo = ctx.Lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo;
            if (gizmo?.IsAvailable == true)
                ctx.Pointer.UpdateGizmoHover(x, y, gizmo.Value.Layout);
            else
                ctx.Pointer.ClearGizmoHover();

            if (!ctx.Pointer.HasHoveredElement)
                return ViewportSceneToolPressResult.NotHandled;

            var snap = BuildTransformSnapshot(ctx);
            if (snap is null) return ViewportSceneToolPressResult.NotHandled;

            var req = new TransformStartRequest(TransformStartSource.GizmoHandle,
                MoveGizmoElement.ViewPlane, x, y);
            var result = ctx.Pointer.OnPointerPressed(req, snap.Value);
            return result.Action == Started
                ? ViewportSceneToolPressResult.BeginDrag
                : ViewportSceneToolPressResult.NotHandled;
        };

        panel.SceneToolPointerReleased += (_, _) =>
        {
            if (!ctx.Pointer.IsDragActive) return;

            var result = ctx.Pointer.OnPointerReleased();
            if (result.Action == Confirmed)
            {
                GizmoDragProbe.Log("V2 Gizmo Commit");
                // V2 暂不写 WorldState，仅更新渲染预览
                schedule(VulkanScene3dFrameReason.TransformPreview);
            }
        };
    }

    static TransformStartSnapshot? BuildTransformSnapshot(EditorShellV2Context ctx)
    {
        var sel = ctx.Selection.State.SelectedWorldEntity;
        if (sel is null) return null;

        var cam = ctx.Lifecycle.State.Session?.LastPresentedSnapshot;
        if (cam is not { IsValid: true }) return null;

        var gizmo = ctx.Lifecycle.State.FrameRoute?.Snapshots.PresentedGizmo
            ?? PresentedMoveGizmoSnapshot.None;

        return new(sel.EntityId, new SceneTransform(Vector3d.Zero, default, default), false, cam, gizmo);
    }
}
