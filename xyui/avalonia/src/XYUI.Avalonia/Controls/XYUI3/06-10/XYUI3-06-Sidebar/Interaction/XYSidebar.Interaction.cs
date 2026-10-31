using Avalonia;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSidebar
{
    bool _resizing;
    double _resizeStartX, _resizeStartWidth;
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsCollapsedProperty)
        {
            if (IsCollapsed) UserSidebarWidth = Math.Clamp(Width > 0 ? Width : UserSidebarWidth, MinWidthValue, MaxWidthValue);
            Build();
        }
        if (change.Property == WidthProperty && !_resizing && !IsCollapsed && Width > 0) UserSidebarWidth = Math.Clamp(Width, MinWidthValue, MaxWidthValue);
    }
    void InitializeResize()
    {
        ResizeHandle.Focusable = false; ResizeHandle.PointerPressed += OnResizePressed; ResizeHandle.PointerMoved += OnResizeMoved; ResizeHandle.PointerReleased += OnResizeReleased;
    }
    void OnResizePressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsCollapsed || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        _resizing = true; _resizeStartX = e.GetPosition(this).X; _resizeStartWidth = UserSidebarWidth; e.Pointer.Capture(ResizeHandle); e.Handled = true;
    }
    void OnResizeMoved(object? sender, PointerEventArgs e)
    {
        if (!_resizing) return; SetUserSidebarWidth(_resizeStartWidth + e.GetPosition(this).X - _resizeStartX); e.Handled = true;
    }
    void OnResizeReleased(object? sender, PointerReleasedEventArgs e) { if (!_resizing) return; _resizing = false; e.Pointer.Capture(null); e.Handled = true; }
}
