using XuanYu.Render.Vulkan;

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
}
