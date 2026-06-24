using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Project.World.Transform;

/// <summary>
/// SceneTransform 各分量的有效性校验。
/// </summary>
public static class SceneTransformValidation
{
    public static bool IsPositionValid(Vector3d p) =>
        double.IsFinite(p.X) && double.IsFinite(p.Y) && double.IsFinite(p.Z);

    public static bool IsRotationValid(Vector3d r) =>
        double.IsFinite(r.X) && double.IsFinite(r.Y) && double.IsFinite(r.Z);

    public static bool IsScaleValid(Vector3d s) =>
        double.IsFinite(s.X) && double.IsFinite(s.Y) && double.IsFinite(s.Z) &&
        s.X >= 0.01 && s.Y >= 0.01 && s.Z >= 0.01;

    public static bool IsValid(SceneTransform t) =>
        IsPositionValid(t.Position) && IsRotationValid(t.Rotation) && IsScaleValid(t.Scale);
}
