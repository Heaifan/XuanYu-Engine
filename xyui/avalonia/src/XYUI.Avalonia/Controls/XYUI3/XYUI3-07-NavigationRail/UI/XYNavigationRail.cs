using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationRail : Border
{
    readonly StackPanel _panel = new() { Spacing = 4 };
    readonly Dictionary<string, XYNavigationItem> _itemViews = [];
    XYNavigationState _state;
    IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> _contextMap;
    XYNavigationEntry? _footer;
    bool _building;
    XyuiNavigationLayoutVariant _layoutVariant;
    Popup? _popup;
    XYSubMenu? _contextFlyout;
    public ObservableCollection<XYNavigationItem> Items { get; } = [];
    public XYNavigationState NavigationState { get => _state; set { if (ReferenceEquals(_state, value)) return; _state.Changed -= SyncSelection; _state = value; _state.Changed += SyncSelection; Build(); } }
    public ObservableCollection<XYNavigationEntry> ContextItems { get; } = [];
    public XYNavigationEntry? Footer { get => _footer; set { _footer = value; Build(); } }
    public bool ShowExpandButton { get; set { if (field == value) return; field = value; Build(); } }
    public XyuiNavigationLayoutVariant LayoutVariant { get => _layoutVariant; set { if (_layoutVariant == value) return; _layoutVariant = value; Build(); } }
    public bool IsContextFlyoutOpen => _popup?.IsOpen == true;
    public XYSubMenu? NavigationContextFlyout => _contextFlyout;
    public event EventHandler? ExpandRequested;
    public XYNavigationRail() : this(new XYNavigationState([]), new Dictionary<string, IReadOnlyList<XYNavigationEntry>>(), null, false) { }
    public XYNavigationRail(IReadOnlyList<XYNavigationItem> items) : this(CreateState(items), new Dictionary<string, IReadOnlyList<XYNavigationEntry>>(), null, false) { }
    public XYNavigationRail(XYNavigationState state, IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> contextMap, XYNavigationEntry? footer = null, bool showExpandButton = false)
    {
        _state = state; _contextMap = contextMap; _footer = footer; ShowExpandButton = showExpandButton; Classes.Add("xyui-navigation-rail"); Child = _panel;
        _state.Changed += SyncSelection; Items.CollectionChanged += OnItemsChanged; foreach (var item in contextMap.Values.SelectMany(x => x)) ContextItems.Add(item); Build();
    }
    public XYNavigationRail(params XYNavigationItem[] items) : this((IReadOnlyList<XYNavigationItem>)items) { }
    void Build()
    {
        _building = true; Classes.Set("xyui-navigation-rail-workspace", LayoutVariant == XyuiNavigationLayoutVariant.Workspace); _panel.Children.Clear(); _itemViews.Clear();
        if (ShowExpandButton) { var expand = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Small }, Classes = { "xyui-rail-expand" } }; expand.SetValue(AutomationProperties.NameProperty, "展开侧边栏"); expand.Click += (_, _) => ExpandRequested?.Invoke(this, EventArgs.Empty); _panel.Children.Add(expand); }
        foreach (var entry in _state.Entries) AddItem(entry);
        if (_footer is not null) { _panel.Children.Add(new XYSeparator { Classes = { "xyui-rail-footer-separator" } }); AddItem(_footer); }
        _building = false;
    }
    void AddItem(XYNavigationEntry entry)
    {
        var item = Items.FirstOrDefault(x => x.Id == entry.Id) ?? new XYNavigationItem { Id = entry.Id };
        item.Label = entry.Label; item.Icon = entry.Icon; item.Badge = entry.Badge; item.Status = entry.Status; item.IsEnabled = entry.IsEnabled; item.IsSelected = entry.Id == _state.SelectedId; item.LayoutVariant = LayoutVariant; item.IsIconOnly = LayoutVariant == XyuiNavigationLayoutVariant.Default;
        if (LayoutVariant == XyuiNavigationLayoutVariant.Default) { item.Width = 36; item.Height = 36; item.HorizontalAlignment = HorizontalAlignment.Center; }
        else { item.ClearValue(Border.WidthProperty); item.ClearValue(Border.HeightProperty); item.ClearValue(Border.HorizontalAlignmentProperty); }
        item.Classes.Set("xyui-rail-item", LayoutVariant == XyuiNavigationLayoutVariant.Default); item.Classes.Set("xyui-rail-workspace-item", LayoutVariant == XyuiNavigationLayoutVariant.Workspace); item.SetValue(AutomationProperties.NameProperty, entry.Label); ToolTip.SetTip(item, entry.Label); item.Selected -= OnSelected; item.Selected += OnSelected; item.KeyDown -= OnItemKeyDown; item.KeyDown += OnItemKeyDown;
        item.Classes.Set("xyui-rail-item", LayoutVariant == XyuiNavigationLayoutVariant.Default); if (!Items.Contains(item)) Items.Add(item); _itemViews[entry.Id] = item; _panel.Children.Add(item);
    }
    static XYNavigationState CreateState(IEnumerable<XYNavigationItem> items) => new(items.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon, x.Badge, x.Status, x.IsEnabled)), items.FirstOrDefault(x => x.IsSelected)?.Id);
    void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) { if (_building) return; if (_state.Entries.Count == 0 && Items.Count > 0) NavigationState = CreateState(Items); else Build(); }
    internal IReadOnlyList<XYNavigationEntry> ContextFor(string id) => _contextMap.TryGetValue(id, out var mapped) ? mapped : ContextItems;
    internal void SetPopup(Popup? popup, XYSubMenu? flyout) { _popup = popup; _contextFlyout = flyout; }
    void SyncSelection(object? sender, EventArgs e) { foreach (var item in _itemViews.Values) item.IsSelected = item.Id == _state.SelectedId; }
}
