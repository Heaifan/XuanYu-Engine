using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Plane;
using XuanYu.Engine.Project.World.Transform;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
namespace FluidWarfare.Editor.Windows.Viewport.Transform.Drag;

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
        var cam = s.Camera;
        if (!cam.IsValid) return false;
        _camera = cam;
        _session.Begin(new TransformEditSnapshot(
            s.EntityId.Value.ToString(), s.InitialTransform, s.IsDirty, TransformEditKind.Translation));
        if (!StartFromElement(el, x, y, s.InitialTransform.Position))
        { _session.Cancel(); _camera = null; _activeKind = TransformDragKind.None; return false; }
        _initialTransform = _session.PreviewTransform;
        return true;
    }
    public void Confirm()
    {
        if (_activeKind == TransformDragKind.None) return;
        _activeKind = TransformDragKind.None; _session.Confirm(); _camera = null;
    }
    public SceneTransform? Cancel()
    {
        if (_activeKind == TransformDragKind.None) return null;
        var initial = _initialTransform;
        _activeKind = TransformDragKind.None; _session.Cancel(); _camera = null;
        return initial;
    }
    public TransformDragMoveResult Move(double x, double y)
    {
        if (_activeKind == TransformDragKind.None) return default;
        if (_activeKind == TransformDragKind.Axis)
        {
            var t = AxisTranslationSolver.Solve(_axisAnchor, x, y);
            _session.Preview(WithPos(_session.PreviewTransform, t));
            return new TransformDragMoveResult(true);
        }
        if (!BuildRay(x, y, out var r)) return default;
        var d = r.Direction.Dot(_planeAnchor.PlaneNormal);
        if (Math.Abs(d) < 1e-10) return default;
        var t2 = (_planeAnchor.PlaneOrigin - r.Origin).Dot(_planeAnchor.PlaneNormal) / d;
        if (t2 <= 0) return default;
        var target = PlaneTranslationSolver.Solve(_planeAnchor, r.At(t2));
        _session.Preview(WithPos(_session.PreviewTransform, target));
        return new TransformDragMoveResult(true);
    }
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
        if (!ok) return false;
        _axisAnchor = a; _activeKind = TransformDragKind.Axis; return true;
    }
    bool StartPlane(Vector3d pivot, Vector3d normal, double x, double y)
    {
        var (a, ok) = PlaneDragAnchorBuilder.Build(pivot, normal, x, y, _camera!, _session.PreviewTransform.Position);
        if (!ok) return false;
        _planeAnchor = a; _activeKind = TransformDragKind.Plane; return true;
    }
    bool BuildRay(double x, double y, out SceneRay ray)
    {
        var s = _camera;
        if (s is not { IsValid: true }) { ray = null!; return false; }
        var status = VulkanSceneRayBuilder.TryBuild((float)x, (float)y, s,
            (uint)s.ViewportWidth, (uint)s.ViewportHeight, out var r);
        if (status != SceneRayBuildStatus.Success || r is null) { ray = null!; return false; }
        ray = r; return true;
    }
    Vector3d GetCamForward() => _camera?.CameraPose is not { } p ? Vector3d.UnitZ : new Vector3d(p.TargetX - p.PositionX, p.TargetY - p.PositionY, p.TargetZ - p.PositionZ) is var f && f.IsZero ? Vector3d.UnitZ : f.Normalize();
    static SceneTransform WithPos(SceneTransform t, Vector3d p) => new(p, t.Rotation, t.Scale);
}
