namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs
{
    void OnSelected(object? sender, EventArgs e) { foreach (var item in Items) item.IsSelected = ReferenceEquals(item, sender); }
    void OnCloseRequested(object? sender, EventArgs e) { if (sender is XYTab tab) Close(tab); }
    public void Close(XYTab tab) { if (!_items.Remove(tab)) return; tab.Selected -= OnSelected; tab.CloseRequested -= OnCloseRequested; Build(); TabClosed?.Invoke(this, tab); }
    public void CloseAll() { foreach (var tab in _items.ToArray()) Close(tab); }
}
