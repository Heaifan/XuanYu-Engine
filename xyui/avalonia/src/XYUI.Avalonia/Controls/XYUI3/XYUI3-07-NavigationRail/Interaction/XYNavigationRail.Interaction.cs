using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationRail
{
    void OnSelected(object? sender, EventArgs e)
    {
        if (sender is not XYNavigationItem item) return;
        if (_footer?.Id == item.Id) { ExpandRequested?.Invoke(this, EventArgs.Empty); return; }
        _state.Select(item.Id); OpenContext(item);
    }
    void OpenContext(XYNavigationItem anchor)
    {
        CloseContext(); var entries = _contextMap.TryGetValue(anchor.Id, out var mapped) ? mapped : _contextMap.GetValueOrDefault("*") ?? [];
        if (entries.Count == 0) return;
        var parent = new XYMenu(new XYMenuItem { Label = anchor.Label, HasSubMenu = true });
        var child = new XYMenu(entries.Select(x => new XYMenuItem { Label = x.Label }).ToArray());
        _contextFlyout = new XYSubMenu { ParentMenu = parent, ChildMenu = child, ShowParentMenu = false }; _contextFlyout.Close();
        _popup = new Popup { PlacementTarget = anchor, Placement = PlacementMode.Right, IsLightDismissEnabled = true, Child = _contextFlyout }; _popup.Closed += OnPopupClosed; _popup.IsOpen = true; _contextFlyout.Open();
    }
    void OnPopupClosed(object? sender, EventArgs e) => CloseContext();
    void CloseContext()
    {
        if (_popup is null) return;
        _popup.Closed -= OnPopupClosed; _popup.IsOpen = false; _popup.Child = null; _popup = null; _contextFlyout?.Close(); _contextFlyout = null;
    }
}
