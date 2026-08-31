using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNavigation
{
    public event EventHandler<XYTreeNode>? SelectionChanged;
    public XYTreeNode? FocusedNode { get; private set; }
    public XYTreeNode? SelectedNode => Items.FirstOrDefault(x => x.IsSelected);

    void Attach(XYTreeNode item)
    {
        item.SelectionRequested -= OnSelectionRequested; item.SelectionRequested += OnSelectionRequested;
        item.FocusRequested -= OnFocusRequested; item.FocusRequested += OnFocusRequested;
        item.ActivationRequested -= OnActivationRequested; item.ActivationRequested += OnActivationRequested;
        item.ExpansionChanged -= OnExpansionChanged; item.ExpansionChanged += OnExpansionChanged;
        item.NavigationRequested -= OnNavigationRequested; item.NavigationRequested += OnNavigationRequested;
    }

    void OnSelectionRequested(object? sender, EventArgs e)
    { if (sender is XYTreeNode item) { Focus(item); Select(item); } }
    void OnFocusRequested(object? sender, EventArgs e) { if (sender is XYTreeNode item) Focus(item); }
    void OnActivationRequested(object? sender, EventArgs e) { if (FocusedNode is not null) Select(FocusedNode); }

    void OnExpansionChanged(object? sender, EventArgs e) => Build();

    void OnNavigationRequested(object? sender, Key key)
    {
        if (sender is not XYTreeNode item) return;
        if (key == Key.Left) MoveLeft(item); else if (key == Key.Right) MoveRight(item);
        else MoveLinear(item, key == Key.Down ? 1 : -1);
    }

    public void Select(XYTreeNode item)
    {
        if (!Items.Contains(item)) return;
        foreach (var candidate in Items) candidate.IsSelected = ReferenceEquals(candidate, item);
        DeriveGuides(item); SelectionChanged?.Invoke(this, item);
    }
    public void Focus(XYTreeNode item)
    { if (!Items.Contains(item) || !VisibleItems.Contains(item)) return; foreach (var candidate in Items) candidate.IsFocusedNode = ReferenceEquals(candidate, item); FocusedNode = item; item.Focus(); }

    void MoveLinear(XYTreeNode item, int delta)
    {
        var visible = _panel.Children.OfType<XYTreeNode>().ToArray(); var index = Array.IndexOf(visible, item);
        if (index >= 0) Focus(visible[Math.Clamp(index + delta, 0, visible.Length - 1)]);
    }

    void MoveLeft(XYTreeNode item)
    {
        if (item.HasChildren && item.IsExpanded) { item.ToggleExpansion(); return; }
        var index = Items.ToList().IndexOf(item);
        for (var i = index - 1; i >= 0; i--) if (Items[i].Depth < item.Depth) { Focus(Items[i]); return; }
    }

    void MoveRight(XYTreeNode item)
    {
        if (item.HasChildren && !item.IsExpanded) { item.ToggleExpansion(); return; }
        var index = Items.ToList().IndexOf(item); if (index + 1 < Items.Count && Items[index + 1].Depth > item.Depth) Focus(Items[index + 1]);
    }
    void DeriveGuides(XYTreeNode selected)
    {
        var selectedIndex = Items.ToList().IndexOf(selected); var ancestors = new HashSet<int>(); var depth = selected.Depth - 1;
        for (var i = selectedIndex - 1; i >= 0 && depth >= 0; i--) if (Items[i].Depth == depth) { ancestors.Add(i); depth--; }
        for (var i = 0; i < Items.Count; i++) Items[i].ActiveGuideDepth = ancestors.Contains(i) || i == selectedIndex ? Items[i].Depth : 0;
    }
}
