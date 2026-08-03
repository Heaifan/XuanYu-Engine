using System.ComponentModel;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

// VIEWPORT-RESIZE-R2：修复 R1 引入的 DPI 错配。
// R1 把 Avalonia 逻辑尺寸（Bounds）直接当作 Win32/Vulkan 物理像素尺寸使用，
// 导致子 HWND 被 resize 成逻辑尺寸、画面只占左上角、右侧/下方露黑。
// 修正：Win32ViewportHost.Resize 必须收到物理像素（round(逻辑 × DPI)）；
// _bridge.Resize 仍收逻辑尺寸（供日志与请求尺寸）。拖动窗口仍走 OnSizeChanged → Coalescer。
public sealed partial class VulkanNativeHost
{
    void HookLayoutSync()
    {
        if (_layoutSyncHooked || DataContext is not UiVm vm) return;
        vm.PropertyChanged += OnLayoutSyncProp;
        _layoutSyncHooked = true;
    }

    void UnhookLayoutSync()
    {
        if (!_layoutSyncHooked || DataContext is not UiVm vm) return;
        vm.PropertyChanged -= OnLayoutSyncProp;
        _layoutSyncHooked = false;
    }

    void OnLayoutSyncProp(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiVm.IsLogOpen))
            Dispatcher.UIThread.InvokeAsync(SyncFinalSize, DispatcherPriority.Render);
    }

    void SyncFinalSize()
    {
        if (_hwnd == 0) return;
        var logicalW = (int)Bounds.Width;
        var logicalH = (int)Bounds.Height;
        if (logicalW <= 0 || logicalH <= 0) return;
        var dpi = GetDpiScale();
        var (physicalW, physicalH) = ToPhysicalSize(logicalW, logicalH, dpi);
        (DataContext as UiVm)?.UpdateViewportFrame(logicalW, logicalH);
        (DataContext as UiVm)?.UpdateViewportDpi(dpi);
        _resizer.Cancel();
        Win32ViewportHost.Resize(_hwnd, physicalW, physicalH);
        var (aw, ah) = Win32ViewportHost.GetClientSize(_hwnd);
        var open = DataContext is UiVm vm && vm.IsLogOpen;
        ViewportNativeHostRoute.ReportProbe(DataContext as UiVm, open, logicalW, logicalH, dpi, physicalW, physicalH, aw, ah);
        _resizer.OnResize(logicalW, logicalH, dpi, true, _hwnd);
    }
}
