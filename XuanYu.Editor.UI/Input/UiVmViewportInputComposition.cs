using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.Editor.UI;

public static class UiVmViewportInputComposition
{
    public static ViewportInputComposition Create(UiVm vm)
    {
        var viewport = () => vm.CurrentViewport;
        var consumers = new IViewportInputConsumer[]
        {
            new MapContextMenuInputConsumer(vm),
            new NavigationViewportInputConsumer(new UiVmNavigationViewportInputHandler(vm)),
            new GizmoViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Gizmo, viewport)),
            new CameraViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Camera, viewport)),
            new PickingViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Picking, viewport)),
            new MapGeometryInputConsumer(new UiVmMapBackend(vm, GestureOwner.MapEdit, viewport)),
            new RegionInputConsumer(new UiVmMapBackend(vm, GestureOwner.Region, viewport)),
            new RoadInputConsumer(new UiVmMapBackend(vm, GestureOwner.Road, viewport)),
            new MarkerInputConsumer(new UiVmMapBackend(vm, GestureOwner.Marker, viewport)),
        };
        return new(consumers.Select(x => (IViewportInputConsumer)
            new ToolChangeLifecycleConsumer(x)),
            new ViewportInputCaptureCoordinator(), vm.CloseTerminalInteraction);
    }

    sealed class ToolChangeLifecycleConsumer(IViewportInputConsumer consumer)
        : IViewportInputConsumer
    {
        readonly IViewportInputConsumer _consumer = consumer;
        public GestureOwner Owner => _consumer.Owner;
        public int BeginPriority => _consumer.BeginPriority;
        public bool CanBegin(EditorPointerEvent pointer, ViewportGestureState state) =>
            _consumer.CanBegin(pointer, state);
        public ViewportInputDispatchResult Handle(
            EditorPointerEvent pointer, ViewportGestureState state) =>
            _consumer.Handle(pointer, state);
        public void Begin(ViewportGestureContext context) => _consumer.Begin(context);
        public void Update(ViewportGestureContext context) => _consumer.Update(context);
        public void Commit(ViewportGestureContext context) => _consumer.Commit(context);
        public void Cancel(ViewportCancellationContext context)
        {
            if (context.Reason == ViewportCancellationReason.ToolChanged) return;
            _consumer.Cancel(context);
        }
    }
}
