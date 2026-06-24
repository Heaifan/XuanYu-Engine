using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的布局计算。
/// 将实体 Pivot 投影到屏幕，以固定像素尺寸计算轴端点、平面块顶点和中心手柄。
/// 使用 PresentedCameraSnapshot 做唯一投影。
/// </summary>
public sealed class MoveGizmoLayout
{
    // ─── 尺寸常量（像素） ──────────────────────────────────
    public const double CenterRadius = 10.0;
    public const double AxisStartPixels = 14.0;
    public const double AxisEndPixels = 110.0;
    public const double ArrowLength = 14.0;
    public const double ShaftHalfWidth = 1.5;
    public const double PlaneInner = 24.0;
    public const double PlaneOuter = 44.0;

    public required double PivotPixelX { get; init; }
    public required double PivotPixelY { get; init; }

    // ─── 三轴端点 ────────────────────────────────────────
    public required double AxisEndPixelX_AxisX { get; init; }
    public required double AxisEndPixelY_AxisX { get; init; }
    public required double AxisEndPixelX_AxisY { get; init; }
    public required double AxisEndPixelY_AxisY { get; init; }
    public required double AxisEndPixelX_AxisZ { get; init; }
    public required double AxisEndPixelY_AxisZ { get; init; }

    public required bool AxisDegenerateX { get; init; }
    public required bool AxisDegenerateY { get; init; }
    public required bool AxisDegenerateZ { get; init; }

    /// <summary>轴在屏幕上的单位方向向量。</summary>
    public double AxisDirX_X => Normalize(AxisEndPixelX_AxisX - PivotPixelX, AxisEndPixelY_AxisX - PivotPixelY).X;
    public double AxisDirX_Y => Normalize(AxisEndPixelX_AxisX - PivotPixelX, AxisEndPixelY_AxisX - PivotPixelY).Y;
    public double AxisDirY_X => Normalize(AxisEndPixelX_AxisY - PivotPixelX, AxisEndPixelY_AxisY - PivotPixelY).X;
    public double AxisDirY_Y => Normalize(AxisEndPixelX_AxisY - PivotPixelX, AxisEndPixelY_AxisY - PivotPixelY).Y;
    public double AxisDirZ_X => Normalize(AxisEndPixelX_AxisZ - PivotPixelX, AxisEndPixelY_AxisZ - PivotPixelY).X;
    public double AxisDirZ_Y => Normalize(AxisEndPixelX_AxisZ - PivotPixelX, AxisEndPixelY_AxisZ - PivotPixelY).Y;

    /// <summary>轴杆起点（从中心偏移 AxisStartPixels）。</summary>
    public double AxisStartX_X => PivotPixelX + AxisDirX_X * AxisStartPixels;
    public double AxisStartY_X => PivotPixelY + AxisDirX_Y * AxisStartPixels;
    public double AxisStartX_Y => PivotPixelX + AxisDirY_X * AxisStartPixels;
    public double AxisStartY_Y => PivotPixelY + AxisDirY_Y * AxisStartPixels;
    public double AxisStartX_Z => PivotPixelX + AxisDirZ_X * AxisStartPixels;
    public double AxisStartY_Z => PivotPixelY + AxisDirZ_Y * AxisStartPixels;

    private static (double X, double Y) Normalize(double dx, double dy)
    {
        var len = Math.Sqrt(dx * dx + dy * dy);
        return len > 1e-10 ? (dx / len, dy / len) : (1, 0);
    }

    public static MoveGizmoLayout? Build(
        (double X, double Y) pivotPixel,
        (double X, double Y) axisEndX,
        (double X, double Y) axisEndY,
        (double X, double Y) axisEndZ,
        bool degX, bool degY, bool degZ)
    {
        return new MoveGizmoLayout
        {
            PivotPixelX = pivotPixel.X,
            PivotPixelY = pivotPixel.Y,
            AxisEndPixelX_AxisX = axisEndX.X,
            AxisEndPixelY_AxisX = axisEndX.Y,
            AxisEndPixelX_AxisY = axisEndY.X,
            AxisEndPixelY_AxisY = axisEndY.Y,
            AxisEndPixelX_AxisZ = axisEndZ.X,
            AxisEndPixelY_AxisZ = axisEndZ.Y,
            AxisDegenerateX = degX,
            AxisDegenerateY = degY,
            AxisDegenerateZ = degZ,
        };
    }
}
