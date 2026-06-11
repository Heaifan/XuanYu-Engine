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

    // RenderScene
    private TextBlock? _renderSceneTitle;
    private TextBlock? _renderSceneEmpty;
    private StackPanel? _renderSceneList;

    // 性能
    private TextBlock? _perfInstance;
    private TextBlock? _perfDevice;
    private TextBlock? _perfSwapchain;
    private TextBlock? _perfClear;

    // 公开属性：让 EditorShell 可以访问 LogPanel
    public LogPanel? LogPanel => _logPanel;

    public DebugDockPanel()
    {
        InitializeComponent();
        CacheControls();
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
        _renderSceneTitle = this.FindControl<TextBlock>("RenderSceneTitle");
        _renderSceneEmpty = this.FindControl<TextBlock>("RenderSceneEmpty");
        _renderSceneList = this.FindControl<StackPanel>("RenderSceneList");
        _perfInstance = this.FindControl<TextBlock>("PerfInstance");
        _perfDevice = this.FindControl<TextBlock>("PerfDevice");
        _perfSwapchain = this.FindControl<TextBlock>("PerfSwapchain");
        _perfClear = this.FindControl<TextBlock>("PerfClear");
    }

    // ─── 渲染诊断 ──────────────────────────────────────────────

    public void SetDiagnostics(string loader, string instance, string device, string nativeHost,
        string surface, string swapchain, string clear)
    {
        if (_diagLoader is not null) _diagLoader.Text = $"Vulkan 后端：{loader}";
        if (_diagInstance is not null) _diagInstance.Text = $"Instance：{instance}";
        if (_diagDevice is not null) _diagDevice.Text = $"Device：{device}";
        if (_diagNativeHost is not null) _diagNativeHost.Text = $"Native Host：{nativeHost}";
        if (_diagSurface is not null) _diagSurface.Text = $"Surface：{surface}";
        if (_diagSwapchain is not null) _diagSwapchain.Text = $"Swapchain：{swapchain}";
        if (_diagClear is not null) _diagClear.Text = $"Clear：{clear}";
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

    public void SetPerformance(string instanceMs, string deviceMs, string swapchainMs, string clearMs)
    {
        if (_perfInstance is not null) _perfInstance.Text = $"Instance：{instanceMs} ms";
        if (_perfDevice is not null) _perfDevice.Text = $"Device：{deviceMs} ms";
        if (_perfSwapchain is not null) _perfSwapchain.Text = $"Swapchain：{swapchainMs} ms";
        if (_perfClear is not null) _perfClear.Text = $"Clear：{clearMs} ms";
    }
}
