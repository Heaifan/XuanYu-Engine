using Avalonia.Input;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public partial class XYSelect
{
    void OnSelectKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Space) { if (!IsDropDownOpen) IsDropDownOpen = true; else if (e.Key == Key.Enter) CommitCandidate(); e.Handled = true; return; }
        if (e.Key == Key.Escape && IsDropDownOpen) { IsDropDownOpen = false; e.Handled = true; return; }
        if (e.Key is Key.Up or Key.Down && IsDropDownOpen) { MoveCandidate(e.Key == Key.Down ? 1 : -1); e.Handled = true; }
    }

    void MoveCandidate(int delta)
    {
        if (ListPart is null || ListPart.ItemCount == 0) return;
        var index = ListPart.SelectedIndex < 0 ? (delta > 0 ? 0 : ListPart.ItemCount - 1) : Math.Clamp(ListPart.SelectedIndex + delta, 0, ListPart.ItemCount - 1);
        IsKeyboardNavigating = true; ListPart.SelectedIndex = index; IsKeyboardNavigating = false;
    }

    void CommitCandidate()
    {
        if (ListPart is not null && ListPart.SelectedIndex >= 0) { SelectedIndex = ListPart.SelectedIndex; IsDropDownOpen = false; }
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source is not XYSelect) return;
        if (ListPart is not null && !IsKeyboardNavigating && ListPart.SelectedIndex != SelectedIndex) ListPart.SelectedIndex = SelectedIndex;
        SyncParts();
    }
}
