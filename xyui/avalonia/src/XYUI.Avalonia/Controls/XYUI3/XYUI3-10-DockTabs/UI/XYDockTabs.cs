using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTabs : Border
{
    readonly List<XYDockTab> _items = [];
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    public IReadOnlyList<XYDockTab> Items => _items;

    public XYDockTabs(params XYDockTab[] items)
    {
        Classes.Add("xyui-dock-tabs"); _items.AddRange(items); Child = _panel;
        if (_items.Count > 0 && !_items.Any(x => x.Tab.IsSelected)) _items[0].Tab.IsSelected = true;
        Build();
    }

    void Build()
    {
        _panel.Children.Clear();
        foreach (var item in _items) { Attach(item); _panel.Children.Add(item); }
    }
}
