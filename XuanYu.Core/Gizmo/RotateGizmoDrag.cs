using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

// 旋转拖拽解算：将指针在"垂直于旋转轴的平面"上的投影角度变化，映射为
// 欧拉度（CommittedTransform.Rotation）绕该轴的增量。采用"每帧增量解缠+累计"
// 策略，保证无 NaN、无 360° 跳变、无逐帧累积误差。
public sealed partial class RotateGizmoDrag
{
    readonly ViewProjectionState _state;
    readonly Vector3d _origin;
    readonly RotateGizmoAxis _axis;
    readonly Vector3d _basis1;
    readonly Vector3d _basis2;
    readonly Vector3d _startRotation;
    readonly double _startComponent;
    readonly Vector3d _eye;
    double _prevAngle;
    double _accumulatedDeltaDeg;
    bool _initialized;

    public RotateGizmoDrag(
        ViewProjectionState state,
        Vector3d origin,
        RotateGizmoAxis axis,
        Vector3d startRotation,
        double x,
        double y)
    {
        _state = state;
        _origin = origin;
        _axis = axis;
        (_basis1, _basis2) = RotateGizmoLayout.Basis(axis);
        _startRotation = startRotation;
        _startComponent = Component(startRotation, axis);
        _eye = state.Camera.Position;
        _prevAngle = 0.0;
        _accumulatedDeltaDeg = 0.0;
        _initialized = false;
    }

    public Vector3d? Solve(double x, double y)
    {
        var angle = PlaneAngle(x, y);
        if (angle is null) return null;
        if (!_initialized)
        {
            _prevAngle = angle.Value;
            _initialized = true;
            return _startRotation;
        }
        var inc = UnwrapToPlusMinus180(angle.Value - _prevAngle);
        _accumulatedDeltaDeg += inc;
        _prevAngle = angle.Value;
        var newComponent = _startComponent + _accumulatedDeltaDeg;
        return WithComponent(_startRotation, _axis, newComponent);
    }

    double? PlaneAngle(double x, double y)
    {
        var world = RayPlaneHit(x, y);
        if (world is null) return null;
        var v = world.Value - _origin;
        var a = v.Dot(_basis1);
        var b = v.Dot(_basis2);
        if (a == 0.0 && b == 0.0) return 0.0;
        return global::System.Math.Atan2(b, a) * (180.0 / global::System.Math.PI);
    }

    Vector3d? RayPlaneHit(double x, double y)
    {
        var ndc = ToNdc(x, y);
        Vector3d worldPoint;
        try { worldPoint = _state.TransformPointToWorld(ndc.X, ndc.Y, 0.0); }
        catch { return null; }
        var dir = (worldPoint - _eye).Normalize();
        var normal = AxisUnit(_axis);
        var denom = dir.Dot(normal);
        if (global::System.Math.Abs(denom) < 1e-6) return null;
        var t = (_origin - _eye).Dot(normal) / denom;
        if (!double.IsFinite(t)) return null;
        return _eye + (dir * t);
    }

    (double X, double Y) ToNdc(double x, double y)
    {
        var vp = _state.Viewport;
        var ndcX = ((x - vp.LogicalX) / (0.5 * vp.LogicalWidth)) - 1.0;
        var ndcY = 1.0 - ((y - vp.LogicalY) / (0.5 * vp.LogicalHeight));
        return (ndcX, ndcY);
    }

}
