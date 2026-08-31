using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationMenu : Border
{
    IReadOnlyList<XYNavigationGroup> _groups = [];
    readonly StackPanel _panel = new() { Spacing = 4 };
    readonly XYNavigationState? _state;
    public IReadOnlyList<XYNavigationGroup> Groups { get => _groups; set { _groups = value; Build(); } }
    string? _selectedId;
    public string? SelectedId { get => _selectedId; set { if (_selectedId == value) return; _selectedId = value; Build(); } }
    public XYNavigationMenu() { Classes.Add("xyui-navigation-menu"); Child = _panel; }
    public XYNavigationMenu(params XYNavigationGroup[] groups) : this() => Groups = groups;
    public XYNavigationMenu(XYNavigationState state) : this() { _state = state; _state.Changed += OnStateChanged; Build(); }
    void Build()
    {
        _panel.Children.Clear();
        var groups = _state is null ? Groups : [new XYNavigationGroup("", _state.Entries.Select(Create).ToArray())];
        foreach (var group in groups)
        {
            if (_panel.Children.Count > 0) _panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-navigation-separator" } });
            if (!string.IsNullOrEmpty(group.Label)) _panel.Children.Add(new TextBlock { Text = group.Label, Classes = { "xyui-navigation-group" } });
            foreach (var item in _state is null ? group.Items : _state.Entries.Select(Create)) Attach(item);
        }
    }
    XYNavigationItem Create(XYNavigationEntry entry) => new() { Id = entry.Id, Label = entry.Label, Icon = entry.Icon };
    void Attach(XYNavigationItem item)
    {
        item.IsSelected = item.Id == (_state?.SelectedId ?? SelectedId); item.Selected -= OnSelected; item.Selected += OnSelected; _panel.Children.Add(item);
    }
    void OnStateChanged(object? sender, EventArgs e) { foreach (var item in _panel.Children.OfType<XYNavigationItem>()) item.IsSelected = item.Id == _state?.SelectedId; }
    public static XYNavigationGroup Group(string label, params XYNavigationItem[] items) => new(label, items);
}

public sealed record XYNavigationGroup(string Label, IReadOnlyList<XYNavigationItem> Items);
