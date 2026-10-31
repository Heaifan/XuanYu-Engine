using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationRail
{
    void OnSelected(object? sender, EventArgs e)
    {
        if (sender is not XYNavigationItem item) return;
        if (_footer?.Id == item.Id) { ExpandRequested?.Invoke(this, EventArgs.Empty); return; }
        var wasSelected = _state.SelectedId == item.Id;
        if (!wasSelected && !_state.RequestNavigation(item.Id)) { item.IsSelected = false; return; }
        if (wasSelected) { if (IsContextFlyoutOpen) CloseContext(); else OpenContext(item); }
    }
    void OnItemKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not XYNavigationItem item) return;
        var items = _itemViews.Values.ToArray(); var index = Array.IndexOf(items, item);
        if (e.Key is Key.Up or Key.Down or Key.Home or Key.End)
        { var target = e.Key == Key.Home ? items.FirstOrDefault() : e.Key == Key.End ? items.LastOrDefault() : items.ElementAtOrDefault(Math.Clamp(index + (e.Key == Key.Down ? 1 : -1), 0, items.Length - 1)); target?.Focus(); e.Handled = true; }
        else if (e.Key == Key.Right) { if (item.Id == _state.SelectedId) OpenContext(item); e.Handled = true; }
        else if (e.Key is Key.Left or Key.Escape) { CloseContext(); e.Handled = true; }
    }
    void OpenContext(XYNavigationItem anchor)
    {
        if (anchor is null) return;
        CloseContext(); var entries = _contextMap.TryGetValue(anchor.Id, out var mapped) ? mapped : _contextMap.GetValueOrDefault("*") ?? ContextItems;
        if (entries.Count == 0) return;
        var parent = new XYMenu(new XYMenuItem { Label = anchor.Label, HasSubMenu = true });
        var child = new XYMenu(entries.Select((entry, index) => new XYMenuItem { Label = entry.Label, IsHovered = index == 0 }).ToArray()) { MinWidth = 218 };
        _contextFlyout = new XYSubMenu { ParentMenu = parent, ChildMenu = child, ShowParentMenu = false }; _contextFlyout.Close();
        _popup = new Popup { PlacementTarget = anchor, Placement = PlacementMode.Right, VerticalOffset = -24, IsLightDismissEnabled = true, Child = _contextFlyout };
        XyuiOverlayResourceBridge.Attach(_contextFlyout); _popup.Closed += OnPopupClosed; _popup.IsOpen = true; child.ApplyOverlayStyling(); _contextFlyout.Open();
    }
    void OnPopupClosed(object? sender, EventArgs e) => CloseContext();
    void CloseContext()
    {
        if (_popup is null) return;
        _popup.Closed -= OnPopupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; _contextFlyout?.Close(); _contextFlyout = null;
    }
}
