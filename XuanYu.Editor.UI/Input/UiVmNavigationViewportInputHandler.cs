using Avalonia;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.UI;

sealed class UiVmNavigationViewportInputHandler(UiVm vm) : INavigationViewportInputHandler
{
    readonly UiVm _vm = vm;
    bool _orbiting;
    public bool CanClaim(EditorPointerEvent pointer) =>
        pointer.Buttons.HasFlag(EditorPointerButtons.Left) && Hit(pointer).HitGizmo;
    public void Observe(EditorPointerEvent pointer)
    {
        if (pointer.Kind != EditorPointerEventKind.Move) return;
        var hit = Hit(pointer);
        _vm.SetNavigationGizmoHover(hit.IsEndpoint ? EndpointIndex(hit.Endpoint!) : -1);
    }
    public void Begin(ViewportGestureContext context)
    {
        var hit = Hit(context.Input);
        if (hit.IsEndpoint)
        {
            _vm.SetNavigationGizmoPressed(EndpointIndex(hit.Endpoint!));
            _vm.ApplyNavigationGizmoEndpoint(hit.Endpoint!);
        }
        else _orbiting = _vm.BeginNavigationGizmoOrbit(context.PointerId,
            context.Input.Position.X, context.Input.Position.Y);
    }
    public void Update(ViewportGestureContext context)
    {
        if (_orbiting) _vm.PreviewCameraNavigation(context.PointerId,
            context.Input.Position.X, context.Input.Position.Y);
    }
    public void Commit(ViewportGestureContext context)
    {
        _orbiting = false;
        _vm.SetNavigationGizmoPressed(-1);
        _vm.EndCameraNavigation(context.PointerId);
    }
    public void Cancel(ViewportCancellationContext context)
    {
        _orbiting = false;
        _vm.SetNavigationGizmoPressed(-1);
        _vm.CancelCameraNavigation(context.Reason.ToString());
    }

    GizmoHitResult Hit(EditorPointerEvent pointer)
    {
        var viewport = _vm.CurrentViewport;
        var center = new Point(NavigationGizmoLayout.GizmoSize / 2,
            NavigationGizmoLayout.GizmoSize / 2);
        var camera = _vm.NavigationCamera;
        var endpoints = NavigationGizmoLayout.Compute(camera.Right, camera.Up, camera.Forward, center);
        var topLeft = new Point(viewport.LogicalWidth - NavigationGizmoLayout.Margin -
            NavigationGizmoLayout.GizmoSize, NavigationGizmoLayout.Margin);
        var local = new Point(pointer.Position.X - topLeft.X, pointer.Position.Y - topLeft.Y);
        return NavigationGizmoHitTest.Hit(endpoints, local, center);
    }
    static int EndpointIndex(string endpoint) => endpoint switch
    {
        "+X" => 0, "+Y" => 1, "+Z" => 2, _ => -1
    };
}
