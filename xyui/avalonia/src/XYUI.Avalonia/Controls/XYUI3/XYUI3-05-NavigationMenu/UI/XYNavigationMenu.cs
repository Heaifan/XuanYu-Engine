using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationMenu : Border
{
    IReadOnlyList<XYNavigationGroup> _groups = [];
    readonly StackPanel _panel = new() { Spacing = 4 };
    public IReadOnlyList<XYNavigationGroup> Groups { get => _groups; set { _groups = value; Build(); } }
    string? _selectedId;
    public string? SelectedId { get => _selectedId; set { if (_selectedId == value) return; _selectedId = value; Build(); } }
    public XYNavigationMenu() { Classes.Add("xyui-navigation-menu"); Child = _panel; }
    public XYNavigationMenu(params XYNavigationGroup[] groups) : this() => Groups = groups;
    void Build()
    {
        _panel.Children.Clear();
        foreach (var group in Groups)
        {
            if (_panel.Children.Count > 0) _panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-navigation-separator" } });
            if (!string.IsNullOrEmpty(group.Label)) _panel.Children.Add(new TextBlock { Text = group.Label, Classes = { "xyui-navigation-group" } });
            foreach (var item in group.Items) Attach(item);
        }
    }
    void Attach(XYNavigationItem item)
    {
        item.IsSelected = item.Id == SelectedId; item.Selected -= OnSelected; item.Selected += OnSelected; _panel.Children.Add(item);
    }
    public static XYNavigationGroup Group(string label, params XYNavigationItem[] items) => new(label, items);
}

public sealed record XYNavigationGroup(string Label, IReadOnlyList<XYNavigationItem> Items);
