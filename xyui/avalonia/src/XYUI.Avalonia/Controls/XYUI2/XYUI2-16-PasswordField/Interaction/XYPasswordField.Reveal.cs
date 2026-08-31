using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYPasswordField
{
    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;
    IPointer? _revealPointer;
    void OnRevealPointerPressed(object? sender, PointerPressedEventArgs e) { if (!IsEnabled || RevealPart is null) return; _revealPointer = e.Pointer; e.Pointer.Capture(RevealPart); SetRevealed(true); e.Handled = true; }
    void OnRevealPointerReleased(object? sender, PointerReleasedEventArgs e) { if (_revealPointer == e.Pointer) ForceHidePassword(); e.Handled = true; }
    void OnRevealPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) => ForceHidePassword();
    internal void ForceHidePassword()
    {
        var pointer = _revealPointer; _revealPointer = null; pointer?.Capture(null); SetRevealed(false);
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; }
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ForceHidePassword(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e);
    }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => ForceHidePassword();
    void OnHostWindowDeactivated(object? sender, EventArgs e) => ForceHidePassword();
    void OnHostWindowClosed(object? sender, EventArgs e) => ForceHidePassword();
    void OnRevealLostFocus(object? sender, RoutedEventArgs e) => ForceHidePassword();
}
