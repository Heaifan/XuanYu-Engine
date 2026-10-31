namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly LogPresenter _logPresenter = new();
    EditorLogBus _logBus => _logPresenter.LogBus;
    public LogPresenter LogPresentation => _logPresenter;
    public bool HasSelectedEntries => _logPresenter.HasSelectedEntries;
    public string SelectedEntriesClipboardText => _logPresenter.SelectedEntriesClipboardText;
    public void NotifyLogCopied() => _logPresenter.NotifyLogCopied();
    void LogCommand(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var category = name is "保存" ? EditorLogCategory.Save : EditorLogCategory.Command;
        var source = name is "构建" ? EditorLogSource.Build : EditorLogSource.Editor;
        _logBus.Info(source, category, $"命令已触发：{name}", "顶部工具栏低频命令。");

    }

    void LogTool(string tool)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Tool, $"当前工具切换为：{tool}", "工具切换是低频输入事件。");

    }

    void LogInteraction(string message, string detail)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture, message, detail);

    }

    public void LogVulkanLifecycle(string message, string detail)
    {
        if (EditorLogNoiseFilter.SuppressRenderBackendInfo(message)) return;
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, message, detail);

    }
}
