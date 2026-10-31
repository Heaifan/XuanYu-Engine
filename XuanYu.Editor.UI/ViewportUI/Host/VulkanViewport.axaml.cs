using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class VulkanViewport : UserControl
{
    VulkanNativeHost? _host;
    bool _hostHooked;

    public VulkanViewport()
    {
        InitializeComponent();
        AttachedToVisualTree += OnAttached;
    }

    void OnAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        SetFallback("Vulkan 正在初始化...");
        _host ??= this.GetLogicalDescendants().OfType<VulkanNativeHost>().Single();
        if (!_hostHooked) { _host.RendererReady += (_, _) => HideFallback(); _hostHooked = true; }
        if (_host.IsRendererReady) HideFallback();
    }

    internal void SetFallback(string message)
    {
        FallbackLayer.IsVisible = true;
        FallbackText.Text = message;
    }

    internal void HideFallback() => FallbackLayer.IsVisible = false;
}
