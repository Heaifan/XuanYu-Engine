using Avalonia;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.Editor.UI;

sealed class UiVmNavigationViewportInputHandler(UiVm vm) : INavigationViewportInputHandler
{
    readonly UiVm _vm = vm;
    bool _orbiting;
    string? _candidate;
    Point _pressPosition;
    public bool CanClaim(EditorPointerEvent pointer) =>
        pointer.Buttons.HasFlag(EditorPointerButtons.Left) && Hit(pointer).HitGizmo;
    public void Observe(EditorPointerEvent pointer)
    {
        if (pointer.Kind != EditorPointerEventKind.Move) return;
        var hit = Hit(pointer);
        _vm.SetNavigationGizmoHover(hit.IsEndpoint ? EndpointIndex(hit.Endpoint!) : -1);
        _vm.SetNavigationGizmoCenterHover(hit.HitCenter);
    }
    public void Begin(ViewportGestureContext context)
    {
        var hit = Hit(context.Input);
        _pressPosition = new Point(context.Input.Position.X, context.Input.Position.Y);
        _candidate = null;
        if (hit.IsEndpoint)
        {
            _candidate = hit.Endpoint;
            _vm.SetNavigationGizmoPressed(EndpointIndex(hit.Endpoint!));
        }
        else _orbiting = _vm.BeginNavigationGizmoOrbit(context.PointerId,
            context.Input.Position.X, context.Input.Position.Y);
    }
    public void Update(ViewportGestureContext context)
    {
        var current = new Point(context.Input.Position.X, context.Input.Position.Y);
        if (!_orbiting && _candidate is not null && Distance(_pressPosition, current) >= 4.0)
        {
            _orbiting = _vm.BeginNavigationGizmoOrbit(context.PointerId,
                _pressPosition.X, _pressPosition.Y);
            _candidate = null;
            _vm.SetNavigationGizmoPressed(-1);
        }
        if (_orbiting) _vm.PreviewCameraNavigation(context.PointerId,
            context.Input.Position.X, context.Input.Position.Y);
    }
    public void Commit(ViewportGestureContext context)
    {
        if (_candidate is not null) _vm.ApplyNavigationGizmoEndpoint(_candidate);
        if (_orbiting) _vm.EndCameraNavigation(context.PointerId);
        _orbiting = false;
        _candidate = null;
        _vm.SetNavigationGizmoPressed(-1);
    }
    public void Cancel(ViewportCancellationContext context)
    {
        if (_orbiting) _vm.CancelCameraNavigation(context.Reason.ToString());
        _orbiting = false;
        _candidate = null;
        _vm.SetNavigationGizmoPressed(-1);
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
        "+X" => 0, "-X" => 1, "+Y" => 2, "-Y" => 3,
        "+Z" => 4, "-Z" => 5, _ => -1
    };

    static double Distance(Point a, Point b)
    {
        var dx = a.X - b.X; var dy = a.Y - b.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }
}
