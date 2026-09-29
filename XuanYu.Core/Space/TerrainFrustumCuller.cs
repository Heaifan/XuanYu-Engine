using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Space;

public static class TerrainFrustumCuller
{
    public static bool Intersects(ViewProjectionState state, SpatialAabb bounds)
    {
        for (var plane = 0; plane < 6; plane++)
        {
            var outside = true;
            for (var corner = 0; corner < 8; corner++)
            {
                if (!Outside(Transform(state, Corner(bounds, corner)), plane))
                {
                    outside = false;
                    break;
                }
            }

            if (outside)
            {
                return false;
            }
        }

        return true;
    }

    internal static Vector4 Transform(Matrix4x4 matrix, Vector3d point) =>
        Vector4.Transform(new Vector4((float)point.X, (float)point.Y, (float)point.Z, 1), matrix);

    internal static Vector4 Transform(ViewProjectionState state, Vector3d point) =>
        Transform(state.ViewProjection, point - state.RenderOrigin);

    internal static Vector3d Corner(SpatialAabb box, int index) => new(
        (index & 1) == 0 ? box.Min.X : box.Max.X,
        (index & 2) == 0 ? box.Min.Y : box.Max.Y,
        (index & 4) == 0 ? box.Min.Z : box.Max.Z);

    static bool Outside(Vector4 point, int plane) => plane switch
    {
        0 => point.X < -point.W,
        1 => point.X > point.W,
        2 => point.Y < -point.W,
        3 => point.Y > point.W,
        4 => point.Z < 0,
        _ => point.Z > point.W
    };

}
