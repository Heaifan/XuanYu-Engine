using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Selection;
using FluidWarfare.Editor.ViewportGround;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Shell.Input.Picking;
using FluidWarfare.Editor.Windows.Viewport.Picking;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Submit;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Selection.Screen;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Picking;

/// <summary>Picking 路由。负责视口点击拾取、地面标记控制及拾取诊断。</summary>
sealed class EditorShellPickingRoute(
    EditorPickInputRoute pickInputRoute,
    Scene3dSessionLifecycle lifecycle,
    ViewportPointerPickRoute viewportPickRoute,
    ViewportRenderSceneStore renderSceneStore,
    EditorSelectionRoute selectionRoute,
    EditorGroundPlacementState groundPlacementState,
    EditorGroundPointerState groundPointerState,
    VulkanViewportHostPanel? vhPanel,
    Action<string> appendInfoLog,
    Action<string> setStatusBarSelection,
    Action refreshDiagnostics,
    Action<Vector3d> completeGroundPlacement,
    Action<VulkanScene3dFrameReason> scheduleFrame)
{
    /// <summary>视口点击 Picking 处理。像素坐标 → 世界射线 → RenderScene Picker → 统一选择入口。</summary>
    public void HandleViewportPick(int pixelX, int pixelY,
        Action<string?, EditorEntitySelectionOrigin> applyEntitySelection)
    {
        var r = pickInputRoute.Pick(pixelX, pixelY, lifecycle, viewportPickRoute, renderSceneStore,
            selectionRoute, groundPlacementState, groundPointerState,
            applyEntitySelection, appendInfoLog,
            setStatusBarSelection,
            refreshDiagnostics, ShowGroundCursor, HideGroundCursor, completeGroundPlacement,
            scheduleFrame);
        if (!r.SelectionChanged)
        {
            var snap = lifecycle.State.Session?.LastPresentedSnapshot;
            if (snap?.IsValid == true && vhPanel is not null)
            {
                var nh = vhPanel.GetNativeHostInfo();
                if (nh.HasNativeHandle)
                {
                    var rb = RayBuilder.Build(new(pixelX, pixelY, snap,
                        lifecycle.State.FrameRoute?.Snapshots.PresentedPick
                            ?? PresentedScenePickSnapshot.None,
                        renderSceneStore.Current, SceneGroundPlane.Default));
                    if (rb is not null)
                        ViewportPickTrace.Write(pixelX, pixelY, snap, rb, renderSceneStore.Current);
                }
            }
        }
    }

    public void ShowGroundCursor(Vector3d worldPosition)
    {
        groundPointerState.Commit(worldPosition);
        if (lifecycle.State.Session is not null
            && lifecycle.State.Session.SetGroundCursor(worldPosition))
        {
            scheduleFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }

    public void HideGroundCursor()
    {
        groundPointerState.ClearCommit();
        if (lifecycle.State.Session is not null)
        {
            lifecycle.State.Session.SetGroundCursor(null);
            scheduleFrame(VulkanScene3dFrameReason.GroundCursorChanged);
        }
    }
}
