using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNode
{
    internal event EventHandler? SelectionRequested;
    internal event EventHandler? ExpansionChanged;
    internal event EventHandler<Key>? NavigationRequested;

    void InitializeInteraction() { PointerPressed += OnPointerPressed; KeyDown += OnKeyDown; }

    public void ToggleExpansion()
    {
        if (!HasChildren) return; IsExpanded = !IsExpanded; ExpansionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Select() => SelectionRequested?.Invoke(this, EventArgs.Empty);

    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Focus(); var chevronEdge = Depth * XyuiCompactNavigationTokens.TreeIndent + 19;
        if (HasChildren && e.GetPosition(this).X <= chevronEdge) ToggleExpansion(); else Select();
        e.Handled = true;
    }

    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) Select();
        else if (e.Key == Key.Space) { if (HasChildren) ToggleExpansion(); else Select(); }
        else if (e.Key is Key.Up or Key.Down or Key.Left or Key.Right) NavigationRequested?.Invoke(this, e.Key);
        else return;
        e.Handled = true;
    }
}
