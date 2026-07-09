using System.ComponentModel;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

// VIEWPORT-RESIZE-R1：日志详情栏展开/收起是低频离散布局变化，
// 不等 250ms Coalescer，布局稳定（Render 优先级）后立即同步最终尺寸，
// 避免 NativeHost 子窗口/HWND 与 Vulkan Swapchain 不同步导致的半屏黑 / 视口恢复慢半拍。
// 拖动窗口仍走 OnSizeChanged → NativeHostResizeCoalescer（高频合并）。
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
        var w = (int)Bounds.Width;
        var h = (int)Bounds.Height;
        if (w <= 0 || h <= 0) return;
        _resizer.Cancel();
        var (cw, ch) = Win32ViewportHost.GetClientSize(_hwnd);
        var open = DataContext is UiVm vm && vm.IsLogOpen;
        ViewportNativeHostRoute.ReportProbe(DataContext as UiVm, open, w, h, cw, ch, GetDpiScale());
        Win32ViewportHost.Resize(_hwnd, w, h);
        _bridge?.Resize(w, h);
    }
}
