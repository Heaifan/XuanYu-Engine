namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs
{
    void OnSelected(object? sender, EventArgs e) { if (sender is XYTab tab) SetSelected(tab); }
    void OnCloseRequested(object? sender, EventArgs e) { if (sender is XYTab tab) Close(tab); }
    public XYTab? SelectedItem => _items.FirstOrDefault(x => x.IsSelected);
    public void Select(XYTab tab) { if (!_items.Contains(tab)) return; foreach (var item in _items) item.IsSelected = ReferenceEquals(item, tab); }
    void SetSelected(XYTab tab) => Select(tab);
    public void Close(XYTab tab)
    {
        var index = _items.IndexOf(tab); if (index < 0) return;
        var wasSelected = tab.IsSelected; _items.RemoveAt(index); tab.SelectionRequested -= OnSelected; tab.CloseRequested -= OnCloseRequested;
        if (wasSelected && _items.Count > 0) SetSelected(_items[Math.Min(index, _items.Count - 1)]);
        Build(); TabClosed?.Invoke(this, tab);
    }
    public void CloseAll() { foreach (var tab in _items.ToArray()) Close(tab); }
}
