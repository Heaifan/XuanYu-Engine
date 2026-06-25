using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.Logging;

namespace XuanYu.Engine.Editor.Windows.Panels.DebugDock;

/// <summary>底部日志面板。渲染诊断、RenderScene 和性能页已移除，避免 UI 热路径卡顿。</summary>
public sealed partial class DebugDockPanel : UserControl
{
    LogPanel? _logPanel;

    public LogPanel? LogPanel => _logPanel;

    public DebugDockPanel() { InitializeComponent(); CacheControls(); }

    void CacheControls()
    {
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
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
