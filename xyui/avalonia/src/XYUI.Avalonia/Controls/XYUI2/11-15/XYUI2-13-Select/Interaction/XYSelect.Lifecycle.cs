using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYSelect
{
    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime;
        if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated;
        _hostWindow = e.RootVisual as WindowBase;
        if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ClosePopupForLifecycle(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated;
        if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; }
        _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e);
    }

    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => ClosePopupForLifecycle();
    void OnHostWindowDeactivated(object? sender, EventArgs e) => ClosePopupForLifecycle();
    void OnHostWindowClosed(object? sender, EventArgs e) => ClosePopupForLifecycle();
}
