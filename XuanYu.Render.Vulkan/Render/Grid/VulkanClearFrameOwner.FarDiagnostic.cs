using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    long _lastFarDiagnosticRevision = long.MinValue;

    void TraceFarProjection(RenderProjection projection, ViewportState viewport,
        bool metricValid, ViewportMetricScale metric)
    {
        var camera = projection.Camera;
        if (camera.Revision == _lastFarDiagnosticRevision) return;
        _lastFarDiagnosticRevision = camera.Revision;
        var ray = WorldRayFactory.FromViewportPoint(camera.ToViewProjection(viewport),
            viewport.LogicalX + (viewport.LogicalWidth * 0.5),
            viewport.LogicalY + (viewport.LogicalHeight * 0.5));
        var t = System.Math.Abs(ray.Direction.Z) < 0.001 ? double.NaN : -ray.Origin.Z / ray.Direction.Z;
        var ratio = t / camera.FarPlane;
        var distance = camera.Position.DistanceTo(projection.CameraTarget);
        var metricText = metricValid
            ? $"有效 x={metric.MetersPerDipX:g6},y={metric.MetersPerDipY:g6}"
            : "无效（沿用上一帧 Step）";
        System.Diagnostics.Debug.WriteLine($"[F1-FAR-DIAG-01] rev={camera.Revision}; pos={camera.Position}; target={projection.CameraTarget}; " +
            $"distance={distance:g6}; near={camera.NearPlane:g6}; far={camera.FarPlane:g6}; " +
            $"metric={metricText}; step={_referenceGridFrameState.StepMeters:g6}; " +
            $"centerRay=({ray.Origin} -> {ray.Direction}); t={t:g6}; t/far={ratio:g6}; " +
            $"gridLimit=far; axisLimit=far*0.75");
    }
}
