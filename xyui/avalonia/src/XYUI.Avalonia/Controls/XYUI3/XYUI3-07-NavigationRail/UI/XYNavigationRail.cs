using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationRail : Border
{
    readonly StackPanel _panel = new() { Spacing = 4 };
    readonly Dictionary<string, XYNavigationItem> _itemViews = [];
    readonly XYNavigationState _state;
    readonly IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> _contextMap;
    readonly XYNavigationEntry? _footer;
    Popup? _popup;
    XYSubMenu? _contextFlyout;
    public IReadOnlyList<XYNavigationItem> Items => _itemViews.Values.ToArray();
    public XYNavigationState NavigationState => _state;
    public IReadOnlyList<XYNavigationEntry> ContextItems => _contextMap.Values.SelectMany(x => x).ToArray();
    public XYSubMenu? NavigationContextFlyout => _contextFlyout;
    public event EventHandler? ExpandRequested;
    public XYNavigationRail(IReadOnlyList<XYNavigationItem> items) : this(CreateState(items), new Dictionary<string, IReadOnlyList<XYNavigationEntry>>(), null, false) { }
    public XYNavigationRail(XYNavigationState state, IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> contextMap, XYNavigationEntry? footer = null, bool showExpandButton = false)
    {
        _state = state; _contextMap = contextMap; _footer = footer; Classes.Add("xyui-navigation-rail"); Child = _panel; Build(showExpandButton); _state.Changed += SyncSelection;
    }
    public XYNavigationRail(params XYNavigationItem[] items) : this((IReadOnlyList<XYNavigationItem>)items) { }
    void Build(bool showExpandButton)
    {
        _panel.Children.Clear(); _itemViews.Clear();
        if (showExpandButton) { var expand = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Small }, Classes = { "xyui-rail-expand" } }; expand.Click += (_, _) => ExpandRequested?.Invoke(this, EventArgs.Empty); _panel.Children.Add(expand); }
        foreach (var entry in _state.Entries) AddItem(entry);
        if (_footer is not null) { _panel.Children.Add(new XYSeparator { Classes = { "xyui-rail-footer-separator" } }); AddItem(_footer); }
    }
    void AddItem(XYNavigationEntry entry) { var item = new XYNavigationItem { Id = entry.Id, Label = entry.Label, Icon = entry.Icon, IsSelected = entry.Id == _state.SelectedId, IsIconOnly = true }; item.Classes.Add("xyui-rail-item"); item.Selected += OnSelected; _itemViews[entry.Id] = item; _panel.Children.Add(item); }
    static XYNavigationState CreateState(IEnumerable<XYNavigationItem> items) => new(items.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)), items.FirstOrDefault(x => x.IsSelected)?.Id);
    void SyncSelection(object? sender, EventArgs e) { foreach (var item in _itemViews.Values) item.IsSelected = item.Id == _state.SelectedId; }
}
