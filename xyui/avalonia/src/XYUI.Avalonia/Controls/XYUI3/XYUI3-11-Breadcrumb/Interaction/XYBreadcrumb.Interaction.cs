namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumb
{
    public event EventHandler<XYBreadcrumbItem>? CurrentChanged;
    public event EventHandler<XYBreadcrumbItem>? DropdownRequested;

    void Attach(XYBreadcrumbItem item)
    {
        item.Invoked -= OnInvoked; item.Invoked += OnInvoked;
        item.DropdownRequested -= OnDropdownRequested; item.DropdownRequested += OnDropdownRequested;
    }

    void OnInvoked(object? sender, EventArgs e)
    { if (sender is XYBreadcrumbItem item) Navigate(item); }

    void OnDropdownRequested(object? sender, EventArgs e)
    { if (sender is XYBreadcrumbItem item) DropdownRequested?.Invoke(this, item); }

    public void Navigate(XYBreadcrumbItem item)
    {
        if (!Items.Contains(item) || item.IsCollapsed) return;
        foreach (var candidate in Items) candidate.IsCurrent = ReferenceEquals(candidate, item);
        CurrentChanged?.Invoke(this, item);
    }
}
