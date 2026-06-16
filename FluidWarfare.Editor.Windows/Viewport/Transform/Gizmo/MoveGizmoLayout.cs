namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的布局计算。
/// 将实体 Pivot 投影到屏幕，以固定像素尺寸计算轴端点、平面块顶点和中心手柄。
/// 使用 PresentedCameraSnapshot 做唯一投影。
/// </summary>
public sealed class MoveGizmoLayout
{
    private const double PlaneBlockOffset = 30.0;

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

    // ─── 三轴屏幕方向（单位向量） ────────────────────────

    public double AxisDirX_X => Normalize(AxisEndPixelX_AxisX - PivotPixelX, AxisEndPixelY_AxisX - PivotPixelY).X;
    public double AxisDirX_Y => Normalize(AxisEndPixelX_AxisX - PivotPixelX, AxisEndPixelY_AxisX - PivotPixelY).Y;
    public double AxisDirY_X => Normalize(AxisEndPixelX_AxisY - PivotPixelX, AxisEndPixelY_AxisY - PivotPixelY).X;
    public double AxisDirY_Y => Normalize(AxisEndPixelX_AxisY - PivotPixelX, AxisEndPixelY_AxisY - PivotPixelY).Y;
    public double AxisDirZ_X => Normalize(AxisEndPixelX_AxisZ - PivotPixelX, AxisEndPixelY_AxisZ - PivotPixelY).X;
    public double AxisDirZ_Y => Normalize(AxisEndPixelX_AxisZ - PivotPixelX, AxisEndPixelY_AxisZ - PivotPixelY).Y;

    // ─── 平面块四角（30px 偏移） ─────────────────────────

    public double PlaneXY_Corner0X => PivotPixelX;
    public double PlaneXY_Corner0Y => PivotPixelY;
    public double PlaneXY_Corner1X => PivotPixelX + AxisDirX_X * PlaneBlockOffset;
    public double PlaneXY_Corner1Y => PivotPixelY + AxisDirX_Y * PlaneBlockOffset;
    public double PlaneXY_Corner2X => PivotPixelX + AxisDirX_X * PlaneBlockOffset + AxisDirY_X * PlaneBlockOffset;
    public double PlaneXY_Corner2Y => PivotPixelY + AxisDirX_Y * PlaneBlockOffset + AxisDirY_Y * PlaneBlockOffset;
    public double PlaneXY_Corner3X => PivotPixelX + AxisDirY_X * PlaneBlockOffset;
    public double PlaneXY_Corner3Y => PivotPixelY + AxisDirY_Y * PlaneBlockOffset;

    public double PlaneXZ_Corner0X => PivotPixelX;
    public double PlaneXZ_Corner0Y => PivotPixelY;
    public double PlaneXZ_Corner1X => PivotPixelX + AxisDirX_X * PlaneBlockOffset;
    public double PlaneXZ_Corner1Y => PivotPixelY + AxisDirX_Y * PlaneBlockOffset;
    public double PlaneXZ_Corner2X => PivotPixelX + AxisDirX_X * PlaneBlockOffset + AxisDirZ_X * PlaneBlockOffset;
    public double PlaneXZ_Corner2Y => PivotPixelY + AxisDirX_Y * PlaneBlockOffset + AxisDirZ_Y * PlaneBlockOffset;
    public double PlaneXZ_Corner3X => PivotPixelX + AxisDirZ_X * PlaneBlockOffset;
    public double PlaneXZ_Corner3Y => PivotPixelY + AxisDirZ_Y * PlaneBlockOffset;

    public double PlaneYZ_Corner0X => PivotPixelX;
    public double PlaneYZ_Corner0Y => PivotPixelY;
    public double PlaneYZ_Corner1X => PivotPixelX + AxisDirY_X * PlaneBlockOffset;
    public double PlaneYZ_Corner1Y => PivotPixelY + AxisDirY_Y * PlaneBlockOffset;
    public double PlaneYZ_Corner2X => PivotPixelX + AxisDirY_X * PlaneBlockOffset + AxisDirZ_X * PlaneBlockOffset;
    public double PlaneYZ_Corner2Y => PivotPixelY + AxisDirY_Y * PlaneBlockOffset + AxisDirZ_Y * PlaneBlockOffset;
    public double PlaneYZ_Corner3X => PivotPixelX + AxisDirZ_X * PlaneBlockOffset;
    public double PlaneYZ_Corner3Y => PivotPixelY + AxisDirZ_Y * PlaneBlockOffset;

    // ─── 中心手柄半径 ────────────────────────────────────

    public double CenterHandleRadius => CenterHandleRadiusValue;

    private const double CenterHandleRadiusValue = 6.0;

    // ─── 辅助 ─────────────────────────────────────────────

    public double AxisScreenLengthX =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisX, AxisEndPixelY_AxisX);
    public double AxisScreenLengthY =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisY, AxisEndPixelY_AxisY);
    public double AxisScreenLengthZ =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisZ, AxisEndPixelY_AxisZ);

    private static double Dist(double x1, double y1, double x2, double y2) =>
        Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

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
