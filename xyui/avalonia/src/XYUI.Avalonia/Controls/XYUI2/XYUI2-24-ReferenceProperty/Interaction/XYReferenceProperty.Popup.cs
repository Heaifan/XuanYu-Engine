using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYReferenceProperty
{
    ListBox? _pickerList;
    void OnLocate(object? sender, RoutedEventArgs e) { if (LocatePart?.IsEnabled == true) { LocateRequested?.Invoke(this, EventArgs.Empty); e.Handled = true; } }
    void OnBrowse(object? sender, RoutedEventArgs e) { if (BrowsePart?.IsEnabled == true) { OpenPicker(); BrowseRequested?.Invoke(this, EventArgs.Empty); e.Handled = true; } }
    void OnClear(object? sender, RoutedEventArgs e) { ClearReference(); e.Handled = true; }
    void OpenPicker() { if (!IsEnabled || IsReadOnly || PopupPart is null || ReferencePickerContent is null) return; IsPickerOpen = true; PopupPart.Height = double.NaN; PopupPart.IsVisible = true; PopupPart.PlacementTarget = this; PopupPart.Width = Math.Max(Bounds.Width, 240); PopupPart.IsOpen = true; }
    internal void ClosePicker() { IsPickerOpen = false; if (PopupPart is not null) { PopupPart.IsOpen = false; PopupPart.IsVisible = false; PopupPart.Height = 0; } }
    void OnPopupClosed(object? sender, EventArgs e) => ClosePicker();
    internal void AttachPicker()
    {
        if (_pickerList is not null) _pickerList.SelectionChanged -= OnPickerSelectionChanged;
        _pickerList = ReferencePickerContent as ListBox; if (_pickerList is not null) _pickerList.SelectionChanged += OnPickerSelectionChanged;
    }
    void OnPickerSelectionChanged(object? sender, SelectionChangedEventArgs e) { if (e.AddedItems.OfType<XYReferenceValue>().FirstOrDefault() is { } reference) TryAssignReference(reference); if (_pickerList is not null) _pickerList.SelectedIndex = -1; }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape && IsPickerOpen) { ClosePicker(); e.Handled = true; return; } base.OnKeyDown(e); }
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { ClosePicker(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => ClosePicker(); void OnHostWindowDeactivated(object? sender, EventArgs e) => ClosePicker(); void OnHostWindowClosed(object? sender, EventArgs e) => ClosePicker();
}
