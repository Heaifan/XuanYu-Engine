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
    private TextBlock? _diagLoader;
    private TextBlock? _diagInstance;
    private TextBlock? _diagDevice;
    private TextBlock? _diagNativeHost;
    private TextBlock? _diagSurface;
    private TextBlock? _diagSwapchain;
    private TextBlock? _diagClear;
    private TextBlock? _diagMarker;
    private TextBlock? _diagScene3d;
    private TextBlock? _diagCamera;
    private TextBlock? _diagGrid;
    private TextBlock? _diagUnit;
    private TextBlock? _diagDrawCall;

    // RenderScene
    private TextBlock? _renderSceneTitle;
    private TextBlock? _renderSceneEmpty;
    private StackPanel? _renderSceneList;

    // 性能
    private TextBlock? _perfInstance;
    private TextBlock? _perfDevice;
    private TextBlock? _perfSwapchain;
    private TextBlock? _perfClear;
    private TextBlock? _perfMarker;
    private TextBlock? _perfScene3d;

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
        _diagLoader = this.FindControl<TextBlock>("DiagVulkanLoader");
        _diagInstance = this.FindControl<TextBlock>("DiagVulkanInstance");
        _diagDevice = this.FindControl<TextBlock>("DiagVulkanDevice");
        _diagNativeHost = this.FindControl<TextBlock>("DiagNativeHost");
        _diagSurface = this.FindControl<TextBlock>("DiagSurface");
        _diagSwapchain = this.FindControl<TextBlock>("DiagSwapchain");
        _diagClear = this.FindControl<TextBlock>("DiagClear");
        _diagMarker = this.FindControl<TextBlock>("DiagMarker");
        _diagScene3d = this.FindControl<TextBlock>("DiagScene3d");
        _diagCamera = this.FindControl<TextBlock>("DiagCamera");
        _diagGrid = this.FindControl<TextBlock>("DiagGrid");
        _diagUnit = this.FindControl<TextBlock>("DiagUnit");
        _diagDrawCall = this.FindControl<TextBlock>("DiagDrawCall");
        _renderSceneTitle = this.FindControl<TextBlock>("RenderSceneTitle");
        _renderSceneEmpty = this.FindControl<TextBlock>("RenderSceneEmpty");
        _renderSceneList = this.FindControl<StackPanel>("RenderSceneList");
        _perfInstance = this.FindControl<TextBlock>("PerfInstance");
        _perfDevice = this.FindControl<TextBlock>("PerfDevice");
        _perfSwapchain = this.FindControl<TextBlock>("PerfSwapchain");
        _perfClear = this.FindControl<TextBlock>("PerfClear");
        _perfMarker = this.FindControl<TextBlock>("PerfMarker");
        _perfScene3d = this.FindControl<TextBlock>("PerfScene3d");
        _scene3dRunButton = this.FindControl<Button>("Scene3dRunButton");
    }

    // ─── 渲染诊断 ──────────────────────────────────────────────

    public void SetDiagnostics(string loader, string instance, string device, string nativeHost,
        string surface, string swapchain, string clear, string marker)
    {
        if (_diagLoader is not null) _diagLoader.Text = $"Vulkan 后端：{loader}";
        if (_diagInstance is not null) _diagInstance.Text = $"Instance：{instance}";
        if (_diagDevice is not null) _diagDevice.Text = $"Device：{device}";
        if (_diagNativeHost is not null) _diagNativeHost.Text = $"Native Host：{nativeHost}";
        if (_diagSurface is not null) _diagSurface.Text = $"Surface：{surface}";
        if (_diagSwapchain is not null) _diagSwapchain.Text = $"Swapchain：{swapchain}";
        if (_diagClear is not null) _diagClear.Text = $"Clear：{clear}";
        if (_diagMarker is not null) _diagMarker.Text = $"Marker：{marker}";
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
            _renderSceneList.Children.Add(new TextBlock
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
