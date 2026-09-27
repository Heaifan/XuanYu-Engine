using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public static class TerrainFrustumCuller
{
    public static bool Intersects(ViewProjectionState state, SpatialAabb bounds)
    {
        var corners = Corners(bounds);
        for (var plane = 0; plane < 6; plane++)
        {
            if (corners.All(corner => Outside(Transform(state.ViewProjection, corner), plane)))
            {
                return false;
            }
        }

        return true;
    }

    internal static Vector4 Transform(Matrix4x4 matrix, Vector3d point) =>
        Vector4.Transform(new Vector4((float)point.X, (float)point.Y, (float)point.Z, 1), matrix);

    static bool Outside(Vector4 point, int plane) => plane switch
    {
        0 => point.X < -point.W,
        1 => point.X > point.W,
        2 => point.Y < -point.W,
        3 => point.Y > point.W,
        4 => point.Z < 0,
        _ => point.Z > point.W
    };

    internal static Vector3d[] Corners(SpatialAabb box) =>
    [
        new(box.Min.X, box.Min.Y, box.Min.Z), new(box.Max.X, box.Min.Y, box.Min.Z),
        new(box.Min.X, box.Max.Y, box.Min.Z), new(box.Max.X, box.Max.Y, box.Min.Z),
        new(box.Min.X, box.Min.Y, box.Max.Z), new(box.Max.X, box.Min.Y, box.Max.Z),
        new(box.Min.X, box.Max.Y, box.Max.Z), new(box.Max.X, box.Max.Y, box.Max.Z)
    ];
}
