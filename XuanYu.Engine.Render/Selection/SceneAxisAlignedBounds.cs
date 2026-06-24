using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Render.Selection;

/// <summary>
/// 轴对齐包围盒（AABB），用于 Picking 和渲染尺寸统一。
/// 中心 Position + HalfExtents 定义。
/// </summary>
public sealed record SceneAxisAlignedBounds(Vector3d Center, Vector3d HalfExtents)
{
    public Vector3d Minimum => new(
        Center.X - HalfExtents.X,
        Center.Y - HalfExtents.Y,
        Center.Z - HalfExtents.Z);

    public Vector3d Maximum => new(
        Center.X + HalfExtents.X,
        Center.Y + HalfExtents.Y,
        Center.Z + HalfExtents.Z);
}
