using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSidebar : Border
{
    public const double DefaultWidth = 240, MinWidthValue = 190, MaxWidthValue = 360, CollapsedWidth = 64;
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYSidebar, bool>(nameof(IsCollapsed));
    IReadOnlyList<XYNavigationItem> _primaryItems = [], _contextItems = [];
    XYNavigationState? _state;
    readonly Grid _panel = new() { RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto") };
    public IReadOnlyList<XYNavigationItem> PrimaryItems { get => _primaryItems; set { DetachState(); _primaryItems = value; _state = null; AttachState(); Build(); } }
    public IReadOnlyList<XYNavigationItem> ContextItems { get => _contextItems; set { _contextItems = value; Build(); } }
    IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> _contextMap = new Dictionary<string, IReadOnlyList<XYNavigationEntry>>();
    public IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> ContextByNavigationId { get => _contextMap; set { _contextMap = value; Build(); } }
    public XYNavigationState NavigationState { get => _state ??= CreateState(); set { DetachState(); _state = value; AttachState(); Build(); } }
    public string? CurrentDestinationId => NavigationState.CurrentDestinationId;
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public double UserSidebarWidth { get; private set; } = DefaultWidth;
    public double ExpandedWidth { get => UserSidebarWidth; set => SetUserSidebarWidth(value); }
    Control? _contextRegion, _stickyFooter;
    public Control? ContextRegion { get => _contextRegion; set { _contextRegion = value; Build(); } }
    public Control? StickyFooter { get => _stickyFooter; set { _stickyFooter = value; Build(); } }
    public Border ResizeHandle { get; } = new() { Classes = { "xyui-sidebar-resize-handle" } };
    public event EventHandler<XYNavigationRequest>? NavigationRequested;
    public event EventHandler? FooterInvoked;
    public XYSidebar() { Classes.Add("xyui-sidebar"); Width = UserSidebarWidth; Child = _panel; AttachState(); Build(); InitializeResize(); }
    public void Collapse() => IsCollapsed = true;
    public void Expand() => IsCollapsed = false;
    public bool SelectDestination(string id) => NavigationState.RequestNavigation(id);
    public void SetUserSidebarWidth(double width) { UserSidebarWidth = Math.Clamp(width, MinWidthValue, MaxWidthValue); if (!IsCollapsed) Width = UserSidebarWidth; }
    public void Build()
    {
        Classes.Set("xyui-sidebar-collapsed", IsCollapsed); Width = IsCollapsed ? CollapsedWidth : UserSidebarWidth; _panel.Children.Clear();
        if (IsCollapsed) { var rail = new XYNavigationRail(NavigationState, ContextMap(), Footer(), true); rail.ExpandRequested += (_, _) => Expand(); Add(rail, 0); return; }
        var collapse = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.ChevronLeft, Size = XyuiIconSize.Small }, Classes = { "xyui-sidebar-collapse" } }; collapse.Click += (_, _) => Collapse();
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), Children = { new TextBlock { Text = "玄域", Classes = { "xyui-sidebar-title" } }, collapse } }; Grid.SetColumn(collapse, 1); Add(new Border { Classes = { "xyui-sidebar-header" }, Child = header }, 0);
        Add(new XYNavigationMenu(NavigationState), 1); Add(ContextRegion ?? Context(), 2); Add(StickyFooter ?? new XYSidebarFooter(() => FooterInvoked?.Invoke(this, EventArgs.Empty)), 3); Add(ResizeHandle, 2);
    }
    void Add(Control control, int row) { _panel.Children.Add(control); Grid.SetRow(control, row); }
    XYNavigationState CreateState() => new(_primaryItems.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon, x.Badge, x.Status, x.IsEnabled)), _primaryItems.FirstOrDefault(x => x.IsSelected)?.Id);
    IReadOnlyDictionary<string, IReadOnlyList<XYNavigationEntry>> ContextMap() => ContextByNavigationId.Count > 0 ? ContextByNavigationId : new Dictionary<string, IReadOnlyList<XYNavigationEntry>> { ["*"] = _contextItems.Select(x => new XYNavigationEntry(x.Id, x.Label, x.Icon, x.Badge, x.Status, x.IsEnabled)).ToArray() };
    XYNavigationEntry Footer() => new("settings", "设置", XyuiVectorIcon.Section);
    Control Context() => new XYNavigationMenu(new XYNavigationGroup("地图内容", _contextItems.Select(Clone).ToArray()));
    static XYNavigationItem Clone(XYNavigationItem item) => new() { Id = item.Id, Label = item.Label, Icon = item.Icon, Badge = item.Badge, Status = item.Status, IsEnabled = item.IsEnabled, IsSelected = item.IsSelected };
    void AttachState() { NavigationState.NavigationRequested += OnStateNavigationRequested; }
    void DetachState() { if (_state is not null) _state.NavigationRequested -= OnStateNavigationRequested; }
    void OnStateNavigationRequested(object? sender, XYNavigationRequest request) => NavigationRequested?.Invoke(this, request);
}

sealed class XYSidebarFooter : Button
{
    public XYSidebarFooter(Action? invoked) { Classes.Add("xyui-sidebar-footer"); Content = new TextBlock { Text = "设置", Classes = { "xyui-sidebar-footer-label" } }; if (invoked is not null) Click += (_, _) => invoked(); }
}
