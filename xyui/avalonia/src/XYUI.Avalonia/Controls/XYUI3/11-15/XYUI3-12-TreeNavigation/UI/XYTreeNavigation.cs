using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNavigation : Border
{
    readonly StackPanel _panel = new();
    bool _building;
    [Content] public ObservableCollection<XYTreeNode> Items { get; } = [];
    public IReadOnlyList<XYTreeNode> VisibleItems => _panel.Children.OfType<XYTreeNode>().ToArray();
    public XYTreeNavigation() { Classes.Add("xyui-tree-navigation"); Child = _panel; Items.CollectionChanged += OnItemsChanged; }
    public XYTreeNavigation(params XYTreeNode[] items) : this() { foreach (var item in items) Items.Add(item); Build(); }
    void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) { if (!_building) Build(); }
    void Build()
    {
        _building = true; _panel.Children.Clear(); var roots = Items.ToArray(); var nested = roots.Any(x => x.Children.Count > 0); var nodes = nested ? Flatten(roots) : roots;
        var hiddenBelow = -1;
        foreach (var item in nodes) { Attach(item); if (!nested && hiddenBelow >= 0 && item.Depth > hiddenBelow) continue; if (!nested && item.Depth <= hiddenBelow) hiddenBelow = -1; _panel.Children.Add(item); if (!nested && item.HasChildren && !item.IsExpanded) hiddenBelow = item.Depth; }
        var initial = SelectedNode ?? VisibleItems.FirstOrDefault(); if (FocusedNode is null && initial is not null) Focus(initial); _building = false;
    }
    static IEnumerable<XYTreeNode> Flatten(IEnumerable<XYTreeNode> nodes, int depth = 0) { foreach (var node in nodes) { node.Depth = depth; yield return node; if (node.IsExpanded) foreach (var child in Flatten(node.Children, depth + 1)) yield return child; } }
}
