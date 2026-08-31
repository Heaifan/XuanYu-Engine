namespace XYUI.Avalonia.Controls;

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
        var item = _items.FirstOrDefault(x => ReferenceEquals(x.Tab, sender));
        if (item is not null) Close(item);
    }

    void OnDropRequested(object? sender, double x)
    {
        if (sender is not XYDockTab item) return;
        var target = _items.Count - 1; var edge = 0d;
        for (var index = 0; index < _items.Count; index++)
        { edge += _items[index].Bounds.Width; if (x < edge) { target = index; break; } }
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
        foreach (var item in _items) { var midpoint = edge + item.Bounds.Width / 2; if (x <= edge + item.Bounds.Width) { after = x > midpoint; return item; } edge += item.Bounds.Width; }
        return _items.LastOrDefault();
    }
    void ClearIndicators() { foreach (var item in _items) item.SetDropIndicator(false); }

    public void Select(XYTab tab)
    {
        if (!_items.Any(x => ReferenceEquals(x.Tab, tab))) return;
        foreach (var item in _items) item.Tab.IsSelected = ReferenceEquals(item.Tab, tab);
    }

    public void Close(XYDockTab item)
    {
        var index = _items.IndexOf(item); if (index < 0) return; var selected = item.Tab.IsSelected;
        _items.RemoveAt(index); Detach(item);
        if (selected && _items.Count > 0) Select(_items[Math.Min(index, _items.Count - 1)].Tab);
        Build(); TabClosed?.Invoke(this, item);
    }

    public void Move(XYDockTab item, int targetIndex)
    {
        var source = _items.IndexOf(item); if (source < 0) return;
        targetIndex = Math.Clamp(targetIndex, 0, _items.Count - 1); if (source == targetIndex) return;
        _items.RemoveAt(source); _items.Insert(targetIndex, item); Build(); OrderChanged?.Invoke(this, EventArgs.Empty);
    }

    void Detach(XYDockTab item)
    { item.Tab.SelectionRequested -= OnSelectionRequested; item.Tab.CloseRequested -= OnCloseRequested; item.DropRequested -= OnDropRequested; item.DragMoved -= OnDragMoved; item.DragCanceled -= OnDragCanceled; }
}
