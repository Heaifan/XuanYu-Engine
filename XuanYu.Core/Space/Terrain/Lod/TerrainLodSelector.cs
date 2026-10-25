using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public static class TerrainLodSelector
{
    public const double TargetSampleSpacingPixels = 2.0;
    public const double HysteresisRatio = 0.15;
    public const int MinLod = 0;
    public const int MaxLod = 4;

    public static TerrainLodSelection Select(ViewProjectionState state, SpatialAabb bounds, int previousLod = -1)
        => Select(state, bounds, 240, 240, previousLod);

    public static TerrainLodSelection Select(ViewProjectionState state, SpatialAabb bounds,
        int cellCountX, int cellCountY, int previousLod = -1)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (cellCountX <= 0 || cellCountY <= 0)
            throw new ArgumentOutOfRangeException(nameof(cellCountX));
        var projectedPixels = ProjectedPixels(state, bounds);
        if (CrossesNearPlane(state, bounds))
            return new TerrainLodSelection(MinLod, projectedPixels,
                projectedPixels / global::System.Math.Max(cellCountX, cellCountY));
        var baseCellPixels = projectedPixels /
            global::System.Math.Max(cellCountX, cellCountY);
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

    // Near-plane clipping generates vertices absent from the original eight AABB corners.
    static bool CrossesNearPlane(ViewProjectionState state, SpatialAabb bounds)
    {
        var min = double.PositiveInfinity; var max = double.NegativeInfinity;
        for (var corner = 0; corner < 8; corner++)
        {
            var depth = (TerrainFrustumCuller.Corner(bounds, corner) -
                state.Camera.Position).Dot(state.Camera.Forward);
            min = global::System.Math.Min(min, depth); max = global::System.Math.Max(max, depth);
        }
        return min <= state.Camera.NearPlane && max >= state.Camera.NearPlane;
    }

    static double ProjectedPixels(ViewProjectionState state, SpatialAabb bounds)
    {
        var minX = double.PositiveInfinity;
        var maxX = double.NegativeInfinity;
        var minY = double.PositiveInfinity;
        var maxY = double.NegativeInfinity;
        for (var corner = 0; corner < 8; corner++)
        {
            var point = TerrainFrustumCuller.Transform(
                state, TerrainFrustumCuller.Corner(bounds, corner));
            if (!(point.W > 0.0f)) continue;
            var x = (point.X / point.W + 1.0) * 0.5 * state.Viewport.PhysicalWidth;
            var y = (point.Y / point.W + 1.0) * 0.5 * state.Viewport.PhysicalHeight;
            minX = global::System.Math.Min(minX, x);
            maxX = global::System.Math.Max(maxX, x);
            minY = global::System.Math.Min(minY, y);
            maxY = global::System.Math.Max(maxY, y);
        }

        return double.IsPositiveInfinity(minX)
            ? 0.0
            : global::System.Math.Max(maxX - minX, maxY - minY);
    }
}
