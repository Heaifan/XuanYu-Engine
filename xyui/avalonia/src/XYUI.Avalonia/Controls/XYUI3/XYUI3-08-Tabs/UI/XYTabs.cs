using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    readonly List<XYTab> _items = [];
    public IReadOnlyList<XYTab> Items => _items;
    public event EventHandler<XYTab>? TabClosed;
    public XYTabs() { Classes.Add("xyui-tabs"); Child = _panel; }
    public XYTabs(params XYTab[] items) : this() { _items.AddRange(items); Build(); }
    public void Build()
    {
        _panel.Children.Clear();
        foreach (var item in Items) { item.Selected -= OnSelected; item.Selected += OnSelected; item.CloseRequested -= OnCloseRequested; item.CloseRequested += OnCloseRequested; _panel.Children.Add(item); }
    }
}
