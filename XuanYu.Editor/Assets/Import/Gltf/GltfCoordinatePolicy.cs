using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Editor.Assets;

public static class GltfCoordinatePolicy
{
    public static Vector3d ToXuanYuPosition(Vector3d gltf) =>
        new(gltf.X, -gltf.Z, gltf.Y);

    public static Vector3d ToXuanYuNormal(Vector3d gltf) =>
        ToXuanYuPosition(gltf).Normalize();

    public static (int A, int B, int C) ToXuanYuTriangle(int a, int b, int c) =>
        (a, b, c);

    public static SpatialAabb ToConvertedBounds(IReadOnlyList<Vector3d> gltfPositions)
    {
        if (gltfPositions.Count == 0)
            throw new ArgumentException("模型顶点不能为空。", nameof(gltfPositions));
        var first = ToXuanYuPosition(gltfPositions[0]);
        var min = first;
        var max = first;
        foreach (var source in gltfPositions.Skip(1))
        {
            var v = ToXuanYuPosition(source);
            min = new Vector3d(Math.Min(min.X, v.X), Math.Min(min.Y, v.Y), Math.Min(min.Z, v.Z));
            max = new Vector3d(Math.Max(max.X, v.X), Math.Max(max.Y, v.Y), Math.Max(max.Z, v.Z));
        }
        return new SpatialAabb(min, max);
    }
}
