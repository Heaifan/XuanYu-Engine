using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSidebar : Border
{
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYSidebar, bool>(nameof(IsCollapsed));
    IReadOnlyList<XYNavigationItem> _primaryItems = [];
    IReadOnlyList<XYNavigationItem> _contextItems = [];
    XYNavigationState? _state;
    readonly Grid _panel = new() { RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*,Auto") };
    public IReadOnlyList<XYNavigationItem> PrimaryItems { get => _primaryItems; set { _primaryItems = value; _state = null; Build(); } }
    public IReadOnlyList<XYNavigationItem> ContextItems { get => _contextItems; set { _contextItems = value; Build(); } }
    public IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> ContextByNavigationId { get; set; } = new Dictionary<string, IReadOnlyList<XYNavigationEntry>>();
    public XYNavigationState NavigationState { get => _state ??= CreateState(); set { _state = value; Build(); } }
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public event EventHandler? FooterInvoked;
    public XYSidebar() { Classes.Add("xyui-sidebar"); Child = _panel; Build(); }
    public void Build()
    {
        Classes.Set("xyui-sidebar-collapsed", IsCollapsed); _panel.Children.Clear();
        if (IsCollapsed) { var rail = new XYNavigationRail(NavigationState, ContextMap(), Footer(), true); rail.ExpandRequested += (_, _) => IsCollapsed = false; Add(rail, 0); return; }
        var expand = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.ChevronLeft, Size = XyuiIconSize.Small }, Classes = { "xyui-sidebar-collapse" } }; expand.Click += (_, _) => IsCollapsed = true;
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        header.Children.Add(new TextBlock { Text = "玄域", Classes = { "xyui-sidebar-title" } }); header.Children.Add(expand); Grid.SetColumn(expand, 1);
        Add(new Border { Classes = { "xyui-sidebar-header" }, Child = header }, 0);
        Add(new XYNavigationMenu(NavigationState), 1); if (ContextItems.Count > 0) Add(Context(), 2); Add(new XYSidebarFooter(() => FooterInvoked?.Invoke(this, EventArgs.Empty)), 4);
    }
    void Add(Control control, int row) { _panel.Children.Add(control); Grid.SetRow(control, row); }
    XYNavigationState CreateState() => new(_primaryItems.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)), _primaryItems.FirstOrDefault(x => x.IsSelected)?.Id);
    IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> ContextMap() => ContextByNavigationId.Count > 0 ? ContextByNavigationId : new Dictionary<string, IReadOnlyList<XYNavigationEntry>> { ["*"] = _contextItems.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon)).ToArray() };
    XYNavigationEntry Footer() => new("settings", "设置", XyuiVectorIcon.Section);
    Control Context() { var panel = new StackPanel { Classes = { "xyui-sidebar-context" } }; panel.Children.Add(new TextBlock { Text = "地图内容", Classes = { "xyui-sidebar-context-label" } }); var items = new StackPanel(); foreach (var item in _contextItems) items.Children.Add(ContextButton(item)); panel.Children.Add(items); return panel; }
    static Control ContextButton(XYNavigationItem item) => new Button { Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new XYIcon { Icon = item.Icon, Size = XyuiIconSize.Small, Classes = { "xyui-sidebar-context-icon" } }, new TextBlock { Text = item.Label, Classes = { "xyui-sidebar-context-item" } } } }, Classes = { "xyui-sidebar-context-button" } };
}

sealed class XYSidebarFooter : Button
{
    public XYSidebarFooter(Action? invoked) { Classes.Add("xyui-sidebar-footer"); Content = new TextBlock { Text = "设置", Classes = { "xyui-sidebar-footer-label" } }; if (invoked is not null) Click += (_, _) => invoked(); }
}
