using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabs : Border
{
    readonly Grid _panel = new();
    bool _syncing;
    string? _selectedTabId;
    public static readonly StyledProperty<XyuiTabSizingMode> SizingModeProperty =
        AvaloniaProperty.Register<XYTabs, XyuiTabSizingMode>(nameof(SizingMode), XyuiTabSizingMode.Equal);
    [Content]
    public ObservableCollection<XYTab> Items { get; } = [];
    public XyuiTabSizingMode SizingMode { get => GetValue(SizingModeProperty); set => SetValue(SizingModeProperty, value); }
    public string? SelectedTabId { get => _selectedTabId; set => Select(value); }
    public XYTab? SelectedItem => Items.FirstOrDefault(x => x.Id == _selectedTabId);
    public event EventHandler<XYTab>? TabClosed;
    public event EventHandler<XYTab>? SelectionChanged;
    public XYTabs() { Classes.Add("xyui-tabs"); Focusable = true; KeyDown += OnKeyDown; Child = _panel; Items.CollectionChanged += OnItemsChanged; }
    public XYTabs(params XYTab[] items) : this() { foreach (var item in items) Items.Add(item); NormalizeSelection(); }
    public void Build()
    {
        _panel.Children.Clear();
        _panel.ColumnDefinitions.Clear();
        foreach (var item in Items)
        {
            item.SelectionRequested -= OnSelected; item.SelectionRequested += OnSelected;
            item.CloseRequested -= OnCloseRequested; item.CloseRequested += OnCloseRequested;
            item.PropertyChanged -= OnTabPropertyChanged; item.PropertyChanged += OnTabPropertyChanged;
            _panel.ColumnDefinitions.Add(new ColumnDefinition(SizingMode == XyuiTabSizingMode.Equal ? GridLength.Star : GridLength.Auto));
            Grid.SetColumn(item, _panel.Children.Count); item.HorizontalAlignment = HorizontalAlignment.Stretch;
            _panel.Children.Add(item);
        }
        NormalizeSelection();
    }
    void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) { Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizingModeProperty) Build();
    }
    void OnTabPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (!_syncing && e.Property == XYTab.IsSelectedProperty && sender is XYTab tab && tab.IsSelected) Select(tab); if (e.Property == XYTab.IsEnabledProperty && sender is XYTab disabled && !disabled.IsEnabled && disabled.Id == _selectedTabId) Select(Items.FirstOrDefault(x => x.IsEnabled)?.Id); }
    void NormalizeSelection() { foreach (var item in Items) if (string.IsNullOrEmpty(item.Id)) item.Id = item.Label; var selected = Items.FirstOrDefault(x => x.IsSelected && x.IsEnabled)?.Id ?? Items.FirstOrDefault(x => x.IsEnabled)?.Id; if (_selectedTabId is null || !Items.Any(x => x.Id == _selectedTabId)) SetSelection(selected, false); else SetSelection(_selectedTabId, false); }
    void SetSelection(string? id, bool raise)
    { var tab = Items.FirstOrDefault(x => x.Id == id && x.IsEnabled); if (tab is null) return; var changed = _selectedTabId != tab.Id; _selectedTabId = tab.Id; _syncing = true; foreach (var item in Items) item.IsSelected = ReferenceEquals(item, tab); _syncing = false; if (raise && changed) SelectionChanged?.Invoke(this, tab); }
}
