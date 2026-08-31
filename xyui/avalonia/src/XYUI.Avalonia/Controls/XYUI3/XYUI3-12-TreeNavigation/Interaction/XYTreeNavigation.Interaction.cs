using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTreeNavigation
{
    public event EventHandler<XYTreeNode>? SelectionChanged;

    void Attach(XYTreeNode item)
    {
        item.SelectionRequested -= OnSelectionRequested; item.SelectionRequested += OnSelectionRequested;
        item.ExpansionChanged -= OnExpansionChanged; item.ExpansionChanged += OnExpansionChanged;
        item.NavigationRequested -= OnNavigationRequested; item.NavigationRequested += OnNavigationRequested;
    }

    void OnSelectionRequested(object? sender, EventArgs e)
    { if (sender is XYTreeNode item) Select(item); }

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
        item.Focus(); SelectionChanged?.Invoke(this, item);
    }

    void MoveLinear(XYTreeNode item, int delta)
    {
        var visible = _panel.Children.OfType<XYTreeNode>().ToArray(); var index = Array.IndexOf(visible, item);
        if (index >= 0) Select(visible[Math.Clamp(index + delta, 0, visible.Length - 1)]);
    }

    void MoveLeft(XYTreeNode item)
    {
        if (item.HasChildren && item.IsExpanded) { item.ToggleExpansion(); return; }
        var index = Items.ToList().IndexOf(item);
        for (var i = index - 1; i >= 0; i--) if (Items[i].Depth < item.Depth) { Select(Items[i]); return; }
    }

    void MoveRight(XYTreeNode item)
    {
        if (item.HasChildren && !item.IsExpanded) { item.ToggleExpansion(); return; }
        var index = Items.ToList().IndexOf(item); if (index + 1 < Items.Count && Items[index + 1].Depth > item.Depth) Select(Items[index + 1]);
    }
}
