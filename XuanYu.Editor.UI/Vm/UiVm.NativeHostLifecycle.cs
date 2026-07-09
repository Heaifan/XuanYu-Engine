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
        var detail = NativeHostLifecycleLogFormatter.Detail(snapshot) + $"；【NativeHost】合并次数：{mergeCount}";
        if (snapshot.IsValid) _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        else _logBus.Warning(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }

    // VIEWPORT-RESIZE-R1 探针：核对 Avalonia 逻辑尺寸 × DPI 是否与 Win32 子窗口物理尺寸对齐，
    // 进而与 Vulkan Surface CurrentExtent / Swapchain extent 对齐。
    public void LogNativeHostProbe(bool isLogOpen, int logicalW, int logicalH, int clientW, int clientH, double dpi)
    {
        var state = isLogOpen ? "展开" : "收起";
        var message = $"【NativeHost】布局同步探针：日志详情栏={state}；逻辑={logicalW}x{logicalH}；Win32子窗口={clientW}x{clientH}；DPI={dpi:F2}";
        var detail = $"逻辑×DPI≈{(int)(logicalW * dpi)}x{(int)(logicalH * dpi)}；子窗口物理={clientW}x{clientH}";
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);
        RefreshLogBindings();
    }
}
