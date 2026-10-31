using Avalonia.Threading;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

using XuanYu.Editor.UI.XycControl;

sealed partial class UiVmXycRuntime(UiVm vm, string instanceId,
    Task<bool>? terrainLoad, SilentVulkanHost? silentHost) : IXycRuntime
{
    public string InstanceId { get; } = instanceId;
    public XycCamera InspectCamera()
    {
        EnsureTerrainLoaded();
        return OnUi(vm.ReadXycCamera);
    }

    public XycCamera SetTestPose(XycCamera pose)
    {
        EnsureTerrainLoaded();
        return OnUi(() => vm.WriteXycCamera(pose));
    }

    public XycCamera OrbitCamera(double deltaX, double deltaY)
    {
        EnsureTerrainLoaded();
        return OnUi(() =>
        {
            var viewport = vm.CurrentViewport;
            const long pointerId = long.MinValue + 31;
            var x = viewport.LogicalWidth / 2.0;
            var y = viewport.LogicalHeight / 2.0;
            if (!vm.BeginCameraNavigation(pointerId, x, y, false,
                    (int)viewport.LogicalWidth, (int)viewport.LogicalHeight))
                throw new InvalidOperationException("Editor Orbit could not begin.");
            try
            {
                if (!vm.PreviewCameraNavigation(pointerId, x + deltaX, y + deltaY) ||
                    !vm.EndCameraNavigation(pointerId))
                    throw new InvalidOperationException("Editor Orbit did not complete.");
                return vm.ReadXycCamera();
            }
            catch
            {
                vm.CancelCameraNavigation("XYC Orbit Failure");
                throw;
            }
        });
    }

    public bool ArmPresentedCapture(long cameraRevision)
    {
        if (silentHost is { IsAttached: false })
            throw new InvalidOperationException("Silent Vulkan host did not attach; capture is unavailable.");
        if (!ViewportProbe.ArmFrameCapture(cameraRevision))
            throw new InvalidOperationException("Vulkan replay capture is not enabled.");
        return true;
    }

    public bool ReleasePresentedCapture()
    {
        ViewportProbe.ReleaseCapturedFrame();
        return true;
    }

    void EnsureTerrainLoaded()
    {
        if (terrainLoad is null) return;
        if (!terrainLoad.Wait(TimeSpan.FromSeconds(120)))
            throw new TimeoutException("等待诊断 DEM 导入超时。");
        if (!terrainLoad.Result) throw new InvalidOperationException("诊断 DEM 导入失败。");
    }

    static T OnUi<T>(Func<T> action) => Dispatcher.UIThread.CheckAccess()
        ? action() : Dispatcher.UIThread.InvokeAsync(action).GetAwaiter().GetResult();
}
