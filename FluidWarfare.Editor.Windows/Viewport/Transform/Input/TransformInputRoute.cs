using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Constraint;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>
/// 变换输入的中央路由器。接收原始输入事件，分发给 Gizmo/Solver/Transaction。
/// 是 EditorShell 的唯一切入点。
/// </summary>
public sealed class TransformInputRoute
{
    private readonly TransformEditSession _session = new();
    private readonly MoveGizmoInteraction _gizmoInteraction = new();
    private AxisTranslationAnchor _axisAnchor;
    private PlaneTranslationAnchor _planeAnchor;
    private bool _hasAnchor;

    public TransformEditSession Session => _session;
    public MoveGizmoInteraction Gizmo => _gizmoInteraction;

    public TransformInputResult OnPointerPressed(int button, double x, double y, Vector3d pivot)
    {
        if (button != 1 || _session.IsActive) return default;

        // 没有 Gizmo 悬停时默认为自由移动（Blender G 模式）
        var element = _gizmoInteraction.HoveredElement;
        if (element == MoveGizmoElement.None)
            element = MoveGizmoElement.ViewPlane;

        if (!_gizmoInteraction.TryBeginDrag(element)) return default;

        return StartDragFromElement(element, x, y, pivot);
    }

    public TransformInputResult OnPointerMoved(double x, double y, Vector3d pivot)
    {
        // Gizmo hover
        // (layout must be set externally from the latest camera)
        _gizmoInteraction.SetHover(MoveGizmoElement.None); // simplified

        if (!_gizmoInteraction.IsDragging || !_hasAnchor)
            return default;

        // Axis drag
        var target = AxisTranslationSolver.Solve(_axisAnchor, x, y);
        _session.Preview(SceneTransformDefaults.FromPosition(target));
        return new TransformInputResult(true, false, false, target.X, target.Y, target.Z);
    }

    public TransformInputResult OnPointerReleased()
    {
        if (!_gizmoInteraction.IsDragging) return default;

        _gizmoInteraction.EndDrag();
        _session.Confirm();
        _hasAnchor = false;
        return new TransformInputResult(true, false, true, 0, 0, 0);
    }

    public void CancelDrag()
    {
        if (!_gizmoInteraction.IsDragging) return;
        _gizmoInteraction.EndDrag();
        _session.Cancel();
        _hasAnchor = false;
    }

    public void UpdateGizmoHover(double x, double y, MoveGizmoLayout layout)
    {
        var element = MoveGizmoHitTest.HitTest(layout, x, y);
        _gizmoInteraction.SetHover(element);
    }

    private TransformInputResult StartDragFromElement(
        MoveGizmoElement element, double x, double y, Vector3d pivot)
    {
        var axis = element switch
        {
            MoveGizmoElement.AxisX => Vector3d.UnitX,
            MoveGizmoElement.AxisY => Vector3d.UnitY,
            MoveGizmoElement.AxisZ => Vector3d.UnitZ,
            MoveGizmoElement.ViewPlane => Vector3d.UnitX,
            _ => Vector3d.Zero,
        };

        if (axis == Vector3d.Zero) return default;

        _axisAnchor = new AxisTranslationAnchor(
            _session.PreviewTransform.Position,
            axis, pivot, 50, new Vector2d(1, 0),
            x, y, AxisTranslationMode.ScreenProjection);
        _hasAnchor = true;
        return new TransformInputResult(true, true, false, x, y, 0);
    }
}
