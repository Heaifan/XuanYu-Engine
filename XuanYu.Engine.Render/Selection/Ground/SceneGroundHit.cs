using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Render.Selection.Ground;

/// <summary>
/// 地面射线求交结果。未命中时 IsHit = false。
/// </summary>
public sealed record SceneGroundHit(
    bool IsHit,
    double Distance,
    Vector3d? WorldPosition)
{
    public static readonly SceneGroundHit NoHit = new(false, 0, null);

    public static SceneGroundHit Hit(double distance, Vector3d worldPosition) =>
        new(true, distance, worldPosition);
}
