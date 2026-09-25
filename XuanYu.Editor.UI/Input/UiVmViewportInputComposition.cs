using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Map;

namespace XuanYu.Editor.UI;

public static class UiVmViewportInputComposition
{
    public static ViewportInputComposition Create(UiVm vm)
    {
        var viewport = () => vm.CurrentViewport;
        var consumers = new IViewportInputConsumer[]
        {
            new NavigationViewportInputConsumer(new UiVmNavigationViewportInputHandler(vm)),
            new GizmoViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Gizmo, viewport)),
            new CameraViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Camera, viewport)),
            new PickingViewportInputConsumer(new UiVmD1Handler(vm, GestureOwner.Picking, viewport)),
            new MapGeometryInputConsumer(new UiVmMapBackend(vm, GestureOwner.MapEdit, viewport)),
            new RegionInputConsumer(new UiVmMapBackend(vm, GestureOwner.Region, viewport)),
            new RoadInputConsumer(new UiVmMapBackend(vm, GestureOwner.Road, viewport)),
            new MarkerInputConsumer(new UiVmMapBackend(vm, GestureOwner.Marker, viewport)),
        };
        return new(consumers, new ViewportInputCaptureCoordinator());
    }
}
