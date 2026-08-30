using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYTabs : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    public IReadOnlyList<XYTab> Items { get; set; } = [];
    public XYTabs() { Classes.Add("xyui-tabs"); Child = _panel; }
    public XYTabs(params XYTab[] items) : this() { Items = items; Build(); }
    public void Build()
    {
        _panel.Children.Clear();
        foreach (var item in Items) { item.Selected -= OnSelected; item.Selected += OnSelected; _panel.Children.Add(item); }
    }
    void OnSelected(object? sender, EventArgs e) { foreach (var item in Items) item.IsSelected = ReferenceEquals(item, sender); }
}
