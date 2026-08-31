using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTab
{
    void Select() => SelectionRequested?.Invoke(this, EventArgs.Empty);
    void RequestClose() => CloseRequested?.Invoke(this, EventArgs.Empty);
    void OnClosePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint((Visual)sender!).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        RequestClose(); e.Handled = true;
    }
    void OnCloseKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space) { RequestClose(); e.Handled = true; }
    }
    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        if ((e.Source as Visual)?.FindAncestorOfType<XYIconButton>() is not null) return;
        Select(); e.Handled = true;
    }
}
