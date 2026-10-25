using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderStaticModelTransform(
    Vector3d Position, Vector3d Rotation, Vector3d Scale)
{
    public static RenderStaticModelTransform Identity { get; } =
        new(Vector3d.Zero, Vector3d.Zero, new Vector3d(1, 1, 1));
}
