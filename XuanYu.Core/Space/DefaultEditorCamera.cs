using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public static class DefaultEditorCamera
{
    public static Vector3d Position { get; } = new(4, 3, -5);
    public static Vector3d Target { get; } = Vector3d.Zero;
    public static Vector3d Up { get; } = Vector3d.UnitY;

    public static CameraState Create(long revision) => new(
        Position,
        (Target - Position).Normalize(),
        Up,
        60,
        0.1,
        100,
        revision);
}
