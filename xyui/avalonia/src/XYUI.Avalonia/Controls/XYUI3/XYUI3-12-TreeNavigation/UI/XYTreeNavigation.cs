using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYTreeNavigation : Border
{
    public IReadOnlyList<XYTreeNode> Items { get; }

    public XYTreeNavigation(params XYTreeNode[] items)
    {
        Classes.Add("xyui-tree-navigation"); Items = items;
        var panel = new StackPanel();
        foreach (var item in items) panel.Children.Add(item);
        Child = panel;
    }
}
