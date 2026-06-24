using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Edit;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Editor.Transform.Translation.Plane;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

public sealed class TransformDragRoute
{
    readonly TransformEditSession _session = new();
    TransformDragKind _activeKind;
    AxisTranslationAnchor _axisAnchor;
    PlaneTranslationAnchor _planeAnchor;
    PresentedCameraSnapshot? _camera;
    SceneTransform _initialTransform;

    public TransformEditSession Session => _session;
    public TransformDragKind ActiveKind => _activeKind;
    public bool IsActive => _activeKind != TransformDragKind.None;

    public bool Begin(MoveGizmoElement el, double x, double y, TransformStartSnapshot s)
    {
        if (!s.Camera.IsValid) return false;
        _camera = s.Camera;
        _session.Begin(new(s.EntityId.Value.ToString(), s.InitialTransform, s.IsDirty, TransformEditKind.Translation));
        if (!StartFromElement(el, x, y, s.InitialTransform.Position))
        { _session.Cancel(); _camera = null; _activeKind = TransformDragKind.None; return false; }
        _initialTransform = _session.PreviewTransform; return true;
    }
    public void Confirm() { if (_activeKind == TransformDragKind.None) return; _activeKind = TransformDragKind.None; _session.Confirm(); _camera = null; }
    public SceneTransform? Cancel()
    {
        if (_activeKind == TransformDragKind.None) return null;
        var initial = _initialTransform; _activeKind = TransformDragKind.None; _session.Cancel(); _camera = null; return initial;
    }
    public TransformDragMoveResult Move(double x, double y)
    {
        if (_activeKind == TransformDragKind.None) return default;
        if (_activeKind == TransformDragKind.Axis && _axisAnchor.Mode == AxisTranslationMode.DragPlane)
            return MoveDrag(x, y, _axisAnchor.DragPlaneNormal, h => AxisTranslationSolver.SolveDragPlane(_axisAnchor, h));
        if (_activeKind == TransformDragKind.Axis)
        { var t = AxisTranslationSolver.Solve(_axisAnchor, x, y); _session.Preview(WithPos(_session.PreviewTransform, t)); return new(true); }
        return MoveDrag(x, y, _planeAnchor.PlaneNormal, h => PlaneTranslationSolver.Solve(_planeAnchor, h));
    }
    TransformDragMoveResult MoveDrag(double x, double y, Vector3d n, Func<Vector3d, Vector3d> s)
    {
        if (!BuildRay(x, y, out var r)) return default;
        var denom = r.Direction.Dot(n);
        if (Math.Abs(denom) < 1e-10) return default;
        var piv = _activeKind == TransformDragKind.Axis ? _axisAnchor.Pivot : _planeAnchor.PlaneOrigin;
        var t = (piv - r.Origin).Dot(n) / denom;
        return t <= 0 ? default : DoPreview(s(r.Origin + r.Direction * t));
    }
    TransformDragMoveResult DoPreview(Vector3d pos) { _session.Preview(WithPos(_session.PreviewTransform, pos)); return new(true); }
    bool StartFromElement(MoveGizmoElement el, double x, double y, Vector3d p) => el switch
    {
        MoveGizmoElement.AxisX => StartAxis(Vector3d.UnitX, x, y, p),
        MoveGizmoElement.AxisY => StartAxis(Vector3d.UnitY, x, y, p),
        MoveGizmoElement.AxisZ => StartAxis(Vector3d.UnitZ, x, y, p),
        MoveGizmoElement.PlaneXY => StartPlane(p, Vector3d.UnitZ, x, y),
        MoveGizmoElement.PlaneXZ => StartPlane(p, Vector3d.UnitY, x, y),
        MoveGizmoElement.PlaneYZ => StartPlane(p, Vector3d.UnitX, x, y),
        MoveGizmoElement.ViewPlane => StartPlane(p, GetCamForward(), x, y),
        _ => false,
    };
    bool StartAxis(Vector3d axis, double x, double y, Vector3d pivot)
    {
        var (a, ok) = AxisDragAnchorBuilder.Build(axis, x, y, pivot, _camera!, _session.PreviewTransform.Position);
        if (!ok) (a, ok) = FallbackScreenProj(axis, x, y, pivot);
        if (!ok) return false; _axisAnchor = a; _activeKind = TransformDragKind.Axis; return true;
    }
    (AxisTranslationAnchor, bool) FallbackScreenProj(Vector3d axis, double x, double y, Vector3d pivot)
    {
        var c = _camera!; var p = c.CameraPose;
        if (AxisScreenMetric.TryCompute(pivot, axis, c.ViewProjection, c.ViewportWidth, c.ViewportHeight, out var d, out var u))
            return (new(pivot, axis, pivot, u, d, x, y, AxisTranslationMode.ScreenProjection), true);
        double dx = p.PositionX - p.TargetX, dy = p.PositionY - p.TargetY, dz = p.PositionZ - p.TargetZ;
        var ppu = p.ProjectionMode == SceneProjectionMode.Orthographic ? p.OrthographicHeight / Math.Max(1, c.ViewportHeight) : 2.0 * Math.Sqrt(dx * dx + dy * dy + dz * dz) * Math.Tan(p.FieldOfViewDegrees * Math.PI / 360.0) / Math.Max(1, c.ViewportHeight);
        return ppu <= 0 ? (default, false) : (new(pivot, axis, pivot, 1.0 / ppu, new(0, -1), x, y, AxisTranslationMode.ScreenProjection), true);
    }
    bool StartPlane(Vector3d pivot, Vector3d normal, double x, double y)
    {
        var (a, ok) = PlaneDragAnchorBuilder.Build(pivot, normal, x, y, _camera!, _session.PreviewTransform.Position);
        return !ok ? false : (_planeAnchor = a, _activeKind = TransformDragKind.Plane, true).Item3;
    }
    bool BuildRay(double x, double y, out SceneRay ray)
    {
        var s = _camera;
        if (s is not { IsValid: true } || VulkanSceneRayBuilder.TryBuild((float)x, (float)y, s, (uint)s.ViewportWidth, (uint)s.ViewportHeight, out var r) != SceneRayBuildStatus.Success || r is null)
        { ray = null!; return false; }
        ray = r; return true;
    }
    Vector3d GetCamForward() => _camera?.CameraPose is not { } p ? Vector3d.UnitZ : new Vector3d(p.TargetX - p.PositionX, p.TargetY - p.PositionY, p.TargetZ - p.PositionZ) is var f && f.IsZero ? Vector3d.UnitZ : f.Normalize();
    static SceneTransform WithPos(SceneTransform t, Vector3d p) => new(p, t.Rotation, t.Scale);
}
