using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Controls;

public partial class XYSearchField : XyuiEditableTextBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty = TextBox.PlaceholderTextProperty.AddOwner<XYSearchField>();
    public static readonly StyledProperty<bool> FilterActiveProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(FilterActive));
    public static readonly StyledProperty<bool> IsSearchingProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(IsSearching));
    public static readonly StyledProperty<bool> IsNoResultProperty = AvaloniaProperty.Register<XYSearchField, bool>(nameof(IsNoResult));
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool FilterActive { get => GetValue(FilterActiveProperty); set => SetValue(FilterActiveProperty, value); }
    public bool IsSearching { get => GetValue(IsSearchingProperty); set => SetValue(IsSearchingProperty, value); }
    public bool IsNoResult { get => GetValue(IsNoResultProperty); set => SetValue(IsNoResultProperty, value); }
    public event EventHandler<RoutedEventArgs>? SearchRequested;
    public event EventHandler<RoutedEventArgs>? FilterRequested;
    internal Button? ClearActionPart { get; private set; }
    internal Button? FilterPart { get; private set; }

    public XYSearchField() { Classes.Add("xyui-search-field"); TextChanged += OnSearchTextChanged; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property is var property && (property == FilterActiveProperty || property == IsSearchingProperty || property == IsNoResultProperty || property == IsEnabledProperty)) SyncState();
    }
    void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => SyncState();
    internal void SyncState()
    {
        Classes.Set("xyui-search-filter-active", FilterActive); Classes.Set("xyui-search-searching", IsSearching); Classes.Set("xyui-search-no-result", IsNoResult);
        if (ClearActionPart is not null) ClearActionPart.IsVisible = IsEnabled && !string.IsNullOrEmpty(Text);
        if (FilterPart is not null) FilterPart.IsEnabled = IsEnabled;
    }
    internal void ClearSearch() { if (!IsEnabled) return; Text = string.Empty; Focus(); }
    internal void RequestFilter() { if (!IsEnabled) return; FilterActive = !FilterActive; FilterRequested?.Invoke(this, new RoutedEventArgs()); }
    internal void RequestSearch() => SearchRequested?.Invoke(this, new RoutedEventArgs());
}
