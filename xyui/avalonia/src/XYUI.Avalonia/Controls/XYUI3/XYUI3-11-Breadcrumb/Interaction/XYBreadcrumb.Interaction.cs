using global::Avalonia.Controls;
using global::Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumb
{
    int _focusedIndex;
    public event EventHandler<XYBreadcrumbItem>? CurrentChanged;
    public event EventHandler<XYBreadcrumbItem>? DropdownRequested;

    void Attach(XYBreadcrumbItem item)
    {
        item.Invoked -= OnInvoked; item.Invoked += OnInvoked;
        item.DropdownRequested -= OnDropdownRequested; item.DropdownRequested += OnDropdownRequested;
        item.NavigationRequested -= OnNavigationRequested; item.NavigationRequested += OnNavigationRequested;
    }

    void OnInvoked(object? sender, EventArgs e)
    { if (sender is XYBreadcrumbItem item) Navigate(item); }

    void OnDropdownRequested(object? sender, EventArgs e)
    { if (sender is XYBreadcrumbItem item) { _focusedIndex = IndexOf(item); OpenPopup(item); DropdownRequested?.Invoke(this, item); } }

    void OnNavigationRequested(object? sender, Key key)
    {
        if (sender is not XYBreadcrumbItem item) return;
        var index = IndexOf(item); if (key is Key.Left) index--; else index++;
        if (key is Key.Down) { OpenPopup(item); return; }
        if (index >= 0 && index < Items.Count) { _focusedIndex = index; Items[index].Focus(); }
    }

    void OpenPopup(XYBreadcrumbItem item)
    {
        var labels = item.IsCollapsed ? item.HiddenPathOptions : item.DropdownOptions;
        if (labels.Count == 0) return;
        var menu = new XYMenu(labels.Select(x => (Control)new XYMenuItem { Label = x }).ToArray());
        foreach (var option in menu.Items.OfType<XYMenuItem>()) option.SelectionRequested += (_, _) => { item.Label = option.Label; Navigate(item); DropdownPopup.IsOpen = false; };
        DropdownPopup.Child = menu; DropdownPopup.PlacementTarget = item; DropdownPopup.IsOpen = true; menu.Open();
    }
    int IndexOf(XYBreadcrumbItem item) { for (var i = 0; i < Items.Count; i++) if (ReferenceEquals(Items[i], item)) return i; return -1; }

    public void Navigate(XYBreadcrumbItem item)
    {
        if (!Items.Contains(item) || item.IsCollapsed) return;
        foreach (var candidate in Items) candidate.IsCurrent = ReferenceEquals(candidate, item);
        CurrentChanged?.Invoke(this, item);
    }
}
