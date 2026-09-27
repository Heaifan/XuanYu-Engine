using System.Numerics;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public static class TerrainLodSelector
{
    public const double TargetSampleSpacingPixels = 2.0;
    public const double HysteresisRatio = 0.15;
    public const int MinLod = 0;
    public const int MaxLod = 4;

    public static TerrainLodSelection Select(ViewProjectionState state, SpatialAabb bounds, int previousLod = -1)
    {
        var projectedPixels = ProjectedPixels(state, bounds);
        var baseCellPixels = projectedPixels / 240.0;
        var candidate = Candidate(baseCellPixels);
        var lod = Stabilize(baseCellPixels, candidate, previousLod);
        return new TerrainLodSelection(lod, projectedPixels, baseCellPixels);
    }

    static int Candidate(double baseCellPixels)
    {
        for (var lod = MaxLod; lod >= MinLod; lod--)
        {
            if (baseCellPixels * (1 << lod) <= TargetSampleSpacingPixels) return lod;
        }

        return MinLod;
    }

    static int Stabilize(double basePixels, int candidate, int previous)
    {
        if (previous is < MinLod or > MaxLod || candidate == previous) return candidate;
        var limit = TargetSampleSpacingPixels;
        if (candidate > previous && basePixels * (1 << candidate) > limit * (1 - HysteresisRatio)) return previous;
        if (candidate < previous && basePixels * (1 << previous) <= limit * (1 + HysteresisRatio)) return previous;
        return candidate;
    }

    static double ProjectedPixels(ViewProjectionState state, SpatialAabb bounds)
    {
        var points = TerrainFrustumCuller.Corners(bounds)
            .Select(corner => TerrainFrustumCuller.Transform(state.ViewProjection, corner))
            .Where(point => point.W > 0.0f).ToArray();
        if (points.Length == 0) return 0.0;
        var xs = points.Select(point => (point.X / point.W + 1.0) * 0.5 * state.Viewport.PhysicalWidth);
        var ys = points.Select(point => (point.Y / point.W + 1.0) * 0.5 * state.Viewport.PhysicalHeight);
        return global::System.Math.Max(xs.Max() - xs.Min(), ys.Max() - ys.Min());
    }
}
