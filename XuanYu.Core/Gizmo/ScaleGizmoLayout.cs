using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// Scale Gizmo 屏幕空间布局：三轴末端控制柄 + 中心等比控制柄。
// 当前没有可见 Global/Local 切换入口，默认锁定世界 X/Y/Z 轴。
public sealed class ScaleGizmoLayout
{
    public ScreenPoint Center { get; }
    public ScreenPoint[] AxisEnd { get; }   // [X, Y, Z]
    public double HandleSizeDip { get; }
    public double CenterSizeDip { get; }

    ScaleGizmoLayout(ScreenPoint center, ScreenPoint[] axisEnd)
    {
        Center = center;
        AxisEnd = axisEnd;
        HandleSizeDip = ScaleGizmoScreenSize.HandleScreenSizeDip;
        CenterSizeDip = ScaleGizmoScreenSize.CenterScreenSizeDip;
    }

    public static ScaleGizmoLayout Project(
        ViewProjectionState state, Vector3d origin, double worldAxisLength, Vector3d entityRotation)
    {
        ScreenPoint center;
        try
        {
            center = state.ProjectWorldPoint(origin);
        }
        catch (InvalidOperationException)
        {
            return new ScaleGizmoLayout(default, [default, default, default]);
        }
        _ = entityRotation;
        var ends = new ScreenPoint[3];
        for (var i = 0; i < 3; i++)
        {
            var dir = UnitAxis(i) * worldAxisLength;
            try
            {
                ends[i] = state.ProjectWorldPoint(origin + dir);
            }
            catch (InvalidOperationException)
            {
                ends[i] = center;
            }
        }
        return new ScaleGizmoLayout(center, ends);
    }

    static Vector3d UnitAxis(int i) =>
        i == 0 ? Vector3d.UnitX : (i == 1 ? Vector3d.UnitY : Vector3d.UnitZ);

}
