using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public static class EditorCamera
{
    public static Vector3d Position { get; } = new(4, -5, 3);
    public static Vector3d Target { get; } = Vector3d.Zero;
    public static Vector3d Up { get; } = Vector3d.UnitZ;

    public static CameraState Create(long revision) => new(
        Position,
        (Target - Position).Normalize(),
        Up,
        60,
        0.1,
        100,
        revision);
}

public static class DefaultEditorCamera
{
    public static Vector3d Position => EditorCamera.Position;
    public static Vector3d Target => EditorCamera.Target;
    public static Vector3d Up => EditorCamera.Up;
    public static CameraState Create(long revision) => EditorCamera.Create(revision);
}
