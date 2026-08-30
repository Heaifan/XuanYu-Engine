using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYNavigationRail : Border
{
    readonly StackPanel _panel = new() { Spacing = 4 };
    public IReadOnlyList<XYNavigationItem> Items { get; }
    public XYNavigationRail(IReadOnlyList<XYNavigationItem> items)
    {
        Items = items; Classes.Add("xyui-navigation-rail"); Child = _panel;
        foreach (var item in items) { item.Classes.Add("xyui-rail-item"); _panel.Children.Add(item); }
    }
    public XYNavigationRail(params XYNavigationItem[] items) : this((IReadOnlyList<XYNavigationItem>)items) { }
}
