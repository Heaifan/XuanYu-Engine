using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.UI;

sealed class UiVmD1Handler(
    UiVm vm, GestureOwner owner, Func<ViewportState> viewport) : IViewportD1ConsumerHandler
{
    readonly UiVm _vm = vm;
    readonly GestureOwner _owner = owner;
    readonly Func<ViewportState> _viewport = viewport;
    public bool CanClaim(EditorPointerEvent p) => p.Kind == EditorPointerEventKind.Pressed &&
        (_owner == GestureOwner.Camera ? p.Buttons.HasFlag(EditorPointerButtons.Middle) :
         _owner == GestureOwner.Gizmo ? _vm.HasSelection && !_vm.IsSelectTool : p.Buttons.HasFlag(EditorPointerButtons.Left));
    public void Begin(ViewportGestureContext c)
    {
        if (_owner == GestureOwner.Camera) _vm.BeginCameraNavigation(c.PointerId, c.Input.Position.X, c.Input.Position.Y,
            c.Input.Modifiers.HasFlag(EditorPointerModifiers.Shift), (int)_viewport().LogicalWidth, (int)_viewport().LogicalHeight);
        else if (_owner == GestureOwner.Gizmo) _vm.BeginViewportPointer(c.PointerId, c.Input.Position.X, c.Input.Position.Y, true, true);
        else _vm.PickViewportPointer(c.Input.Position.X, c.Input.Position.Y, (int)_viewport().LogicalWidth,
            (int)_viewport().LogicalHeight, _viewport().PhysicalWidth, _viewport().PhysicalHeight, _viewport().DpiScale, _viewport().Revision, true);
    }
    public void Update(ViewportGestureContext c)
    {
        if (_owner == GestureOwner.Camera) _vm.PreviewCameraNavigation(c.PointerId, c.Input.Position.X, c.Input.Position.Y);
        if (_owner == GestureOwner.Gizmo) _vm.PreviewViewportPointer(c.PointerId, c.Input.Position.X, c.Input.Position.Y);
    }
    public void Commit(ViewportGestureContext c)
    {
        if (_owner == GestureOwner.Camera) _vm.EndCameraNavigation(c.PointerId);
        if (_owner == GestureOwner.Gizmo) _vm.CommitViewportPointer(c.PointerId, c.Input.Position.X, c.Input.Position.Y);
    }
    public void Cancel(ViewportCancellationContext c)
    {
        if (_owner == GestureOwner.Camera) _vm.CancelCameraNavigation(c.Reason.ToString());
        if (_owner == GestureOwner.Gizmo) _vm.CancelInteractionFromNativePointer(c.Reason.ToString());
    }
}
