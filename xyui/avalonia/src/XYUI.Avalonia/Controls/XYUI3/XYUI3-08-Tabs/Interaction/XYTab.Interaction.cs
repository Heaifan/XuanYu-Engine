using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTab
{
    void Select() => SelectionRequested?.Invoke(this, EventArgs.Empty);
    void RequestClose() => CloseRequested?.Invoke(this, EventArgs.Empty);
    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed) return;
        if ((e.Source as Visual)?.FindAncestorOfType<XYIconButton>() is not null) { RequestClose(); e.Handled = true; return; }
        Select(); e.Handled = true;
    }
}
