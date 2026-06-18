using FluidWarfare.Core.Math;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Drag;
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;

/// <summary>
/// 视口指针变换交互路由。负责：Gizmo HitTest、Drag 调度、G 模态逐出。
/// 不持有 WorldState/RenderScene/VulkanSession，所有外部依赖通过参数传入。
/// </summary>
public sealed class TransformPointerRoute
{
    readonly TransformInteractionState _state = new();
    readonly MoveGizmoInteraction _gizmo = new();
    readonly TransformDragRoute _dragRoute = new();

    public TransformInteractionState State => _state;
    public TransformEditSession Session => _dragRoute.Session;
    public MoveGizmoElement HoveredElement => _gizmo.HoveredElement;
    public bool HasHoveredElement => _gizmo.HoveredElement != MoveGizmoElement.None;
    public bool IsDragActive => _dragRoute.IsActive;

    public void UpdateGizmoHover(double x, double y, MoveGizmoLayout layout)
    {
        var el = MoveGizmoHitTest.HitTest(layout, x, y);
        _gizmo.SetHover(el);
    }
    public TransformInteractionResult OnPointerPressed(
        TransformStartRequest req, TransformStartSnapshot snap)
    {
        var element = req.Source == TransformStartSource.GizmoHandle
            ? _gizmo.HoveredElement : req.GizmoElement;
        if (element == MoveGizmoElement.None) return default;

        if (!_gizmo.TryBeginDrag(element))
        { _gizmo.ClearHover(); return default; }

        if (!_dragRoute.Begin(element, req.PointerX, req.PointerY, snap))
        { _gizmo.EndDrag(); return default; }

        return new TransformInteractionResult(
            TransformInteractionAction.Started,
            _dragRoute.Session.PreviewTransform,
            ReasonFromSource(req.Source, element));
    }
    public TransformInteractionResult OnPointerMoved(double x, double y)
    {
        var r = _dragRoute.Move(x, y);
        if (!r.Handled) return default;

        return new TransformInteractionResult(
            TransformInteractionAction.Previewed,
            _dragRoute.Session.PreviewTransform,
            TransformInteractionReason.None);
    }
    public TransformInteractionResult OnPointerReleased()
    {
        if (!_dragRoute.IsActive) return default;

        var finalTransform = _dragRoute.Session.PreviewTransform;
        _gizmo.EndDrag();
        _dragRoute.Confirm();

        return new TransformInteractionResult(
            TransformInteractionAction.Confirmed, finalTransform,
            TransformInteractionReason.None);
    }
    public void Cancel()
    {
        _gizmo.EndDrag();
        _dragRoute.Cancel();
    }
    static TransformInteractionReason ReasonFromSource(
        TransformStartSource src, MoveGizmoElement el) => src switch
    {
        TransformStartSource.EntityBody => TransformInteractionReason.EntityBody,
        TransformStartSource.BlenderG => TransformInteractionReason.BlenderG,
        _ when IsAxis(el) => TransformInteractionReason.GizmoAxis,
        _ => TransformInteractionReason.GizmoPlane,
    };
    static bool IsAxis(MoveGizmoElement e) => e
        is MoveGizmoElement.AxisX or MoveGizmoElement.AxisY or MoveGizmoElement.AxisZ;
}
