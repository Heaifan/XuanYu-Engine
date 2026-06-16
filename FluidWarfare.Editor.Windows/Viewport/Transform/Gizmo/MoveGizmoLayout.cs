namespace FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// Move Gizmo 的布局计算。
/// 将实体 Pivot 投影到屏幕，以固定像素尺寸计算轴端点和平面块顶点。
/// 使用 PresentedCameraSnapshot 做唯一投影。
/// </summary>
public sealed class MoveGizmoLayout
{
    private const double AxisPixelLength = 80.0;
    private const double PlaneBlockPixelSize = 18.0;

    public required double PivotPixelX { get; init; }
    public required double PivotPixelY { get; init; }

    public required double AxisEndPixelX_AxisX { get; init; }
    public required double AxisEndPixelY_AxisX { get; init; }
    public required double AxisEndPixelX_AxisY { get; init; }
    public required double AxisEndPixelY_AxisY { get; init; }
    public required double AxisEndPixelX_AxisZ { get; init; }
    public required double AxisEndPixelY_AxisZ { get; init; }

    public required bool AxisDegenerateX { get; init; }
    public required bool AxisDegenerateY { get; init; }
    public required bool AxisDegenerateZ { get; init; }

    public double AxisScreenLengthX =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisX, AxisEndPixelY_AxisX);
    public double AxisScreenLengthY =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisY, AxisEndPixelY_AxisY);
    public double AxisScreenLengthZ =>
        Dist(PivotPixelX, PivotPixelY, AxisEndPixelX_AxisZ, AxisEndPixelY_AxisZ);

    private static double Dist(double x1, double y1, double x2, double y2) =>
        Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

    /// <summary>
    /// 从世界 Pivot 和三轴投影构建布局。
    /// </summary>
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
