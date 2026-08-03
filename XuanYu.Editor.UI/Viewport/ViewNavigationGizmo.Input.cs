using System;
using Avalonia;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

// F3-D3：导航 Gizmo 输入——点击六端点对齐视角、中心球/非端点拖动 Orbit。
// 点击与拖动区分：移动 < 4 DIP = 点击；≥ 4 DIP = Orbit（复用 UiVm 相机会话）。
// PointerCaptureLost / 取消 → 正常结束；导航操作不进入 Dirty/Undo。
public sealed partial class ViewNavigationGizmo
{
    const double ClickOrbitThresholdDips = 4.0;
    Point _pointerDown;
    bool _pointerCaptured;
    bool _dragOrbit;
    string? _hoverEndpoint;
    string? _pressedEndpoint;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        if (DataContext is not UiVm vm) return;
        var point = e.GetPosition(this);
        _pointerDown = point;
        _pointerCaptured = true;
        _dragOrbit = false;
        _pressedEndpoint = HitEndpoints(point);
        e.Pointer.Capture(this);
        e.Handled = true;
        // 端点按下暂不动作（等待移动阈值判定）；非端点（中心球/空白）直接进入 Orbit 候选。
        if (_pressedEndpoint is null && vm.NavigationCamera is not null)
            vm.BeginCameraNavigation(e.Pointer.Id, point.X, point.Y, false, (int)Width, (int)Height);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var point = e.GetPosition(this);
        var hover = HitEndpoints(point);
        if (hover != _hoverEndpoint) { _hoverEndpoint = hover; InvalidateVisual(); }
        if (!_pointerCaptured || _dragOrbit || DataContext is not UiVm vm) return;
        var dx = point.X - _pointerDown.X;
        var dy = point.Y - _pointerDown.Y;
        if (System.Math.Sqrt((dx * dx) + (dy * dy)) < ClickOrbitThresholdDips) return;
        // 超过阈值：端点点击转为 Orbit（不触发视角跳转）。
        _dragOrbit = true;
        _pressedEndpoint = null;
        InvalidateVisual();
        if (vm.NavigationCamera is not null)
            vm.BeginCameraNavigation(e.Pointer.Id, point.X, point.Y, false, (int)Width, (int)Height);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_pointerCaptured) return;
        _pointerCaptured = false;
        e.Pointer.Capture(null);
        if (DataContext is not UiVm vm) return;
        var point = e.GetPosition(this);
        if (_dragOrbit)
        {
            _dragOrbit = false;
            vm.EndCameraNavigation(e.Pointer.Id);
            e.Handled = true;
            return;
        }
        // 点击：执行标准视角命令（保留 Pivot 与距离；日志由 ApplyViewFaceCommand 记录）。
        var hit = _pressedEndpoint ?? HitEndpoints(point);
        _pressedEndpoint = null;
        if (hit is not null)
        {
            vm.RunCommand.Execute($"视角-{StandardViewResolver.EndpointToViewName(hit)}");
            e.Handled = true;
            return;
        }
        if (vm.NavigationCamera is not null) vm.EndCameraNavigation(e.Pointer.Id);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _pointerCaptured = false;
        _dragOrbit = false;
        _pressedEndpoint = null;
        if (DataContext is UiVm vm && vm.NavigationCamera is not null)
            vm.CancelCameraNavigation("Gizmo 捕获丢失");
    }

    string? HitEndpoints(Point point)
    {
        if (NavigationCamera is not { } camera) return null;
        var endpoints = NavigationGizmoLayout.Compute(camera.Right, camera.Up, camera.Forward, Center);
        return NavigationGizmoHitTest.Hit(endpoints, point, Center).Endpoint;
    }
}
