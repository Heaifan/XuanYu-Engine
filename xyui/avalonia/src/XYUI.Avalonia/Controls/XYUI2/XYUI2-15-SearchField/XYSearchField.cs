using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYSearchField : XyuiEditableTextBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty = TextBox.PlaceholderTextProperty.AddOwner<XYSearchField>();
    public static readonly StyledProperty<Control?> FilterContentProperty = AvaloniaProperty.Register<XYSearchField, Control?>(nameof(FilterContent));
    public static readonly StyledProperty<bool> FilterActiveProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(FilterActive));
    public static readonly StyledProperty<bool> IsFilterOpenProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(IsFilterOpen));
    public static readonly StyledProperty<bool> IsSearchingProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(IsSearching));
    public static readonly StyledProperty<bool> IsNoResultProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(IsNoResult));
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public Control? FilterContent { get => GetValue(FilterContentProperty); set => SetValue(FilterContentProperty, value); }
    public bool FilterActive { get => GetValue(FilterActiveProperty); set => SetValue(FilterActiveProperty, value); }
    public bool IsFilterOpen { get => GetValue(IsFilterOpenProperty); set => SetValue(IsFilterOpenProperty, value); }
    public bool IsSearching { get => GetValue(IsSearchingProperty); set => SetValue(IsSearchingProperty, value); }
    public bool IsNoResult { get => GetValue(IsNoResultProperty); set => SetValue(IsNoResultProperty, value); }
    public event EventHandler<RoutedEventArgs>? SearchRequested;
    public event EventHandler<RoutedEventArgs>? FilterRequested;
    internal Button? ClearActionPart { get; private set; }
    internal Button? FilterPart { get; private set; }
    internal Popup? FilterPopupPart { get; private set; }

    public XYSearchField() { Classes.Add("xyui-search-field"); TextChanged += OnSearchTextChanged; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property is var property && (property == FilterActiveProperty || property == IsSearchingProperty || property == IsNoResultProperty || property == IsEnabledProperty || property == FilterContentProperty || property == IsFilterOpenProperty)) SyncState();
        if (change.Property == IsEnabledProperty && !IsEnabled) CloseFilterForLifecycle();
    }
    void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => SyncState();
    internal void SyncState()
    {
        Classes.Set("xyui-search-filter-active", FilterActive); Classes.Set("xyui-search-searching", IsSearching); Classes.Set("xyui-search-no-result", IsNoResult);
        if (ClearActionPart is not null) ClearActionPart.IsVisible = IsEnabled && !string.IsNullOrEmpty(Text);
        if (FilterPart is not null) FilterPart.IsEnabled = IsEnabled;
        SyncFilterPopup();
    }
    internal void ClearSearch() { if (!IsEnabled) return; Text = string.Empty; Focus(); }
    internal void RequestFilter() { if (!IsEnabled) return; IsFilterOpen = !IsFilterOpen; FilterRequested?.Invoke(this, new RoutedEventArgs()); }
    internal void RequestSearch() => SearchRequested?.Invoke(this, new RoutedEventArgs());

    internal void SyncFilterPopup()
    {
        if (FilterPopupPart is null) return;
        if (!IsEnabled || !IsFilterOpen || FilterContent is null) { FilterPopupPart.IsOpen = false; FilterPopupPart.IsVisible = false; FilterPopupPart.Height = 0; return; }
        FilterPopupPart.Height = double.NaN; FilterPopupPart.IsVisible = true; FilterPopupPart.PlacementTarget = this; FilterPopupPart.Width = Math.Max(Bounds.Width, 240); FilterPopupPart.IsOpen = true;
    }

    internal void CloseFilterForLifecycle()
    {
        if (IsFilterOpen) IsFilterOpen = false;
        if (FilterPopupPart is not null) { FilterPopupPart.IsOpen = false; FilterPopupPart.IsVisible = false; FilterPopupPart.Height = 0; }
    }

    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; }
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CloseFilterForLifecycle(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e);
    }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => CloseFilterForLifecycle();
    void OnHostWindowDeactivated(object? sender, EventArgs e) => CloseFilterForLifecycle();
    void OnHostWindowClosed(object? sender, EventArgs e) => CloseFilterForLifecycle();
}
