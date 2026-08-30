using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSubMenu
{
    bool _closing;
    void InitializeInteraction() { Focusable = true; KeyDown += OnKeyDown; }
    public void Open()
    {
        if (!EffectiveParentVisible()) return;
        var siblings = _parentSubMenu?.ChildSubMenus ?? ParentMenu.SubMenus;
        foreach (var sibling in siblings.Where(x => !ReferenceEquals(x, this))) sibling.Close();
        if (IsOpen) { SyncVisibility(); return; }
        IsOpen = true; ChildMenu.IsVisible = true; ChildMenu.Open(); SyncVisibility(); Opened?.Invoke(this, EventArgs.Empty);
    }
    public void Close()
    {
        if (_closing) return; _closing = true; var wasOpen = IsOpen || ChildMenu.IsOpen || _children.Any(x => x.IsOpen);
        IsOpen = false; foreach (var child in _children.ToArray()) child.Close(); ChildMenu.IsVisible = false; ChildMenu.Close(); ParentMenu.ClearSelection(); SyncVisibility(); _closing = false;
        if (wasOpen) Closed?.Invoke(this, EventArgs.Empty);
    }
    bool EffectiveParentVisible() => _parentSubMenu?.EffectiveVisible ?? true;
    void SyncVisibility()
    {
        var visible = EffectiveVisible; _child.IsVisible = visible; _connector.IsVisible = visible && ParentMenu.IsVisible && _child.IsVisible;
        _grid.ColumnDefinitions[0].Width = OpenLeft && !visible ? new GridLength(0) : new GridLength(270);
        _grid.ColumnDefinitions[1].Width = visible ? new GridLength(40) : new GridLength(0);
        _grid.ColumnDefinitions[2].Width = !OpenLeft && !visible ? new GridLength(0) : new GridLength(260);
        foreach (var child in _children) child.SyncVisibility();
    }
    void OnTriggerRequested(object? sender, EventArgs e) => Open();
    void OnKeyDown(object? sender, KeyEventArgs e)
    { if (e.Key == Key.Right) { Open(); e.Handled = true; } else if (e.Key is Key.Left or Key.Escape) { Close(); e.Handled = true; } }
}
