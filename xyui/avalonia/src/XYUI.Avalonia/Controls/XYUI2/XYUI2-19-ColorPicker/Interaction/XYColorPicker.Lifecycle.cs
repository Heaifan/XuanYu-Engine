using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    internal void OpenPanel() { if (!IsEnabled || PopupPart is null) return; PopupPart.Height = double.NaN; PopupPart.IsVisible = true; PopupPart.PlacementTarget = this; PopupPart.IsOpen = true; SyncVisuals(); }
    internal void ClosePanel() { if (PopupPart is null) return; PopupPart.IsOpen = false; PopupPart.IsVisible = false; PopupPart.Height = 0; }
    internal void OnPopupClosed(object? sender, EventArgs e) { if (IsOpen) IsOpen = false; }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnWindowDeactivated; _hostWindow.Closed += OnWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { IsOpen = false; if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnWindowDeactivated; _hostWindow.Closed -= OnWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => IsOpen = false;
    void OnWindowDeactivated(object? sender, EventArgs e) => IsOpen = false;
    void OnWindowClosed(object? sender, EventArgs e) => IsOpen = false;
}
