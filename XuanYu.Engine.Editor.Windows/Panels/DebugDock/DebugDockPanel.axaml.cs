using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.Logging;

using XuanYu.Engine.Editor.Windows.Panels.Logging;
namespace XuanYu.Engine.Editor.Windows.Panels.DebugDock;

/// <summary>底部调试终端，包含日志和调试两个页签；调试页内嵌渲染诊断、RenderScene 和性能。</summary>
public sealed partial class DebugDockPanel : UserControl
{
    LogPanel? _logPanel;

    // 渲染诊断
    SelectableTextBlock? _diagLoader, _diagInstance, _diagDevice, _diagNativeHost, _diagSurface;
    SelectableTextBlock? _diagSwapchain, _diagClear, _diagMarker, _diagValidation, _diagScene3d;
    SelectableTextBlock? _diagCamera, _diagGrid, _diagUnit, _diagDrawCall;

    // RenderScene
    SelectableTextBlock? _renderSceneTitle, _renderSceneEmpty;
    StackPanel? _renderSceneList;

    // 性能
    SelectableTextBlock? _perfInstance, _perfDevice, _perfSwapchain, _perfClear, _perfMarker, _perfScene3d;

    public LogPanel? LogPanel => _logPanel;

    public DebugDockPanel() { InitializeComponent(); CacheControls(); }

    void CacheControls()
    {
        _logPanel = this.FindControl<LogPanel>("EditorLogPanel");
        _diagLoader = this.FindControl<SelectableTextBlock>("DiagVulkanLoader");
        _diagInstance = this.FindControl<SelectableTextBlock>("DiagVulkanInstance");
        _diagDevice = this.FindControl<SelectableTextBlock>("DiagVulkanDevice");
        _diagNativeHost = this.FindControl<SelectableTextBlock>("DiagNativeHost");
        _diagSurface = this.FindControl<SelectableTextBlock>("DiagSurface");
        _diagSwapchain = this.FindControl<SelectableTextBlock>("DiagSwapchain");
        _diagClear = this.FindControl<SelectableTextBlock>("DiagClear");
        _diagMarker = this.FindControl<SelectableTextBlock>("DiagMarker");
        _diagValidation = this.FindControl<SelectableTextBlock>("DiagValidation");
        _diagScene3d = this.FindControl<SelectableTextBlock>("DiagScene3d");
        _diagCamera = this.FindControl<SelectableTextBlock>("DiagCamera");
        _diagGrid = this.FindControl<SelectableTextBlock>("DiagGrid");
        _diagUnit = this.FindControl<SelectableTextBlock>("DiagUnit");
        _diagDrawCall = this.FindControl<SelectableTextBlock>("DiagDrawCall");
        _renderSceneTitle = this.FindControl<SelectableTextBlock>("RenderSceneTitle");
        _renderSceneEmpty = this.FindControl<SelectableTextBlock>("RenderSceneEmpty");
        _renderSceneList = this.FindControl<StackPanel>("RenderSceneList");
        _perfInstance = this.FindControl<SelectableTextBlock>("PerfInstance");
        _perfDevice = this.FindControl<SelectableTextBlock>("PerfDevice");
        _perfSwapchain = this.FindControl<SelectableTextBlock>("PerfSwapchain");
        _perfClear = this.FindControl<SelectableTextBlock>("PerfClear");
        _perfMarker = this.FindControl<SelectableTextBlock>("PerfMarker");
        _perfScene3d = this.FindControl<SelectableTextBlock>("PerfScene3d"); }
}
