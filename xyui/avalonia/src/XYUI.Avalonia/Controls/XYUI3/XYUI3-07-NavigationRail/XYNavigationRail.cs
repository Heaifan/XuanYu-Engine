using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYNavigationRail : Border
{
    readonly StackPanel _panel = new() { Spacing = 4 };
    readonly XYSubMenu _contextFlyout;
    public IReadOnlyList<XYNavigationItem> Items { get; }
    public IReadOnlyList<XYNavigationItem> ContextItems { get; }
    public XYSubMenu ContextFlyout => _contextFlyout;
    public XYNavigationRail(IReadOnlyList<XYNavigationItem> items)
    {
        Items = items; ContextItems = []; Classes.Add("xyui-navigation-rail"); Child = _panel;
        foreach (var item in items) Attach(item);
        _contextFlyout = new XYSubMenu { ShowParentMenu = false }; _contextFlyout.Close();
    }
    public XYNavigationRail(IReadOnlyList<XYNavigationItem> items, IReadOnlyList<XYNavigationItem> contextItems) : this(items)
    {
        ContextItems = contextItems; _contextFlyout.ParentMenu = new XYMenu(items.Select(x => new XYMenuItem { Label = x.Label, HasSubMenu = true }).ToArray());
        _contextFlyout.ChildMenu = new XYMenu(contextItems.Select(x => new XYMenuItem { Label = x.Label }).ToArray());
        _contextFlyout.Close();
        Child = new Grid { ColumnDefinitions = new ColumnDefinitions("54,*"), Children = { _panel, _contextFlyout } }; Grid.SetColumn(_contextFlyout, 1);
    }
    public XYNavigationRail(params XYNavigationItem[] items) : this((IReadOnlyList<XYNavigationItem>)items) { }
    void Attach(XYNavigationItem item) { item.Classes.Add("xyui-rail-item"); item.Selected += OnSelected; _panel.Children.Add(item); }
    void OnSelected(object? sender, EventArgs e) { if (ContextItems.Count > 0) _contextFlyout.Open(); }
}
