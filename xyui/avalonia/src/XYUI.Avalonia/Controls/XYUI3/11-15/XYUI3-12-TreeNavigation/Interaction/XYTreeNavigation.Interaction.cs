using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNavigation
{
    public event EventHandler<XYTreeNode>? SelectionChanged;
    public XYTreeNode? FocusedNode { get; private set; }
    public XYTreeNode? SelectedNode => AllNodes().FirstOrDefault(x => x.IsSelected);
    void Attach(XYTreeNode item)
    { item.SelectionRequested -= OnSelectionRequested; item.SelectionRequested += OnSelectionRequested; item.FocusRequested -= OnFocusRequested; item.FocusRequested += OnFocusRequested; item.ActivationRequested -= OnActivationRequested; item.ActivationRequested += OnActivationRequested; item.ExpansionChanged -= OnExpansionChanged; item.ExpansionChanged += OnExpansionChanged; item.NavigationRequested -= OnNavigationRequested; item.NavigationRequested += OnNavigationRequested; }
    void OnSelectionRequested(object? sender, EventArgs e) { if (sender is XYTreeNode item) { Focus(item); Select(item); } }
    void OnFocusRequested(object? sender, EventArgs e) { if (sender is XYTreeNode item) Focus(item); }
    void OnActivationRequested(object? sender, EventArgs e) { if (FocusedNode is not null) Select(FocusedNode); }
    void OnExpansionChanged(object? sender, EventArgs e) => Build();
    void OnNavigationRequested(object? sender, Key key)
    { if (sender is not XYTreeNode item) return; if (key == Key.Left) MoveLeft(item); else if (key == Key.Right) MoveRight(item); else if (key is Key.Home or Key.End) { var nodes = VisibleItems.Where(x => x.IsEnabled).ToArray(); if (nodes.Length > 0) Focus(key == Key.Home ? nodes[0] : nodes[^1]); } else MoveLinear(item, key == Key.Down ? 1 : -1); }
    public void Select(XYTreeNode item)
    { if (!AllNodes().Contains(item) || !item.IsEnabled) return; foreach (var candidate in AllNodes()) candidate.IsSelected = ReferenceEquals(candidate, item); DeriveGuides(item); SelectionChanged?.Invoke(this, item); }
    public void Focus(XYTreeNode item) { if (!AllNodes().Contains(item) || !VisibleItems.Contains(item) || !item.IsEnabled) return; foreach (var candidate in AllNodes()) candidate.IsFocusedNode = ReferenceEquals(candidate, item); FocusedNode = item; item.Focus(); }
    void MoveLinear(XYTreeNode item, int delta) { var visible = VisibleItems.Where(x => x.IsEnabled).ToArray(); var index = Array.IndexOf(visible, item); if (index >= 0) Focus(visible[Math.Clamp(index + delta, 0, visible.Length - 1)]); }
    void MoveLeft(XYTreeNode item) { if (item.HasChildren && item.IsExpanded) { item.ToggleExpansion(); return; } var parent = FindParent(item); if (parent is not null) Focus(parent); }
    void MoveRight(XYTreeNode item) { if (item.HasChildren && !item.IsExpanded) { item.ToggleExpansion(); return; } var child = item.Children.FirstOrDefault(x => x.IsEnabled); if (child is not null) Focus(child); }
    XYTreeNode? FindParent(XYTreeNode target) => Items.SelectMany(x => FindParent(x, target)).FirstOrDefault();
    static IEnumerable<XYTreeNode> FindParent(XYTreeNode node, XYTreeNode target) { if (node.Children.Contains(target)) yield return node; foreach (var child in node.Children) foreach (var parent in FindParent(child, target)) yield return parent; }
    void DeriveGuides(XYTreeNode selected) { var all = AllNodes().ToArray(); var index = Array.IndexOf(all, selected); var ancestors = new HashSet<XYTreeNode>(); var parent = FindParent(selected); while (parent is not null) { ancestors.Add(parent); parent = FindParent(parent); } for (var i = 0; i < all.Length; i++) all[i].ActiveGuideDepth = ancestors.Contains(all[i]) || i == index ? all[i].Depth : 0; }
    IEnumerable<XYTreeNode> AllNodes() => Items.SelectMany(AllNodes);
    static IEnumerable<XYTreeNode> AllNodes(XYTreeNode node) { yield return node; foreach (var child in node.Children) foreach (var nested in AllNodes(child)) yield return nested; }
}
