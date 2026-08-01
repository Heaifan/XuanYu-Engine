using XuanYu.Core.Math;

namespace XuanYu.Editor.Assets;

public readonly record struct StaticModelVertex(
    Vector3d Position,
    Vector3d Normal,
    StaticModelUv Uv0);

public readonly record struct StaticModelUv(double U, double V)
{
    public static StaticModelUv Zero { get; } = new(0, 0);
}
