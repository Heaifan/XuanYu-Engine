using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;

/// <summary>V2 Picking 接线。击中实体 → 更新选择状态，不刷新 Inspector/Diagnostics。</summary>
static class EditorShellV2PickingWiring
{
    public static void Wire(EditorShellV2Context ctx)
    {
        var panel = ctx.ViewportPanel;
        if (panel is null) return;

        panel.PickRequested += (x, y) =>
        {
            var session = ctx.Lifecycle.State.Session;
            if (session?.Status != VulkanScene3dSessionStatus.Active) return;
            var snap = session.LastPresentedSnapshot;
            if (!snap.IsValid) return;

            var pick = ctx.Lifecycle.State.FrameRoute?.Snapshots.PresentedPick
                ?? PresentedScenePickSnapshot.None;
            var req = new ViewportPickRequest(x, y, snap, pick, ctx.RenderStore.Current, SceneGroundPlane.Default);
            var result = ctx.Pick.Pick(req);

            if (result.Kind == ViewportPickKind.Entity && result.EntityId is not null)
            {
                ctx.Selection.State.Select(new WorldEntityInfo(
                    result.EntityId.Value, $"Entity{result.EntityId.Value.Value}", null));
            }
            else
            {
                ctx.Selection.State.Select(null);
            }
        };
    }
}
