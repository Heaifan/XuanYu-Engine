using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.Logging;

namespace FluidWarfare.Editor.Windows.Panels.DebugDock;

/// <summary>
/// 底部调试终端，包含日志、渲染诊断、RenderScene 和性能页签。
/// 不创建日志，不创建 Vulkan 对象，不读取 WorldState。
/// </summary>
public sealed partial class DebugDockPanel : UserControl
{
    private LogPanel? _logPanel;

    // 渲染诊断
    private SelectableTextBlock?_diagLoader;
    private SelectableTextBlock?_diagInstance;
    private SelectableTextBlock?_diagDevice;
    private SelectableTextBlock?_diagNativeHost;
    private SelectableTextBlock?_diagSurface;
    private SelectableTextBlock?_diagSwapchain;
    private SelectableTextBlock?_diagClear;
    private SelectableTextBlock?_diagMarker;
    private SelectableTextBlock?_diagValidation;
    private SelectableTextBlock?_diagScene3d;
    private SelectableTextBlock?_diagCamera;
    private SelectableTextBlock?_diagGrid;
    private SelectableTextBlock?_diagUnit;
    private SelectableTextBlock?_diagDrawCall;

    // RenderScene
    private SelectableTextBlock?_renderSceneTitle;
    private SelectableTextBlock?_renderSceneEmpty;
    private StackPanel? _renderSceneList;

    // 性能
    private SelectableTextBlock?_perfInstance;
    private SelectableTextBlock?_perfDevice;
    private SelectableTextBlock?_perfSwapchain;
    private SelectableTextBlock?_perfClear;
    private SelectableTextBlock?_perfMarker;
    private SelectableTextBlock?_perfScene3d;

    private Separator? _scene3dRunSeparator;
    private Button? _scene3dRunButton;

    // 公开属性：让 EditorShell 可以访问 LogPanel
    public LogPanel? LogPanel => _logPanel;

    /// <summary>
    /// 手动触发 Scene3D 运行请求。
    /// </summary>
    public event EventHandler? Scene3dRunRequested;

    /// <summary>
    /// 启用或禁用 Scene3D 运行按钮。
    /// </summary>
    public bool Scene3dRunButtonEnabled
    {
        set
        {
            if (_scene3dRunButton is not null)
            {
                _scene3dRunButton.IsEnabled = value;
                _scene3dRunButton.IsVisible = value;
            }

            if (_scene3dRunSeparator is not null)
            {
                _scene3dRunSeparator.IsVisible = value;
            }
        }
    }

    public DebugDockPanel()
    {
        InitializeComponent();
        CacheControls();
    }

    private void HandleScene3dRunButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Scene3dRunRequested?.Invoke(this, EventArgs.Empty);
    }

    private void CacheControls()
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
        _perfScene3d = this.FindControl<SelectableTextBlock>("PerfScene3d");
        _scene3dRunSeparator = this.FindControl<Separator>("Scene3dRunSeparator");
        _scene3dRunButton = this.FindControl<Button>("Scene3dRunButton");
    }

    // ─── 渲染诊断 ──────────────────────────────────────────────

    public void SetDiagnostics(string loader, string instance, string device, string nativeHost,
        string surface, string swapchain, string clear, string marker, string validation)
    {
        if (_diagLoader is not null) _diagLoader.Text = $"Vulkan 后端：{loader}";
        if (_diagInstance is not null) _diagInstance.Text = $"Instance：{instance}";
        if (_diagDevice is not null) _diagDevice.Text = $"Device：{device}";
        if (_diagNativeHost is not null) _diagNativeHost.Text = $"Native Host：{nativeHost}";
        if (_diagSurface is not null) _diagSurface.Text = $"Surface：{surface}";
        if (_diagSwapchain is not null) _diagSwapchain.Text = $"Swapchain：{swapchain}";
        if (_diagClear is not null) _diagClear.Text = $"Clear：{clear}";
        if (_diagMarker is not null) _diagMarker.Text = $"Marker：{marker}";
        if (_diagValidation is not null) _diagValidation.Text = $"Validation：{validation}";
    }

    public void SetScene3d(string scene3d, string camera, string grid, string unit, string drawCall)
    {
        if (_diagScene3d is not null) _diagScene3d.Text = $"Scene3D：{scene3d}";
        if (_diagCamera is not null) _diagCamera.Text = $"Camera：{camera}";
        if (_diagGrid is not null) _diagGrid.Text = $"Grid：{grid}";
        if (_diagUnit is not null) _diagUnit.Text = $"Unit：{unit}";
        if (_diagDrawCall is not null) _diagDrawCall.Text = $"DrawCall：{drawCall}";
    }

    // ─── RenderScene 列表 ──────────────────────────────────────

    public void SetRenderSceneSummary(string title, IReadOnlyList<string> entries)
    {
        if (_renderSceneTitle is not null) _renderSceneTitle.Text = title;
        if (_renderSceneEmpty is null || _renderSceneList is null) return;

        if (entries.Count == 0)
        {
            _renderSceneEmpty.IsVisible = true;
            _renderSceneList.IsVisible = false;
            return;
        }

        _renderSceneEmpty.IsVisible = false;
        _renderSceneList.IsVisible = true;
        _renderSceneList.Children.Clear();

        foreach (var entry in entries)
        {
            _renderSceneList.Children.Add(new SelectableTextBlock
            {
                Text = entry,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#C9D1D9")),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
        }
    }

    // ─── 性能计时 ──────────────────────────────────────────────

    public void SetPerformance(string instanceMs, string deviceMs, string swapchainMs, string clearMs, string markerMs, string scene3dMs)
    {
        if (_perfInstance is not null) _perfInstance.Text = $"Instance：{instanceMs} ms";
        if (_perfDevice is not null) _perfDevice.Text = $"Device：{deviceMs} ms";
        if (_perfSwapchain is not null) _perfSwapchain.Text = $"Swapchain：{swapchainMs} ms";
        if (_perfClear is not null) _perfClear.Text = $"Clear：{clearMs} ms";
        if (_perfMarker is not null) _perfMarker.Text = $"MarkerDraw：{markerMs} ms";
        if (_perfScene3d is not null) _perfScene3d.Text = $"Scene3D：{scene3dMs} ms";
    }
}
