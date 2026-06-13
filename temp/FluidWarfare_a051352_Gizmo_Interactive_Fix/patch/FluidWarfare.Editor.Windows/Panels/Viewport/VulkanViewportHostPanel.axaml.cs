using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class VulkanViewportHostPanel : UserControl
{
    private TextBlock? _nativeHostInfoText;
    private WindowsVulkanViewportHostControl? _nativeHostControl;
    private TextBlock? _clearStatusText;

    public event EventHandler<VulkanViewportNativeHostInfo>? NativeHostInfoChanged;

    // 相机输入事件转发（来自 Win32 原生窗口消息）
    public event Action<float, float>? CameraOrbitRequested;
    public event Action<int, int, int, int>? CameraPanRequested;
    public event Action<float>? CameraDollyRequested;
    public event Action<float>? CameraZoomRequested;
    public event Action? CameraResetRequested;
    public event Action? CameraProjectionToggleRequested;
    public event Action? NumpadPeriodRequested;
    public event Action? EscapeRequested;
    public event Action<int, int>? PickRequested;

    // ─── Overlay 导航输入事件 ────────────────────────────────────
    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;
    public event Func<int, int, bool>? NavigationPointerMoved;
    public event Action? NavigationPointerReleased;
    public event Action? NavigationCaptureLost;

    /// <summary>鼠标在视口内移动（pixelX, pixelY）。</summary>
    public new event Action<int, int>? PointerMoved;

    /// <summary>鼠标离开视口。</summary>
    public event Action? PointerLeft;

    public VulkanViewportHostPanel()
    {
        InitializeComponent();
        _nativeHostInfoText = this.FindControl<TextBlock>("NativeHostInfoText");
        _nativeHostControl = this.FindControl<WindowsVulkanViewportHostControl>("NativeHostControl");
        _clearStatusText = this.FindControl<TextBlock>("ClearStatusText");

        if (_nativeHostControl is not null)
        {
            _nativeHostControl.HostInfoChanged += HandleHostInfoChanged;
            _nativeHostControl.CameraOrbitRequested += (dy, dp) => CameraOrbitRequested?.Invoke(dy, dp);
            _nativeHostControl.CameraPanRequested += (dx, dy, w, h) => CameraPanRequested?.Invoke(dx, dy, w, h);
            _nativeHostControl.CameraDollyRequested += p => CameraDollyRequested?.Invoke(p);
            _nativeHostControl.CameraZoomRequested += n => CameraZoomRequested?.Invoke(n);
            _nativeHostControl.CameraResetRequested += () => CameraResetRequested?.Invoke();
            _nativeHostControl.CameraProjectionToggleRequested += () => CameraProjectionToggleRequested?.Invoke();
            _nativeHostControl.NumpadPeriodRequested += () => NumpadPeriodRequested?.Invoke();
            _nativeHostControl.EscapeRequested += () => EscapeRequested?.Invoke();
            _nativeHostControl.PickRequested += (x, y) => PickRequested?.Invoke(x, y);
            _nativeHostControl.NavigationPointerPressed += (x, y) =>
                NavigationPointerPressed?.Invoke(x, y) ?? ViewportNavigationPressResult.NotHandled;
            _nativeHostControl.NavigationPointerMoved += (x, y) => NavigationPointerMoved?.Invoke(x, y) == true;
            _nativeHostControl.NavigationPointerReleased += () => NavigationPointerReleased?.Invoke();
            _nativeHostControl.NavigationCaptureLost += () => NavigationCaptureLost?.Invoke();
            _nativeHostControl.PointerMoved += (x, y) => PointerMoved?.Invoke(x, y);
            _nativeHostControl.PointerLeft += () => PointerLeft?.Invoke();
        }
    }

    /// <summary>
    /// 更新 Vulkan 清屏状态文本。
    /// 不创建 Vulkan 对象，不写日志。
    /// </summary>
    public void ShowClearStatus(string statusText)
    {
        if (_clearStatusText is not null)
        {
            _clearStatusText.Text = statusText;
        }
    }

    /// <summary>
    /// 返回 Vulkan Surface 所需的原生宿主句柄信息。
    /// </summary>
    public VulkanViewportNativeHostInfo GetNativeHostInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            return new VulkanViewportNativeHostInfo("非 Windows", false, "当前平台不支持 Windows Vulkan Surface 创建。", 0, 0, 0, 0);
        }

        var hostInfo = GetWindowsNativeHostInfo();
        if (!hostInfo.HasWindowHandle)
        {
            return new VulkanViewportNativeHostInfo(hostInfo.PlatformText, false, hostInfo.Message, 0, hostInfo.InstanceHandle, hostInfo.Width, hostInfo.Height);
        }

        return new VulkanViewportNativeHostInfo(
            "Windows",
            true,
            "已获取 Windows Vulkan 视口原生子窗口句柄。",
            hostInfo.WindowHandle,
            hostInfo.InstanceHandle,
            hostInfo.Width,
            hostInfo.Height);
    }

    /// <summary>
    /// 返回 Windows 原生子窗口宿主状态。
    /// </summary>
    public WindowsVulkanViewportHostInfo GetWindowsNativeHostInfo()
    {
        var hostInfo = _nativeHostControl?.GetHostInfo() ?? WindowsVulkanViewportHostInfo.NotCreated;
        if (_nativeHostInfoText is not null)
        {
            _nativeHostInfoText.Text = hostInfo.HasWindowHandle
                ? $"{hostInfo.Message} 平台：{hostInfo.PlatformText}。"
                : hostInfo.Message;
        }
        return hostInfo;
    }

    private void HandleHostInfoChanged(object? sender, WindowsVulkanViewportHostInfo hostInfo)
    {
        if (_nativeHostInfoText is not null)
        {
            _nativeHostInfoText.Text = hostInfo.HasWindowHandle
                ? $"{hostInfo.Message} 平台：{hostInfo.PlatformText}。"
                : hostInfo.Message;
        }

        NativeHostInfoChanged?.Invoke(this, GetNativeHostInfo());
    }
}
