using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>
/// 变换输入的中央路由器。接收原始输入事件，分发给 Gizmo/Solver/Transaction。
/// 支持 Axis（轴向拖动）和 Plane（平面拖动）两种模式。
/// </summary>
public sealed class TransformInputRoute
{
    private readonly TransformEditSession _session = new();
    private readonly MoveGizmoInteraction _gizmoInteraction = new();
    private TransformDragKind _activeKind = TransformDragKind.None;
    private AxisTranslationAnchor _axisAnchor;
    private PlaneTranslationAnchor _planeAnchor;
    private Vector3d _cameraPosition;
    private Vector3d _cameraForward;
    private int _viewportWidth;
    private int _viewportHeight;

    public TransformEditSession Session => _session;
    public MoveGizmoInteraction Gizmo => _gizmoInteraction;

    public TransformInputResult OnPointerPressed(
        int button, double x, double y, Vector3d pivot,
        float[] viewProjection, int viewportWidth, int viewportHeight,
        double cameraDistance, double fovDeg, bool isOrtho, double orthoHeight,
        Vector3d cameraPosition, Vector3d cameraForward)
    {
        if (button != 1) return default;

        // 不回落：未命中 Gizmo 时不开始拖动
        var element = _gizmoInteraction.HoveredElement;
        if (element == MoveGizmoElement.None) return default;

        if (!_gizmoInteraction.TryBeginDrag(element)) return default;

        _cameraPosition = cameraPosition;
        _cameraForward = cameraForward;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;

        return StartDragFromElement(element, x, y, pivot,
            viewProjection, viewportWidth, viewportHeight,
            cameraDistance, fovDeg, isOrtho, orthoHeight);
    }

    public TransformInputResult OnPointerMoved(double x, double y,
        double fovDeg, bool isOrtho)
    {
        if (!_gizmoInteraction.IsDragging) return default;

        return _activeKind switch
        {
            TransformDragKind.Axis => SolveAxis(x, y),
            TransformDragKind.Plane => SolvePlane(x, y, fovDeg, isOrtho),
            _ => default,
        };
    }

    public TransformInputResult OnPointerReleased()
    {
        if (!_gizmoInteraction.IsDragging) return default;

        _gizmoInteraction.EndDrag();
        _session.Confirm();
        _activeKind = TransformDragKind.None;
        return new TransformInputResult(true, false, true, 0, 0, 0);
    }

    public void CancelDrag()
    {
        if (!_gizmoInteraction.IsDragging) return;
        _gizmoInteraction.EndDrag();
        _session.Cancel();
        _activeKind = TransformDragKind.None;
    }

    public void UpdateGizmoHover(double x, double y, MoveGizmoLayout layout)
    {
        var element = MoveGizmoHitTest.HitTest(layout, x, y);
        _gizmoInteraction.SetHover(element);
    }

    public bool HasHoveredElement =>
        _gizmoInteraction.HoveredElement != MoveGizmoElement.None;

    private TransformInputResult StartDragFromElement(
        MoveGizmoElement element, double x, double y, Vector3d pivot,
        float[] vp, int vw, int vh,
        double camDist, double fovDeg, bool isOrtho, double orthoH) =>
        element switch
        {
            MoveGizmoElement.AxisX => StartAxisDrag(Vector3d.UnitX,
                x, y, pivot, vp, vw, vh, camDist, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.AxisY => StartAxisDrag(Vector3d.UnitY,
                x, y, pivot, vp, vw, vh, camDist, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.AxisZ => StartAxisDrag(Vector3d.UnitZ,
                x, y, pivot, vp, vw, vh, camDist, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.PlaneXY => StartPlaneDrag(pivot, Vector3d.UnitZ,
                x, y, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.PlaneXZ => StartPlaneDrag(pivot, Vector3d.UnitY,
                x, y, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.PlaneYZ => StartPlaneDrag(pivot, Vector3d.UnitX,
                x, y, fovDeg, isOrtho, orthoH),
            MoveGizmoElement.ViewPlane => StartPlaneDrag(pivot, _cameraForward,
                x, y, fovDeg, isOrtho, orthoH),
            _ => default,
        };

    private TransformInputResult StartAxisDrag(Vector3d axis, double x, double y,
        Vector3d pivot, float[] vp, int vw, int vh,
        double camDist, double fovDeg, bool isOrtho, double orthoH)
    {
        _activeKind = TransformDragKind.Axis;

        if (AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out var dir, out var ppu))
        {
            _axisAnchor = new AxisTranslationAnchor(
                _session.PreviewTransform.Position,
                axis, pivot, ppu, dir, x, y, AxisTranslationMode.ScreenProjection);
        }
        else
        {
            var vh2 = Math.Max(1, vh);
            var fallbackPpu = isOrtho
                ? orthoH / vh2
                : 2.0 * camDist * Math.Tan(fovDeg * Math.PI / 360.0) / vh2;
            if (fallbackPpu <= 0) return default;
            _axisAnchor = new AxisTranslationAnchor(
                _session.PreviewTransform.Position,
                axis, pivot, 1.0 / fallbackPpu, new Vector2d(0, -1),
                x, y, AxisTranslationMode.ScreenProjection);
        }

        return new TransformInputResult(true, true, false, x, y, 0);
    }

    private TransformInputResult StartPlaneDrag(Vector3d pivot, Vector3d normal,
        double x, double y, double fovDeg, bool isOrtho, double orthoH)
    {
        _activeKind = TransformDragKind.Plane;

        var rayDir = ComputeRayDirection(x, y, fovDeg, isOrtho);
        var denom = Dot(rayDir, normal);
        if (Math.Abs(denom) < 1e-10) return default;

        var t = Dot(pivot - _cameraPosition, normal) / denom;
        if (t <= 0) return default;

        var hitPoint = _cameraPosition + rayDir * t;
        _planeAnchor = PlaneTranslationStart.CreateAnchor(
            _session.PreviewTransform.Position, pivot, normal, hitPoint);

        return new TransformInputResult(true, true, false, x, y, 0);
    }

    private TransformInputResult SolveAxis(double x, double y)
    {
        var target = AxisTranslationSolver.Solve(_axisAnchor, x, y);
        _session.Preview(SceneTransformDefaults.FromPosition(target));
        return new TransformInputResult(true, false, false, target.X, target.Y, target.Z);
    }

    private TransformInputResult SolvePlane(double x, double y, double fovDeg, bool isOrtho)
    {
        var rayDir = ComputeRayDirection(x, y, fovDeg, isOrtho);
        var denom = Dot(rayDir, _planeAnchor.PlaneNormal);
        if (Math.Abs(denom) < 1e-10) return default;

        var t = Dot(_planeAnchor.PlaneOrigin - _cameraPosition, _planeAnchor.PlaneNormal) / denom;
        if (t <= 0) return default;

        var hitPoint = _cameraPosition + rayDir * t;
        var target = PlaneTranslationSolver.Solve(_planeAnchor, hitPoint);
        _session.Preview(SceneTransformDefaults.FromPosition(target));
        return new TransformInputResult(true, false, false, target.X, target.Y, target.Z);
    }

    private Vector3d ComputeRayDirection(double screenX, double screenY,
        double fovDeg, bool isOrtho)
    {
        var aspect = (double)_viewportWidth / Math.Max(1, _viewportHeight);
        var fwd = _cameraForward;

        // Z-up 世界：上向量为 (0,0,1)，右向量为 forward × up
        var up = Vector3d.UnitZ;
        var right = Cross(fwd, up);
        if (right.LengthSquared < 1e-10)
            right = Cross(fwd, Vector3d.UnitY);
        right = right.Normalize();
        up = Cross(right, fwd).Normalize();

        var ndcX = 2.0 * screenX / _viewportWidth - 1.0;
        var ndcY = -(2.0 * screenY / _viewportHeight - 1.0);

        if (isOrtho) return fwd;

        var halfH = Math.Tan(fovDeg * Math.PI / 360.0);
        var halfW = halfH * aspect;

        var dir = fwd + right * ndcX * halfW + up * ndcY * halfH;
        return dir.Normalize();
    }

    private static Vector3d Cross(Vector3d a, Vector3d b) =>
        new(a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X);

    private static double Dot(Vector3d a, Vector3d b) => a.Dot(b);
}

internal enum TransformDragKind
{
    None,
    Axis,
    Plane,
}
