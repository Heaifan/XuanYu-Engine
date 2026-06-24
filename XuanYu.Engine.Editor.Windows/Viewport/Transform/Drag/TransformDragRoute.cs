using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Edit;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Editor.Transform.Translation.Plane;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

public sealed class TransformDragRoute
{
    readonly TransformEditSession _session = new();
    TransformDragKind _activeKind;
    AxisTranslationAnchor _axisAnchor;
    PlaneTranslationAnchor _planeAnchor;
    PresentedCameraSnapshot? _camera;
    SceneTransform _initialTransform;

    public Action<string>? Trace { get; set; }
    public TransformEditSession Session => _session;
    public TransformDragKind ActiveKind => _activeKind;
    public bool IsActive => _activeKind != TransformDragKind.None;

    public bool Begin(MoveGizmoElement el, double x, double y, TransformStartSnapshot s)
    {
        if (!s.Camera.IsValid) { Trace?.Invoke("[D] Begin: 相机无效"); return false; }
        _camera = s.Camera;
        _session.Begin(new(s.EntityId.Value.ToString(), s.InitialTransform, s.IsDirty, TransformEditKind.Translation));
        var ip = s.InitialTransform.Position;
        Trace?.Invoke($"[D] Begin: el={el}, pos=({ip.X:F2},{ip.Y:F2},{ip.Z:F2})");
        if (!StartFromElement(el, x, y, ip))
        { Trace?.Invoke("[D] Begin: StartFromElement 失败"); _session.Cancel(); _camera = null; _activeKind = TransformDragKind.None; return false; }
        _initialTransform = _session.PreviewTransform;
        Trace?.Invoke($"[D] StartAxis: mode={_axisAnchor.Mode}");
        return true;
    }
    public void Confirm()
    {
        if (_activeKind == TransformDragKind.None) return;
        var fp = _session.PreviewTransform.Position; Trace?.Invoke($"[D] Confirm: ({fp.X:F2},{fp.Y:F2},{fp.Z:F2})");
        _activeKind = TransformDragKind.None; _session.Confirm(); _camera = null;
    }
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
        { var t = AxisTranslationSolver.Solve(_axisAnchor, x, y); Trace?.Invoke($"[D] Move SP: ({t.X:F2},{t.Y:F2},{t.Z:F2})"); _session.Preview(WithPos(_session.PreviewTransform, t)); return new(true); }
        return MoveDrag(x, y, _planeAnchor.PlaneNormal, h => PlaneTranslationSolver.Solve(_planeAnchor, h));
    }
    TransformDragMoveResult MoveDrag(double x, double y, Vector3d n, Func<Vector3d, Vector3d> s)
    {
        if (!BuildRay(x, y, out var r)) { Trace?.Invoke("[D] MoveDrag: BuildRay 失败"); return default; }
        var denom = r.Direction.Dot(n);
        if (Math.Abs(denom) < 1e-10) { Trace?.Invoke("[D] MoveDrag: 射线平行于平面"); return default; }
        var piv = _activeKind == TransformDragKind.Axis ? _axisAnchor.Pivot : _planeAnchor.PlaneOrigin;
        var t = (piv - r.Origin).Dot(n) / denom;
        if (t <= 0) { Trace?.Invoke($"[D] MoveDrag: t={t:F4}<=0"); return default; }
        var hit = r.Origin + r.Direction * t; var np = s(hit);
        Trace?.Invoke($"[D] Move: ({np.X:F2},{np.Y:F2},{np.Z:F2})");
        _session.Preview(WithPos(_session.PreviewTransform, np)); return new(true);
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
    bool StartAxis(Vector3d a, double x, double y, Vector3d p)
    { var (an, ok) = AxisDragAnchorBuilder.Build(a, x, y, p, _camera!, _session.PreviewTransform.Position); if (!ok) return false; _axisAnchor = an; _activeKind = TransformDragKind.Axis; return true; }
    bool StartPlane(Vector3d pivot, Vector3d normal, double x, double y)
    { var (an, ok) = PlaneDragAnchorBuilder.Build(pivot, normal, x, y, _camera!, _session.PreviewTransform.Position); return !ok ? false : (_planeAnchor = an, _activeKind = TransformDragKind.Plane, true).Item3; }
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
