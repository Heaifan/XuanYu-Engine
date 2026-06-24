using Avalonia.Threading;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.World;

namespace FluidWarfare.Editor.Windows.Shell.Viewport;

/// <summary>视口 Frame Selected / 聚焦所选路由。负责实体聚焦相机命令的构建与执行。</summary>
sealed class EditorShellViewportFrameRoute(
    Func<bool> isSessionActive,
    Scene3dSessionLifecycle lifecycle,
    EditorSelectionRoute selectionRoute,
    Func<WorldState?> getWorldState,
    StatusBarPanel? statusBarPanel,
    ViewportCameraRoute cameraRoute,
    Action<VulkanScene3dFrameReason> scheduleFrame)
{
    bool _frameSelectedPending;

    public void ExecuteViewportFrameSelected()
    {
        if (_frameSelectedPending) return;
        _frameSelectedPending = true;
        try
        {
            if (!isSessionActive() || lifecycle.State.Session is null) return;
            if (selectionRoute.State.SelectedWorldEntity is null)
            {
                statusBarPanel?.SetCurrentSelection("没有可聚焦的世界实体。");
                return;
            }
            var target = ViewportCameraFocusTarget.Compute(
                selectionRoute.State.SelectedWorldEntity.EntityId, getWorldState()!);
            if (target is null) return;
            var (cx, cy, cz, r) = target.Value;
            var result = cameraRoute.Apply(new ViewportCameraCommand.FrameSelected(cx, cy, cz, r));
            statusBarPanel?.SetCurrentSelection(
                $"已聚焦实体 {selectionRoute.State.SelectedWorldEntity.DisplayName}。");
            if (result.NeedsFrame) scheduleFrame(result.Reason);
        }
        finally { Dispatcher.UIThread.Post(() => _frameSelectedPending = false); }
    }
}
