using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class Top : UserControl
{
    public Top()
    {
        InitializeComponent();
        ContextToolScrollHost.AddHandler(
            InputElement.PointerWheelChangedEvent, OnContextWheel, RoutingStrategies.Tunnel);
    }

    void OnContextWheel(object? sender, PointerWheelEventArgs e)
    {
        var host = ContextToolScrollHost;
        var max = Math.Max(0, host.Extent.Width - host.Viewport.Width);
        if (max <= 0) return;
        var offset = Math.Clamp(host.Offset.X + e.Delta.Y * 96, 0, max);
        host.Offset = new Vector(offset, 0);
        e.Handled = true;
    }
}
