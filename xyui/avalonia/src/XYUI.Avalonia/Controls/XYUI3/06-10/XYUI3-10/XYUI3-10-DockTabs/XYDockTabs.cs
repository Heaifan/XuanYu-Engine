using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYDockTabs : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    string? _activeTabId;
    bool _building;
    [Content] public ObservableCollection<XYDockTab> Items { get; } = [];
    public string? ActiveTabId { get => _activeTabId; set => Select(value); }
    public XYTab? ActiveTab => Items.FirstOrDefault(x => x.Tab.Id == _activeTabId)?.Tab;
    public event EventHandler<XYDockTab>? SelectionChanged;
    public event EventHandler<XYDockHandoffRequest>? DockHandoffRequested;
    public XYDockTabs() { Classes.Add("xyui-dock-tabs"); Child = _panel; Items.CollectionChanged += OnItemsChanged; }
    public XYDockTabs(params XYDockTab[] items) : this() { _building = true; foreach (var item in items) Items.Add(item); _building = false; Build(); }
    void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) { if (!_building) Build(); }
    void Build()
    { _panel.Children.Clear(); foreach (var item in Items) { Attach(item); _panel.Children.Add(item); } NormalizeActive(); }
    void NormalizeActive() { foreach (var item in Items) if (string.IsNullOrEmpty(item.Tab.Id)) item.Tab.Id = item.Tab.Label; var selected = Items.FirstOrDefault(x => x.Tab.IsSelected)?.Tab ?? Items.FirstOrDefault()?.Tab; Select(_activeTabId is not null && Items.Any(x => x.Tab.Id == _activeTabId) ? _activeTabId : selected?.Id); }
}
