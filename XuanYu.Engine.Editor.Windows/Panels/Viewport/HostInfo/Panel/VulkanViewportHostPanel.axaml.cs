using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;
using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>Vulkan 视口宿主面板。持有原生子窗口，转发原始输入事件。</summary>
public sealed partial class VulkanViewportHostPanel : UserControl
{
    TextBlock? _nativeHostInfoText;
    WindowsVulkanViewportHostControl? _nativeHostControl;
    TextBlock? _clearStatusText;

    public event EventHandler<VulkanViewportNativeHostInfo>? NativeHostInfoChanged;
    public event Action<int, int, int>? RawPointerButtonDown, RawPointerButtonUp;
    public event Action<int, int>? RawPointerMoved;
    public event Action<int>? RawKeyDown, RawKeyUp;
    public event Action<int, int>? RawMouseWheel;
    public event Action? RawInputFocusLost;
    public event Action<int, int>? PickRequested;
    public event Func<int, int, ViewportSceneToolPressResult>? SceneToolPointerPressed;
    public event Action<int, int>? SceneToolPointerReleased;
    public event Func<int, int, ViewportNavigationPressResult>? NavigationPointerPressed;
    public event Func<int, int, bool>? NavigationPointerMoved;
    public event Action? NavigationPointerReleased, NavigationCaptureLost;
    public new event Action<int, int>? PointerMoved;
    public event Action? PointerLeft;

    public VulkanViewportHostPanel()
    {
        InitializeComponent();
        _nativeHostInfoText = this.FindControl<TextBlock>("NativeHostInfoText");
        _nativeHostControl = this.FindControl<WindowsVulkanViewportHostControl>("NativeHostControl");
        _clearStatusText = this.FindControl<TextBlock>("ClearStatusText");
        if (_nativeHostControl is not null) WireNativeHostEvents(_nativeHostControl);
    }

    public void RequestCapture() => _nativeHostControl?.RequestCapture();
    public void RequestReleaseCapture() => _nativeHostControl?.RequestReleaseCapture();
    public void RequestCancelToolCapture() => _nativeHostControl?.RequestCancelToolCapture();

    partial void WireNativeHostEvents(WindowsVulkanViewportHostControl ctl);
}
