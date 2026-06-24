using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Project.World.Transform;

/// <summary>
/// SceneTransform 默认值和工厂方法。
/// </summary>
public static class SceneTransformDefaults
{
    public static Vector3d DefaultPosition => Vector3d.Zero;
    public static Vector3d DefaultRotation => Vector3d.Zero;
    public static Vector3d DefaultScale => new(1, 1, 1);

    public static SceneTransform CreateDefault() =>
        new(DefaultPosition, DefaultRotation, DefaultScale);

    public static SceneTransform FromPosition(Vector3d position) =>
        new(position, DefaultRotation, DefaultScale);
}
