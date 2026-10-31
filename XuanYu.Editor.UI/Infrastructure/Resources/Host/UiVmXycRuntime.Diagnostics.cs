using XuanYu.Core.Diagnostics;
using XuanYu.Editor.UI.XycControl;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;

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
            var resources = vm.RenderProjection.Projection.TerrainResources;
            var viewport = vm.CurrentViewport;
            var presented = ViewportProbe.LastPresented;
            var camera = vm.ReadXycCamera();
            XycTerrainDiagnostics? terrainDiagnostics = null;
            if (terrain is not null)
            {
                var surface = new TerrainRenderGroundSurface(resources);
                var clearance = surface.MeasureVerticalClearance(camera.Position.X,
                    camera.Position.Y, camera.Position.Z, vm.VerticalExaggeration);
                var dx = camera.Position.X - camera.ObservationCenter.X;
                var dy = camera.Position.Y - camera.ObservationCenter.Y;
                var dz = camera.Position.Z - camera.ObservationCenter.Z;
                var chunkCount = resources.Where(resource => resource.IsAvailable).Sum(resource =>
                    TerrainChunkPartitioner.Partition(resource.Heightfield,
                        resource.TerrainId, resource.Revision).Count);
                terrainDiagnostics = new(terrain.Heightfield.Width, terrain.Heightfield.Height,
                    chunkCount, (long)(terrain.Heightfield.Width - 1) *
                        (terrain.Heightfield.Height - 1) * 2,
                    "Auto (selected per-chunk LOD is not exposed here)",
                    camera.Position, Math.Sqrt(dx * dx + dy * dy + dz * dz),
                    clearance.IsValid, clearance.QueryStatus, clearance.RenderSurfaceZ,
                    clearance.VerticalClearanceMeters, "vertical-z-at-camera-xy",
                    vm.VerticalExaggeration, camera.Revision, ViewportProbe.CurrentFrameId,
                    presented?.FrameId, presented?.CameraRevision);
            }
            return new XycScene(vm.SceneId, vm.SceneRevision, vm.TerrainStatus,
                vm.SceneEntityCount, terrain?.TerrainId, terrain?.Revision,
                vm.ScaleIndicatorText, ViewportProbe.CurrentFrameId,
                viewport.PhysicalWidth, viewport.PhysicalHeight,
                presented?.FrameId, presented?.CameraRevision, presented?.ImageIndex,
                presented?.CaptureHeld, silentHost?.Inspect(), terrainDiagnostics);
        });
    }

    public SilentHostDiagnostics? InspectSilentHost() => OnUi(() => silentHost?.Inspect());
}
