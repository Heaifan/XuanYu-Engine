namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs
{
    void OnSelected(object? sender, EventArgs e) { if (sender is XYTab tab) SetSelected(tab); }
    void OnCloseRequested(object? sender, EventArgs e) { if (sender is XYTab tab) Close(tab); }
    public XYTab? SelectedItem => _items.FirstOrDefault(x => x.IsSelected);
    public void Select(XYTab tab)
    {
        if (!_items.Contains(tab)) return;
        var changed = !tab.IsSelected;
        foreach (var item in _items) if (item.IsSelected != ReferenceEquals(item, tab)) item.IsSelected = ReferenceEquals(item, tab);
        if (changed) SelectionChanged?.Invoke(this, tab);
    }
    public void Add(XYTab tab, bool select = false)
    {
        if (_items.Contains(tab)) return;
        _items.Add(tab); Build(); if (select || _items.Count == 1) Select(tab);
    }
    void SetSelected(XYTab tab) => Select(tab);
    public void Close(XYTab tab)
    {
        var index = _items.IndexOf(tab); if (index < 0) return;
        var wasSelected = tab.IsSelected; _items.RemoveAt(index); tab.SelectionRequested -= OnSelected; tab.CloseRequested -= OnCloseRequested; tab.PropertyChanged -= OnTabPropertyChanged;
        if (wasSelected && _items.Count > 0) SetSelected(_items[Math.Min(index, _items.Count - 1)]);
        Build(); TabClosed?.Invoke(this, tab);
    }
    public void CloseAll() { foreach (var tab in _items.ToArray()) Close(tab); }
}
