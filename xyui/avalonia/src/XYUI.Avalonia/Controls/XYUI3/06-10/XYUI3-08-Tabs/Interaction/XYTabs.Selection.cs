namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs
{
    void OnSelected(object? sender, EventArgs e) { if (sender is XYTab tab) Select(tab); }
    void OnCloseRequested(object? sender, EventArgs e) { if (sender is XYTab tab) Close(tab); }
    public void Select(XYTab tab) { if (Items.Contains(tab)) SetSelection(tab.Id, true); }
    public void Select(string? id) { if (id is not null) SetSelection(id, true); }
    public void Add(XYTab tab, bool select = false) { if (Items.Contains(tab)) return; Items.Add(tab); if (select || Items.Count == 1) Select(tab); }
    public void Close(XYTab tab)
    {
        var index = Items.IndexOf(tab); if (index < 0) return; var wasSelected = tab.Id == _selectedTabId; Items.RemoveAt(index);
        if (wasSelected) Select(Items.ElementAtOrDefault(Math.Min(index, Items.Count - 1))?.Id); Build(); TabClosed?.Invoke(this, tab);
    }
    public void CloseAll() { foreach (var tab in Items.ToArray()) Close(tab); }
}
