namespace XYUI.Avalonia.Controls;

public sealed record XYDockHandoffRequest(string TabId, int FromIndex, int ToIndex);

public sealed partial class XYDockTabs
{
    public event EventHandler<XYDockTab>? TabClosed;
    public event EventHandler? OrderChanged;

    void Attach(XYDockTab item)
    {
        item.Tab.SelectionRequested -= OnSelectionRequested; item.Tab.SelectionRequested += OnSelectionRequested;
        item.Tab.CloseRequested -= OnCloseRequested; item.Tab.CloseRequested += OnCloseRequested;
        item.DropRequested -= OnDropRequested; item.DropRequested += OnDropRequested;
        item.DragMoved -= OnDragMoved; item.DragMoved += OnDragMoved; item.DragCanceled -= OnDragCanceled; item.DragCanceled += OnDragCanceled;
    }

    void OnSelectionRequested(object? sender, EventArgs e)
    { if (sender is XYTab tab) Select(tab); }

    void OnCloseRequested(object? sender, EventArgs e)
    {
        var item = Items.FirstOrDefault(x => ReferenceEquals(x.Tab, sender));
        if (item is not null) Close(item);
    }

    void OnDropRequested(object? sender, double x)
    {
        if (sender is not XYDockTab item) return;
        var target = Items.Count - 1; var edge = 0d;
        for (var index = 0; index < Items.Count; index++)
        { edge += Items[index].Bounds.Width; if (x < edge) { target = index; break; } }
        ClearIndicators(); Move(item, target);
    }

    void OnDragMoved(object? sender, double x)
    {
        if (sender is not XYDockTab item) return;
        var target = HitTarget(x, out var after); ClearIndicators(); if (target is not null && !ReferenceEquals(item, target)) target.SetDropIndicator(true, after);
    }
    void OnDragCanceled(object? sender, EventArgs e) => ClearIndicators();
    XYDockTab? HitTarget(double x, out bool after)
    {
        after = false; var edge = 0d;
        foreach (var item in Items) { var midpoint = edge + item.Bounds.Width / 2; if (x <= edge + item.Bounds.Width) { after = x > midpoint; return item; } edge += item.Bounds.Width; }
        return Items.LastOrDefault();
    }
    void ClearIndicators() { foreach (var item in Items) item.SetDropIndicator(false); }

    public void Select(XYTab tab)
    {
        var item = Items.FirstOrDefault(x => ReferenceEquals(x.Tab, tab)); if (item is null || !tab.IsEnabled) return;
        var changed = _activeTabId != tab.Id; _activeTabId = tab.Id;
        foreach (var candidate in Items) candidate.Tab.IsSelected = ReferenceEquals(candidate, item);
        if (changed) SelectionChanged?.Invoke(this, item);
    }

    public void Close(XYDockTab item)
    {
        var index = Items.IndexOf(item); if (index < 0) return; var selected = item.Tab.IsSelected;
        Detach(item); Items.RemoveAt(index);
        if (selected && Items.Count > 0) Select(Items[Math.Min(index, Items.Count - 1)].Tab); else if (selected) _activeTabId = null;
        TabClosed?.Invoke(this, item);
    }
    public void Select(string? id) { if (id is not null) Select(Items.FirstOrDefault(x => x.Tab.Id == id)?.Tab!); }

    public void Move(XYDockTab item, int targetIndex)
    {
        var source = Items.IndexOf(item); if (source < 0) return;
        targetIndex = Math.Clamp(targetIndex, 0, Items.Count - 1); if (source == targetIndex) return;
        Items.Move(source, targetIndex); OrderChanged?.Invoke(this, EventArgs.Empty); DockHandoffRequested?.Invoke(this, new(item.Tab.Id, source, targetIndex));
    }

    void Detach(XYDockTab item)
    { item.Tab.SelectionRequested -= OnSelectionRequested; item.Tab.CloseRequested -= OnCloseRequested; item.DropRequested -= OnDropRequested; item.DragMoved -= OnDragMoved; item.DragCanceled -= OnDragCanceled; }
}
