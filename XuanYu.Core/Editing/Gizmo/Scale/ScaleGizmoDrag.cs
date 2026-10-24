using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// Scale Gizmo 拖拽解算：指数映射，倍率恒为正、不穿过零，且不逐帧累乘。
// 所有倍率基于 StartScale 计算：单轴手柄只改对应分量，Uniform 三轴同倍。
public sealed class ScaleGizmoDrag
{
    public const double MinimumScale = 0.01;
    public const double SensitivityDip = 110.0;

    readonly ScaleGizmoHandle _handle;
    readonly Vector3d _startScale;
    readonly double _startX;
    readonly double _startY;
    readonly ScreenPoint _axisDir;   // 轴向手柄的屏幕归一化方向；Uniform 未使用

    public ScaleGizmoDrag(
        ScaleGizmoHandle handle,
        Vector3d startScale,
        double startX,
        double startY,
        ScreenPoint axisScreenDir)
    {
        _handle = handle;
        _startScale = startScale;
        _startX = startX;
        _startY = startY;
        _axisDir = axisScreenDir;
    }

    public Vector3d Solve(double x, double y)
    {
        double factor;
        if (_handle == ScaleGizmoHandle.Uniform)
        {
            var projected = _startY - y;            // 向上拖动放大
            factor = System.Math.Exp(projected / SensitivityDip);
        }
        else
        {
            var dx = x - _startX;
            var dy = y - _startY;
            var projected = dx * _axisDir.X + dy * _axisDir.Y;
            factor = System.Math.Exp(projected / SensitivityDip);
        }
        factor = double.IsFinite(factor) ? factor : 1.0;
        return Clamp(ApplyFactor(_startScale, _handle, factor));
    }

    static Vector3d ApplyFactor(Vector3d s, ScaleGizmoHandle handle, double factor)
    {
        return handle switch
        {
            ScaleGizmoHandle.X => new Vector3d(s.X * factor, s.Y, s.Z),
            ScaleGizmoHandle.Y => new Vector3d(s.X, s.Y * factor, s.Z),
            ScaleGizmoHandle.Z => new Vector3d(s.X, s.Y, s.Z * factor),
            _ => new Vector3d(s.X * factor, s.Y * factor, s.Z * factor),
        };
    }

    static Vector3d Clamp(Vector3d s) => new(
        System.Math.Max(MinimumScale, s.X),
        System.Math.Max(MinimumScale, s.Y),
        System.Math.Max(MinimumScale, s.Z));
}
