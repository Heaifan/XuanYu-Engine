using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBar
{
    Popup? _popup;
    EventHandler<EventArgs>? _popupClosed;
    void InitializeInteraction() { Focusable = true; KeyDown += OnKeyDown; PointerPressed += OnPointerPressed; }
    void OnItemActivated(object? sender, EventArgs e) { if (sender is XYMenuBarItem item) { if (OpenMenu == item.Menu) Close(); else Open(item); } }
    public void Open(XYMenuBarItem item)
    {
        Close(); item.IsActive = true; OpenMenuId = item.Label; OpenMenu = item.Menu; if (OpenMenu is null) return;
        OpenMenu.Open(); _popup = new Popup { PlacementTarget = item, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = OpenMenu }; _popupClosed = (_, _) => Close(); _popup.Closed += _popupClosed; _popup.IsOpen = true;
    }
    public void Close()
    {
        if (OpenMenu is not null) OpenMenu.Close(); if (_popup is not null) { if (_popupClosed is not null) _popup.Closed -= _popupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; _popupClosed = null; }
        foreach (var item in Items) item.IsActive = false; OpenMenu = null; OpenMenuId = null;
    }
    void OnPointerPressed(object? sender, PointerPressedEventArgs e) { if (OpenMenu is not null && (e.Source as Visual)?.FindAncestorOfType<XYMenuBarItem>() is null) Close(); }
    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) { Close(); e.Handled = true; return; }
        if (e.Key is Key.Left or Key.Right) { MoveItem(e.Key == Key.Right ? 1 : -1); e.Handled = true; return; }
        if ((e.Key is Key.Enter or Key.Down) && FocusedItem() is { } item) { Open(item); e.Handled = true; }
    }
    void MoveItem(int delta) { if (Items.Count == 0) return; var current = Items.Select((item, index) => (item, index)).FirstOrDefault(x => x.item.IsActive).index; var index = (current + delta + Items.Count) % Items.Count; Items[index].Focus(); }
    XYMenuBarItem? FocusedItem() => Items.FirstOrDefault(x => x.IsFocused) ?? Items.FirstOrDefault(x => x.IsActive);
}
