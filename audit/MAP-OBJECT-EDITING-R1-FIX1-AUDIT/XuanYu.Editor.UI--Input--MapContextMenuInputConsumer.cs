using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.UI;

public sealed class MapContextMenuInputConsumer : IViewportInputConsumer
{
    readonly UiVm _vm;
    public MapContextMenuInputConsumer(UiVm vm) => _vm = vm;
    public GestureOwner Owner => GestureOwner.ContextMenu;
    public int BeginPriority => 1000;
    public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
        pointer.Kind == EditorPointerEventKind.Pressed && pointer.Buttons.HasFlag(EditorPointerButtons.Right);
    public ViewportInputDispatchResult Handle(EditorPointerEvent pointer, ViewportGestureState state) =>
        _vm.PublishMapGeometryContext(pointer) ? ViewportInputDispatchResult.Observed : ViewportInputDispatchResult.Ignored;
    public void Begin(ViewportGestureContext context) { }
    public void Update(ViewportGestureContext context) { }
    public void Commit(ViewportGestureContext context) { }
    public void Cancel(ViewportCancellationContext context) { }
}
