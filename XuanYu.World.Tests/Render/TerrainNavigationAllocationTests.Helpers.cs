using System.Diagnostics;
using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.World.Tests.Render;

public sealed partial class TerrainNavigationAllocationTests
{
    static Measurement MeasureCurrent(ViewProjectionState state, IReadOnlyList<SpatialAabb> chunks)
    {
        NavigateCurrent(state, chunks);
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        var watch = Stopwatch.StartNew();
        var visible = NavigateCurrent(state, chunks);
        watch.Stop();
        return new(visible, GC.GetAllocatedBytesForCurrentThread() - allocated, watch.Elapsed.TotalMilliseconds);
    }

    static Measurement MeasureLegacy(ViewProjectionState state, IReadOnlyList<SpatialAabb> chunks)
    {
        NavigateLegacy(state, chunks);
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        var allocated = GC.GetAllocatedBytesForCurrentThread();
        var watch = Stopwatch.StartNew();
        var visible = NavigateLegacy(state, chunks);
        watch.Stop();
        return new(visible, GC.GetAllocatedBytesForCurrentThread() - allocated, watch.Elapsed.TotalMilliseconds);
    }

    static int NavigateCurrent(ViewProjectionState state, IReadOnlyList<SpatialAabb> chunks)
    {
        var visible = 0;
        for (var frame = 0; frame < 60; frame++) foreach (var chunk in chunks)
            if (TerrainFrustumCuller.Intersects(state, chunk))
            { visible++; _ = TerrainLodSelector.Select(state, chunk); }
        return visible;
    }

    static int NavigateLegacy(ViewProjectionState state, IReadOnlyList<SpatialAabb> chunks)
    {
        var visible = 0;
        for (var frame = 0; frame < 60; frame++) foreach (var chunk in chunks)
            if (LegacyIntersects(state, chunk))
            { visible++; _ = LegacyProjectedPixels(state, chunk); }
        return visible;
    }

    static bool LegacyIntersects(ViewProjectionState state, SpatialAabb box)
    {
        var corners = LegacyCorners(box);
        for (var plane = 0; plane < 6; plane++)
            if (corners.All(corner => LegacyOutside(LegacyTransform(state, corner), plane))) return false;
        return true;
    }

    static double LegacyProjectedPixels(ViewProjectionState state, SpatialAabb bounds)
    {
        var points = LegacyCorners(bounds).Select(corner => LegacyTransform(state, corner))
            .Where(point => point.W > 0.0f).ToArray();
        if (points.Length == 0) return 0.0;
        var xs = points.Select(point => (point.X / point.W + 1.0) * 0.5 * state.Viewport.PhysicalWidth);
        var ys = points.Select(point => (point.Y / point.W + 1.0) * 0.5 * state.Viewport.PhysicalHeight);
        return global::System.Math.Max(xs.Max() - xs.Min(), ys.Max() - ys.Min());
    }

    static Vector4 LegacyTransform(ViewProjectionState state, Vector3d point) => Vector4.Transform(
        new Vector4((float)(point.X - state.RenderOrigin.X), (float)(point.Y - state.RenderOrigin.Y),
            (float)(point.Z - state.RenderOrigin.Z), 1), state.ViewProjection);

    static Vector3d[] LegacyCorners(SpatialAabb box) =>
    [
        new(box.Min.X, box.Min.Y, box.Min.Z), new(box.Max.X, box.Min.Y, box.Min.Z),
        new(box.Min.X, box.Max.Y, box.Min.Z), new(box.Max.X, box.Max.Y, box.Min.Z),
        new(box.Min.X, box.Min.Y, box.Max.Z), new(box.Max.X, box.Min.Y, box.Max.Z),
        new(box.Min.X, box.Max.Y, box.Max.Z), new(box.Max.X, box.Max.Y, box.Max.Z)
    ];

    static bool LegacyOutside(Vector4 point, int plane) => plane switch
    { 0 => point.X < -point.W, 1 => point.X > point.W, 2 => point.Y < -point.W,
      3 => point.Y > point.W, 4 => point.Z < 0, _ => point.Z > point.W };

    static IReadOnlyList<SpatialAabb> Chunks(int count) => Enumerable.Range(0, count)
        .Select(index => new SpatialAabb(new(index % 15 * 240, index / 15 * 240, 0),
            new(index % 15 * 240 + 240, index / 15 * 240 + 240, 100))).ToArray();

    static ViewProjectionState ViewState() => ViewProjectionState.Create(
        new CameraState(new(1800, 1800, -2500), new(0, 0, 1), Vector3d.UnitY,
            60, 1, 5000, 0, ProjectionMode.Perspective, 0),
        new ViewportState(0, 0, 800, 600, 800, 600, 1, 0));

    readonly record struct Measurement(int Visible, long Allocated, double CpuMs);
}
