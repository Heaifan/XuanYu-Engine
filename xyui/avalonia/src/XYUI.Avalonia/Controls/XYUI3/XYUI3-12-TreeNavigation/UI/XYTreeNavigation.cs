using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNavigation : Border
{
    readonly StackPanel _panel = new();
    public IReadOnlyList<XYTreeNode> Items { get; }
    public IReadOnlyList<XYTreeNode> VisibleItems => _panel.Children.OfType<XYTreeNode>().ToArray();

    public XYTreeNavigation(params XYTreeNode[] items)
    {
        Classes.Add("xyui-tree-navigation"); Items = items;
        Child = _panel; Build();
    }

    void Build()
    {
        _panel.Children.Clear(); var hiddenBelow = -1;
        foreach (var item in Items)
        {
            Attach(item);
            if (hiddenBelow >= 0 && item.Depth > hiddenBelow) continue;
            hiddenBelow = -1; _panel.Children.Add(item);
            if (item.HasChildren && !item.IsExpanded) hiddenBelow = item.Depth;
        }
    }
}
