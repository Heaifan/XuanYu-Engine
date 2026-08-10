using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void LogNativeHostLifecycle(NativeHostHandleSnapshot snapshot)
    {
        var isWarning = snapshot.State == NativeHostLifecycleState.Invalidated ||
            (snapshot.State == NativeHostLifecycleState.Resized && !snapshot.IsValid);
        var level = isWarning ? EditorLogLevel.Warning : EditorLogLevel.Info;
        var message = NativeHostLifecycleLogFormatter.Message(snapshot);
        var detail = NativeHostLifecycleLogFormatter.Detail(snapshot);
        if (level == EditorLogLevel.Warning) _logBus.Warning(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        else _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }

    public void LogNativeHostResizedMerged(NativeHostHandleSnapshot snapshot, int mergeCount)
    {
        var message = NativeHostLifecycleLogFormatter.MergedMessage(snapshot, mergeCount);
        var detail = NativeHostLifecycleLogFormatter.Detail(snapshot) + $"；合并次数：{mergeCount}";
        if (snapshot.IsValid) _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        else _logBus.Warning(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }

    // VIEWPORT-RESIZE-R2 探针：核对 Avalonia 逻辑尺寸 × DPI 是否等于目标物理（= Win32 子窗口物理尺寸），
    // 进而与 Vulkan Surface 当前尺寸 / Swapchain 物理尺寸对齐。
    public void LogNativeHostProbe(bool isLogOpen, int logicalW, int logicalH, double dpi, int targetW, int targetH, int clientW, int clientH)
    {
        var state = isLogOpen ? "展开" : "收起";
        var message = $"【NativeHost】布局同步探针：日志详情栏={state}；逻辑尺寸={logicalW}x{logicalH}；DPI={dpi:F2}；目标物理尺寸={targetW}x{targetH}；Win32子窗口={clientW}x{clientH}";
        var detail = $"逻辑×DPI≈{(int)(logicalW * dpi)}x{(int)(logicalH * dpi)}；目标物理={targetW}x{targetH}；子窗口实际={clientW}x{clientH}";
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }

    internal void LogScaleIndicatorProbe(Win32ViewportHost.ScaleIndicatorProbe probe)
    {
        var message = $"【Native比例尺】HWND=0x{probe.Hwnd.ToInt64():X}；窗口={probe.IsWindow}；可见={probe.IsVisible}；文本={probe.Text}";
        var detail = $"矩形={probe.Left},{probe.Top},{probe.Right},{probe.Bottom}；尺寸宽度={probe.WindowWidth}；WM_PAINT次数={probe.PaintCount}；矩形有效={probe.HasRect}";
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }
}
