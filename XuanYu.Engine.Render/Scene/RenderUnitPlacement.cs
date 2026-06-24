using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Scene;

/// <summary>
/// 单位渲染位置与 Picking 包围盒的单一真源。
/// 渲染位置（GPU 模型矩阵）和 Picking 包围盒（CPU AABB）必须从此结构推导。
/// 禁止 EditorShell / Session / Picking 独立计算位置或偏移。
/// </summary>
public sealed record RenderUnitPlacement
{
    // ─── 单位视觉参数（所有子系统必须使用同一组） ─────────────
    /// <summary>立方体局部尺寸（BuildCube size 参数）。</summary>
    public const double LocalSize = 1.0;

    /// <summary>渲染缩放（与 BuildCube 配合）。</summary>
    public const double Scale = 1.25;

    /// <summary>
    /// 局部半边长：localSize * scale / 2 = 1.0 * 1.25 / 2 = 0.625。
    /// 这是 GPU 立方体中心到面的距离，也是 CPU AABB 的半边长。
    /// </summary>
    public const double HalfExtent = LocalSize * Scale / 2.0;

    // ─── 实例数据 ──────────────────────────────────────────────

    /// <summary>地面锚点（Z=0）。</summary>
    public Vector3d GroundAnchor { get; }

    /// <summary>视觉中心（GroundAnchor + Z 半边长）。</summary>
    public Vector3d VisualCenter { get; }

    /// <summary>Picking 包围盒。</summary>
    public SceneAxisAlignedBounds SelectionBounds { get; }

    /// <summary>
    /// 从地面锚点创建完整 Placement。
    /// </summary>
    public RenderUnitPlacement(Vector3d groundAnchor)
    {
        GroundAnchor = groundAnchor;
        VisualCenter = new Vector3d(
            groundAnchor.X,
            groundAnchor.Y,
            groundAnchor.Z + HalfExtent);
        SelectionBounds = new SceneAxisAlignedBounds(
            VisualCenter,
            new Vector3d(HalfExtent, HalfExtent, HalfExtent));
    }

    /// <summary>
    /// 检查是否与指定视觉中心和缩放一致（运行时诊断断言）。
    /// </summary>
    public bool IsConsistentWithDraw(float drawCenterX, float drawCenterY, float drawCenterZ, float drawScale)
    {
        return Math.Abs(drawCenterX - VisualCenter.X) < 0.0001 &&
               Math.Abs(drawCenterY - VisualCenter.Y) < 0.0001 &&
               Math.Abs(drawCenterZ - VisualCenter.Z) < 0.0001 &&
               Math.Abs(drawScale - Scale) < 0.0001;
    }

    /// <summary>诊断摘要。</summary>
    public string ToDiagnostic()
    {
        var (bcx, bcy, bcz) = (SelectionBounds.Center.X, SelectionBounds.Center.Y, SelectionBounds.Center.Z);
        var (bhx, bhy, bhz) = (SelectionBounds.HalfExtents.X, SelectionBounds.HalfExtents.Y, SelectionBounds.HalfExtents.Z);
        return $"GroundAnchor ({GroundAnchor.X:F3},{GroundAnchor.Y:F3},{GroundAnchor.Z:F3}), " +
               $"VisualCenter ({VisualCenter.X:F3},{VisualCenter.Y:F3},{VisualCenter.Z:F3}), " +
               $"Scale {Scale:F3}, " +
               $"BoundsCenter ({bcx:F3},{bcy:F3},{bcz:F3}), " +
               $"BoundsHalf ({bhx:F3},{bhy:F3},{bhz:F3})";
    }
}
