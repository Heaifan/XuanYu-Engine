using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYBottomNavigationItem(string Id, string Label, XyuiVectorIcon Icon, string? Badge = null, bool IsEnabled = true);
public sealed class XYBottomNavigationRequest : EventArgs
{
    public XYNavigationEntry Destination { get; } public bool IsAccepted { get; private set; } public bool IsRejected { get; private set; }
    public XYBottomNavigationRequest(XYNavigationEntry destination) => Destination = destination; public void Accept() { IsAccepted = true; IsRejected = false; } public void Reject() { IsRejected = true; IsAccepted = false; }
}
sealed class XYBottomDestination : Border
{
    public string Id { get; } public event EventHandler? Invoked;
    public XYBottomDestination(XYBottomNavigationItem item, bool selected)
    { Id = item.Id; Classes.Add("xyui-bottom-navigation-destination"); Classes.Set("xyui-bottom-navigation-selected", selected); IsEnabled = item.IsEnabled; var stack = new StackPanel { Spacing = 2, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Children = { new XYIcon { Icon = item.Icon, Size = XyuiIconSize.Small, Classes = { "xyui-bottom-navigation-icon" } }, new TextBlock { Text = item.Label, Classes = { "xyui-bottom-navigation-label" }, HorizontalAlignment = HorizontalAlignment.Center } } }; var content = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; content.Children.Add(stack); if (item.Badge is not null) content.Children.Add(new XYStatusDot { State = XyuiStatusState.Error, Classes = { "xyui-bottom-navigation-badge" }, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top }); Child = content; PointerPressed += (_, e) => { if (IsEnabled) { Invoked?.Invoke(this, EventArgs.Empty); e.Handled = true; } }; }
}
public sealed class XYBottomNavigation : Border
{
    readonly Grid _grid = new() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch }; Grid? _primaryHost;
    public XYNavigationState NavigationState { get; } public XYButton? PrimaryAction { get; } public IReadOnlyList<XYBottomNavigationItem> Items { get; }
    public string? CurrentDestinationId => NavigationState.SelectedId; public double SafeAreaBottom { get; set; }
    public event EventHandler<XYBottomNavigationRequest>? DestinationRequested; public event EventHandler? PrimaryActionRequested; public event EventHandler<string>? DestinationChanged;
    public XYBottomNavigation(XYNavigationState state, IEnumerable<XYBottomNavigationItem>? items = null, XYButton? primaryAction = null) { NavigationState = state; Items = (items ?? state.Entries.Select(e => new XYBottomNavigationItem(e.Id, e.Label, e.Icon))).ToArray(); PrimaryAction = primaryAction; state.Changed += (_, _) => Refresh(); Classes.Add("xyui-bottom-navigation"); HorizontalAlignment = HorizontalAlignment.Stretch; Child = _grid; ConfigurePrimary(); Refresh(); }
    public XYBottomNavigation(IEnumerable<XYBottomNavigationItem> items) : this(new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon))), items) { }
    void ConfigurePrimary() { if (PrimaryAction is null) return; PrimaryAction.Classes.Add("xyui-bottom-navigation-primary"); PrimaryAction.Width = 54; PrimaryAction.Height = 54; PrimaryAction.HorizontalAlignment = HorizontalAlignment.Center; PrimaryAction.VerticalAlignment = VerticalAlignment.Top; if (PrimaryAction.Content is XYIcon icon) icon.Classes.Add("xyui-bottom-navigation-primary-icon"); PrimaryAction.Click += (_, _) => PrimaryActionRequested?.Invoke(this, EventArgs.Empty); _primaryHost = new Grid { Classes = { "xyui-bottom-navigation-primary-host" }, RowDefinitions = new RowDefinitions("*,Auto"), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch }; _primaryHost.Children.Add(PrimaryAction); var label = new TextBlock { Text = "新建", Classes = { "xyui-bottom-navigation-primary-label" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom }; Grid.SetRow(label, 1); _primaryHost.Children.Add(label); }
    void Refresh() { _grid.Children.Clear(); var slotCount = Items.Count + (PrimaryAction is null ? 0 : 1); _grid.ColumnDefinitions = new ColumnDefinitions(string.Join(',', Enumerable.Repeat("*", slotCount))); var primarySlot = PrimaryAction is null ? -1 : Items.Count / 2; var slot = 0; for (var i = 0; i < Items.Count; i++) { if (slot == primarySlot) { _grid.Children.Add(_primaryHost!); Grid.SetColumn(_primaryHost!, slot++); } var item = Items[i]; var nav = new XYBottomDestination(item, item.Id == CurrentDestinationId); nav.Invoked += (_, _) => SelectDestination(item.Id); Grid.SetColumn(nav, slot++); _grid.Children.Add(nav); } if (slot == primarySlot && _primaryHost is not null) { _grid.Children.Add(_primaryHost); Grid.SetColumn(_primaryHost, slot); } }
    public void SelectDestination(string id) { var entry = NavigationState.Entries.FirstOrDefault(e => e.Id == id); if (entry is null || entry.Id == CurrentDestinationId) return; var request = new XYBottomNavigationRequest(entry); DestinationRequested?.Invoke(this, request); if (request.IsRejected || !request.IsAccepted) return; NavigationState.Select(id); DestinationChanged?.Invoke(this, id); }
    public void CommitDestination(string id) => NavigationState.Select(id);
}
