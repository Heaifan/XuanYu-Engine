using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYPasswordField
{
    void OnRevealKeyDown(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Space && IsEnabled) { SetRevealed(true); e.Handled = true; } }
    void OnRevealKeyUp(object? sender, KeyEventArgs e) { if (e.Key is Key.Enter or Key.Space) { ForceHidePassword(); e.Handled = true; } }
}
