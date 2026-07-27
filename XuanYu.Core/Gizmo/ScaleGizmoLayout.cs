using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// Scale Gizmo 屏幕空间布局：三轴末端控制柄 + 中心等比控制柄。
// 轴方向随实体局部旋转（与渲染层 eulerRot 一致），保证视觉轴 = 实际缩放分量。
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
        var ends = new ScreenPoint[3];
        for (var i = 0; i < 3; i++)
        {
            var dir = RotateByEuler(UnitAxis(i), entityRotation) * worldAxisLength;
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

    // 与 scene.vert eulerRot(deg)=Rz*Ry*Rx 完全一致，将世界轴旋转到实体局部朝向。
    static Vector3d RotateByEuler(Vector3d v, Vector3d deg)
    {
        var r = deg * (System.Math.PI / 180.0);
        var cx = System.Math.Cos(r.X); var sx = System.Math.Sin(r.X);
        var cy = System.Math.Cos(r.Y); var sy = System.Math.Sin(r.Y);
        var cz = System.Math.Cos(r.Z); var sz = System.Math.Sin(r.Z);
        var x0 = v.X;
        var y0 = cx * v.Y + sx * v.Z;
        var z0 = -sx * v.Y + cx * v.Z;
        var x1 = cy * x0 + sy * z0;
        var y1 = y0;
        var z1 = -sy * x0 + cy * z0;
        var x2 = cz * x1 - sz * y1;
        var y2 = sz * x1 + cz * y1;
        return new Vector3d(x2, y2, z1);
    }
}
