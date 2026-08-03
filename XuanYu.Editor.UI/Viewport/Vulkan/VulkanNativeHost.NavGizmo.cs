using Avalonia;

namespace XuanYu.Editor.UI;

// F3-F1：导航 Gizmo 命中——原生指针消息流（Avalonia 覆盖层被原生子窗口遮挡，命中走这里）。
// 视口局部坐标 → Gizmo 局部坐标（右上角 12 DIP 边距、88×88 区域）；
// 端点点击 → 标准视角命令；中心球/空白按下 → 复用 UiVm 相机会话 Orbit（<4 DIP 视为点击不触发视角跳转）。
public sealed partial class VulkanNativeHost
{
    const double NavGizmoMarginDips = 14.0;
    const double NavGizmoSizeDips = 96.0;
    bool _navGizmoPressed;
    string? _navGizmoEndpoint;
    Point _navGizmoDown;

    bool TryNavGizmoPress(UiVm vm, double x, double y)
    {
        if (!NavGizmoLocal(x, y, Bounds.Width, Bounds.Height, out var local)) return false;
        var hit = vm.NavigationCamera is null ? null
            : NavigationGizmoHitTest.Hit(NavigationGizmoLayout.Compute(
                vm.NavigationCamera.Right, vm.NavigationCamera.Up, vm.NavigationCamera.Forward,
                new Point(NavGizmoSizeDips * 0.5, NavGizmoSizeDips * 0.5)), local, new Point(NavGizmoSizeDips * 0.5, NavGizmoSizeDips * 0.5));
        _navGizmoPressed = true;
        _navGizmoEndpoint = hit.IsEndpoint ? hit.Endpoint : null;
        _navGizmoDown = new Point(x, y);
        // 非端点（中心球/空白）：直接进入 Orbit 候选。
        if (_navGizmoEndpoint is null && vm.NavigationCamera is not null)
            vm.BeginCameraNavigation(NativePointerId, x, y, false, (int)Bounds.Width, (int)Bounds.Height);
        return true;
    }

    bool TryNavGizmoMove(UiVm vm, double x, double y)
    {
        if (!_navGizmoPressed) return false;
        var dx = x - _navGizmoDown.X;
        var dy = y - _navGizmoDown.Y;
        if (_navGizmoEndpoint is null)
        {
            // 中心球/空白拖动：直接预览 Orbit。
            if (vm.NavigationCamera is not null) vm.PreviewCameraNavigation(NativePointerId, x, y);
            return true;
        }
        // 端点按下且移动 ≥4 DIP：转为 Orbit（不触发视角跳转）。
        if ((dx * dx) + (dy * dy) >= 16.0 && vm.NavigationCamera is not null)
        {
            _navGizmoEndpoint = null;
            vm.BeginCameraNavigation(NativePointerId, x, y, false, (int)Bounds.Width, (int)Bounds.Height);
        }
        return true;
    }

    bool TryNavGizmoRelease(UiVm vm, double x, double y)
    {
        if (!_navGizmoPressed) return false;
        _navGizmoPressed = false;
        var endpoint = _navGizmoEndpoint;
        _navGizmoEndpoint = null;
        var dx = x - _navGizmoDown.X;
        var dy = y - _navGizmoDown.Y;
        if (endpoint is not null && (dx * dx) + (dy * dy) < 16.0)
        {
            // 端点点击：执行标准视角命令（保留 Pivot 与距离；日志由 ApplyViewFaceCommand 记录）。
            vm.RunCommand.Execute($"视角-{StandardViewResolver.EndpointToViewName(endpoint)}");
            return true;
        }
        if (vm.NavigationCamera is not null) vm.EndCameraNavigation(NativePointerId);
        return true;
    }

    void CancelNavGizmo(UiVm vm)
    {
        if (!_navGizmoPressed) return;
        _navGizmoPressed = false;
        _navGizmoEndpoint = null;
        if (vm.NavigationCamera is not null) vm.CancelCameraNavigation("Gizmo 捕获丢失");
    }

    // 视口局部坐标 → Gizmo 局部坐标；区域外返回 false（点击继续进入实体 Picking 等）。
    static bool NavGizmoLocal(double x, double y, double viewportW, double viewportH, out Point local)
    {
        local = default;
        var left = viewportW - NavGizmoMarginDips - NavGizmoSizeDips;
        var top = NavGizmoMarginDips;
        if (x < left || x > viewportW - NavGizmoMarginDips) return false;
        if (y < top || y > top + NavGizmoSizeDips) return false;
        local = new Point(x - left, y - top);
        return true;
    }
}
