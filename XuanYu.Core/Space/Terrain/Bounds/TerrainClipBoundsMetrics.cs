using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public readonly record struct TerrainScreenBounds(double MinX, double MinY,
    double MaxX, double MaxY);

public readonly record struct TerrainClipBoundsMetrics(int FrontFacingCornerCount,
    int FarOutsideCornerCount, int NearOutsideCornerCount, double MinDepthRatio,
    double MaxDepthRatio, TerrainScreenBounds ProjectedBounds)
{
    public static TerrainClipBoundsMetrics Measure(ViewProjectionState state,
        SpatialAabb bounds)
    {
        var front = 0; var far = 0; var near = 0;
        var minDepth = double.PositiveInfinity; var maxDepth = double.NegativeInfinity;
        var minX = double.PositiveInfinity; var minY = double.PositiveInfinity;
        var maxX = double.NegativeInfinity; var maxY = double.NegativeInfinity;
        for (var index = 0; index < 8; index++)
        {
            var point = Corner(bounds, index) - state.RenderOrigin;
            var clip = Vector4.Transform(new Vector4((float)point.X, (float)point.Y,
                (float)point.Z, 1), state.ViewProjection);
            if (clip.Z < 0) far++;
            if (clip.Z > clip.W) near++;
            if (clip.W <= 0) continue;
            front++;
            var depth = clip.Z / clip.W;
            minDepth = global::System.Math.Min(minDepth, depth);
            maxDepth = global::System.Math.Max(maxDepth, depth);
            var x = state.Viewport.LogicalX + (clip.X / clip.W + 1) *
                0.5 * state.Viewport.LogicalWidth;
            var y = state.Viewport.LogicalY + (1 - clip.Y / clip.W) *
                0.5 * state.Viewport.LogicalHeight;
            minX = global::System.Math.Min(minX, x); minY = global::System.Math.Min(minY, y);
            maxX = global::System.Math.Max(maxX, x); maxY = global::System.Math.Max(maxY, y);
        }
        if (front == 0)
            return new(front, far, near, double.NaN, double.NaN,
                new(double.NaN, double.NaN, double.NaN, double.NaN));
        var depthMin = minDepth;
        var depthMax = front == 0 ? double.NaN : maxDepth;
        return new(front, far, near, depthMin, depthMax,
            new(minX, minY, maxX, maxY));
    }

    static Vector3d Corner(SpatialAabb box, int index) => new(
        (index & 1) == 0 ? box.Min.X : box.Max.X,
        (index & 2) == 0 ? box.Min.Y : box.Max.Y,
        (index & 4) == 0 ? box.Min.Z : box.Max.Z);
}
