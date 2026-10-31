using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYCommandPalette
{
    public void Open(Control? placementTarget = null)
    {
        if (IsOpen) return; var parent = this.GetVisualParent(); _restoreParent = parent as Panel; _restoreContentHost = parent as ContentControl ?? this.GetVisualAncestors().OfType<ContentControl>().FirstOrDefault(x => ReferenceEquals(x.Content, this)); _restoreIndex = _restoreParent?.Children.IndexOf(this) ?? -1;
        _reparenting = true; if (_restoreParent is not null) _restoreParent.Children.Remove(this); else if (_restoreContentHost?.Content == this) _restoreContentHost.Content = null; _reparenting = false;
        _popup.PlacementTarget = placementTarget ?? _restoreParent; _popup.Child = this; IsOpen = true; _popup.IsOpen = true; SearchBox.Focus(); SearchBox.SelectAll();
    }
    public void Close()
    {
        if (_closing) return; _closing = true; IsOpen = false; _reparenting = true; _popup.IsOpen = false; _popup.Child = null; _reparenting = false;
        if (_restoreParent is not null && !_restoreParent.Children.Contains(this)) _restoreParent.Children.Insert(Math.Clamp(_restoreIndex, 0, _restoreParent.Children.Count), this); else if (_restoreContentHost is not null) _restoreContentHost.Content = this; _restoreParent = null; _restoreContentHost = null; _closing = false;
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; }
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (!_reparenting) Close(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e);
    }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => Close();
    void OnHostWindowDeactivated(object? sender, EventArgs e) => Close();
    void OnHostWindowClosed(object? sender, EventArgs e) => Close();
}
