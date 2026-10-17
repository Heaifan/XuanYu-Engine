using XuanYu.Core.Diagnostics;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

sealed partial class UiVmXycRuntime
{
    public XycRuntimeInfo InspectRuntime() => OnUi(() =>
    {
        var host = silentHost?.Inspect();
        var capability = host is null ? "windowed-editor-instance; offscreen=NOT-IMPLEMENTED"
            : host.AttachFailure is null
                ? "HIDDEN-HWND-SWAPCHAIN-PRESENT; attach=SUCCESS"
                : $"HIDDEN-HWND-SWAPCHAIN-PRESENT; attach=FAILED; reason={host.AttachFailure}";
        return new XycRuntimeInfo("XYC-API/1", InstanceId,
            silentHost is null ? "windowed-editor-instance" : "silent-hidden-hwnd",
            capability, ViewportProbeIdentity.Current(),
            Environment.GetEnvironmentVariable("XYC_BRANCH") ?? "UNKNOWN",
            Environment.GetEnvironmentVariable("XYE_PROBE_SOURCE_DIRTY") ?? "UNKNOWN",
            Environment.ProcessId, ViewportProbe.CurrentFrameId, host);
    });

    public XycScene InspectScene()
    {
        EnsureTerrainLoaded();
        return OnUi(() =>
        {
            var terrain = vm.RenderProjection.Projection.TerrainResources.FirstOrDefault();
            var viewport = vm.CurrentViewport;
            var presented = ViewportProbe.LastPresented;
            return new XycScene(vm.SceneId, vm.SceneRevision, vm.TerrainStatus,
                vm.SceneEntityCount, terrain?.TerrainId, terrain?.Revision,
                vm.ScaleIndicatorText, ViewportProbe.CurrentFrameId,
                viewport.PhysicalWidth, viewport.PhysicalHeight,
                presented?.FrameId, presented?.CameraRevision, presented?.ImageIndex,
                presented?.CaptureHeld, silentHost?.Inspect());
        });
    }

    public SilentHostDiagnostics? InspectSilentHost() => OnUi(() => silentHost?.Inspect());
}
