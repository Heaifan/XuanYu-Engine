using Avalonia.Controls;
using Avalonia.Input;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class ViewportPlaceholderPanel : UserControl
{
    public event EventHandler? ViewportFocused;

    public ViewportPlaceholderPanel()
    {
        InitializeComponent();
    }

    private void HandleViewportPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewportFocused?.Invoke(this, EventArgs.Empty);
    }
}
