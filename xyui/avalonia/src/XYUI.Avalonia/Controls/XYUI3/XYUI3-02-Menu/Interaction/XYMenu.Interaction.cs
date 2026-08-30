using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenu
{
    public void Open()
    {
        IsOpen = true; FocusedIndex = FirstEnabled();
        if (FocusedIndex >= 0 && Items[FocusedIndex] is XYMenuItem item) item.Focus();
    }
    public void Close()
    {
        if (!IsOpen && SelectedItem is null) return; IsOpen = false; FocusedIndex = -1; ClearSelection(); Closed?.Invoke(this, EventArgs.Empty);
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape) { Close(); e.Handled = true; return; }
        if (e.Key is Key.Up or Key.Down) { MoveFocus(e.Key == Key.Down ? 1 : -1); e.Handled = true; return; }
        if (e.Key == Key.Enter && FocusedItem() is { } item) { item.Activate(); e.Handled = true; return; }
        base.OnKeyDown(e);
    }
    void MoveFocus(int delta)
    {
        var count = Items.Count; if (count == 0) return; var index = FocusedIndex < 0 ? 0 : FocusedIndex;
        for (var i = 0; i < count; i++) { index = (index + delta + count) % count; if (Items[index] is XYMenuItem item && item.IsEnabled) { FocusedIndex = index; item.Focus(); return; } }
    }
    XYMenuItem? FocusedItem() => FocusedIndex >= 0 && FocusedIndex < Items.Count ? Items[FocusedIndex] as XYMenuItem : null;
    int FirstEnabled() => Items.Select((x, i) => (x, i)).Where(x => x.x is XYMenuItem item && item.IsEnabled).Select(x => x.i).DefaultIfEmpty(-1).First();
}
