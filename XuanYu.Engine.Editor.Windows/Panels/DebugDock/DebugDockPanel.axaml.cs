using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.Logging;
using XuanYu.Engine.Editor.Windows.UI.Text;

namespace XuanYu.Engine.Editor.Windows.Panels.DebugDock;

/// <summary>底部日志面板。渲染诊断、RenderScene 和性能页已移除，避免 UI 热路径卡顿。</summary>
public sealed partial class DebugDockPanel : UserControl
{
    bool _isExpanded;
    LogPanel? _logPanel;

    public LogPanel? LogPanel => _logPanel;
    public event Action<bool>? ExpandedChanged;

    public DebugDockPanel() { InitializeComponent(); CacheControls(); UpdateVisualState(); }

    void CacheControls()
    {
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
    }

    void OnToggleDockClicked(object? sender, RoutedEventArgs e)
    {
        _isExpanded = !_isExpanded;
        UpdateVisualState();
        ExpandedChanged?.Invoke(_isExpanded);
    }

    void UpdateVisualState()
    {
        if (_logPanel is not null) _logPanel.IsVisible = _isExpanded;
        ToggleDockButton!.Content = _isExpanded ? EditorText.Collapse : EditorText.Expand;
    }

    public void SetDiagnostics(string loader, string instance, string device, string nativeHost,
        string surface, string swapchain, string clear, string marker, string validation)
    {
    }

    public void SetScene3d(string scene3d, string camera, string grid, string unit, string drawCall)
    {
    }

    public void SetPerformance(string instanceMs, string deviceMs, string swapchainMs,
        string clearMs, string markerMs, string scene3dMs)
    {
    }

    public void SetRenderSceneSummary(string title, IReadOnlyList<string> entries)
    {
    }
}
