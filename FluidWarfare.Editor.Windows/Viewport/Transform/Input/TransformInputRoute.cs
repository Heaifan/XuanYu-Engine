using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>
/// 变换输入的中央路由器。
/// 使用 PresentedCameraSnapshot 统一射线（透视/正交同源），
/// 根据 Axis/Plane DragKind 分流到对应 Solver。
/// </summary>
public sealed class TransformInputRoute
{
    private readonly TransformEditSession _session = new();
    private readonly MoveGizmoInteraction _gizmoInteraction = new();
    private TransformDragKind _activeKind = TransformDragKind.None;
    private AxisTranslationAnchor _axisAnchor;
    private PlaneTranslationAnchor _planeAnchor;
    private PresentedCameraSnapshot? _cameraSnapshot;

    public TransformEditSession Session => _session;
    public MoveGizmoInteraction Gizmo => _gizmoInteraction;

    public TransformInputResult OnPointerPressed(
        int button, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot? cameraSnapshot)
    {
        if (button != 1) return default;

        var element = _gizmoInteraction.HoveredElement;
        if (element == MoveGizmoElement.None) return default;
        if (!_gizmoInteraction.TryBeginDrag(element)) return default;

        _cameraSnapshot = cameraSnapshot;
        return StartDragFromElement(element, x, y, pivot);
    }

    public TransformInputResult OnPointerMoved(double x, double y)
    {
        if (!_gizmoInteraction.IsDragging) return default;
        return _activeKind switch
        {
            TransformDragKind.Axis => SolveAxis(x, y),
            TransformDragKind.Plane => SolvePlane(x, y),
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
        MoveGizmoElement element, double x, double y, Vector3d pivot) =>
        element switch
        {
            MoveGizmoElement.AxisX => StartAxisDrag(Vector3d.UnitX, x, y, pivot),
            MoveGizmoElement.AxisY => StartAxisDrag(Vector3d.UnitY, x, y, pivot),
            MoveGizmoElement.AxisZ => StartAxisDrag(Vector3d.UnitZ, x, y, pivot),
            MoveGizmoElement.PlaneXY => StartPlaneDrag(pivot, Vector3d.UnitZ, x, y),
            MoveGizmoElement.PlaneXZ => StartPlaneDrag(pivot, Vector3d.UnitY, x, y),
            MoveGizmoElement.PlaneYZ => StartPlaneDrag(pivot, Vector3d.UnitX, x, y),
            MoveGizmoElement.ViewPlane => StartPlaneDrag(pivot, CameraForward, x, y),
            _ => default,
        };

    private Vector3d CameraForward
    {
        get
        {
            if (_cameraSnapshot?.CameraPose is not { } pose) return Vector3d.UnitZ;
            var fwd = new Vector3d(
                pose.TargetX - pose.PositionX,
                pose.TargetY - pose.PositionY,
                pose.TargetZ - pose.PositionZ);
            return fwd.IsZero ? Vector3d.UnitZ : fwd.Normalize();
        }
    }

    private TransformInputResult StartAxisDrag(Vector3d axis, double x, double y, Vector3d pivot)
    {
        _activeKind = TransformDragKind.Axis;

        if (_cameraSnapshot is not { IsValid: true } snap) return default;
        var vp = snap.ViewProjection;
        var vw = snap.ViewportWidth;
        var vh = snap.ViewportHeight;
        var pose = snap.CameraPose;
        var isOrtho = pose.ProjectionMode == SceneProjectionMode.Orthographic;
        var fovDeg = pose.FieldOfViewDegrees;
        var camDist = Math.Sqrt(
            Math.Pow(pose.PositionX - pose.TargetX, 2) +
            Math.Pow(pose.PositionY - pose.TargetY, 2) +
            Math.Pow(pose.PositionZ - pose.TargetZ, 2));

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
                ? pose.OrthographicHeight / vh2
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
        double x, double y)
    {
        _activeKind = TransformDragKind.Plane;

        if (!TryBuildRay(x, y, out var ray)) return default;

        var denom = ray.Direction.Dot(normal);
        if (Math.Abs(denom) < 1e-10) return default;

        var t = (pivot - ray.Origin).Dot(normal) / denom;
        if (t <= 0) return default;

        var hitPoint = ray.At(t);
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

    private TransformInputResult SolvePlane(double x, double y)
    {
        if (!TryBuildRay(x, y, out var ray)) return default;

        var denom = ray.Direction.Dot(_planeAnchor.PlaneNormal);
        if (Math.Abs(denom) < 1e-10) return default;

        var t = (_planeAnchor.PlaneOrigin - ray.Origin).Dot(_planeAnchor.PlaneNormal) / denom;
        if (t <= 0) return default;

        var hitPoint = ray.At(t);
        var target = PlaneTranslationSolver.Solve(_planeAnchor, hitPoint);
        _session.Preview(SceneTransformDefaults.FromPosition(target));
        return new TransformInputResult(true, false, false, target.X, target.Y, target.Z);
    }

    /// <summary>
    /// 使用 PresentedCameraSnapshot 的 InverseViewProjection 统一构建射线。
    /// 透视和正交共用此路径。
    /// </summary>
    private bool TryBuildRay(double x, double y, out SceneRay ray)
    {
        ray = null!;
        var snap = _cameraSnapshot;
        if (snap is null || !snap.IsValid) return false;

        var status = VulkanSceneRayBuilder.TryBuild(
            (float)x, (float)y, snap,
            (uint)snap.ViewportWidth, (uint)snap.ViewportHeight,
            out var r);
        if (status != SceneRayBuildStatus.Success || r is null) return false;

        ray = r;
        return true;
    }
}

internal enum TransformDragKind
{
    None,
    Axis,
    Plane,
}
