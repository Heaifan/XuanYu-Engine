using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYDockTabs : Border
{
    public IReadOnlyList<XYDockTab> Items { get; }

    public XYDockTabs(params XYDockTab[] items)
    {
        Classes.Add("xyui-dock-tabs"); Items = items;
        Child = new StackPanel { Orientation = Orientation.Horizontal, Children = { } };
        var panel = (StackPanel)Child;
        foreach (var item in items) panel.Children.Add(item);
    }
}
