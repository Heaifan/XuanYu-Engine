using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYBottomNavigationItem(string Id, string Label, XyuiVectorIcon Icon, string? Badge = null, bool IsEnabled = true);
public sealed class XYBottomNavigationRequest : EventArgs
{
    public XYNavigationEntry Destination { get; } public bool IsAccepted { get; private set; } public bool IsRejected { get; private set; }
    public XYBottomNavigationRequest(XYNavigationEntry destination) => Destination = destination; public void Accept() { IsAccepted = true; IsRejected = false; } public void Reject() { IsRejected = true; IsAccepted = false; }
}
public sealed class XYBottomNavigation : Border
{
    readonly Grid _grid = new() { ColumnDefinitions = new ColumnDefinitions("*") }; public XYNavigationState NavigationState { get; } public XYButton? PrimaryAction { get; }
    public IReadOnlyList<XYBottomNavigationItem> Items { get; } public string? CurrentDestinationId => NavigationState.SelectedId; public double SafeAreaBottom { get; set; }
    public event EventHandler<XYBottomNavigationRequest>? DestinationRequested; public event EventHandler? PrimaryActionRequested; public event EventHandler<string>? DestinationChanged;
    public XYBottomNavigation(XYNavigationState state, IEnumerable<XYBottomNavigationItem>? items = null, XYButton? primaryAction = null) { NavigationState = state; Items = (items ?? state.Entries.Select(e => new XYBottomNavigationItem(e.Id, e.Label, e.Icon))).ToArray(); PrimaryAction = primaryAction; state.Changed += (_, _) => Refresh(); Classes.Add("xyui-bottom-navigation"); Child = _grid; Refresh(); }
    public XYBottomNavigation(IEnumerable<XYBottomNavigationItem> items) : this(new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon))), items) { }
    void Refresh() { _grid.Children.Clear(); _grid.ColumnDefinitions = new ColumnDefinitions(string.Join(',', Enumerable.Repeat("*", Items.Count + (PrimaryAction is null ? 0 : 1)))); for (var i = 0; i < Items.Count; i++) { var item = Items[i]; var nav = new XYNavigationItem { Id = item.Id, Label = item.Label, Icon = item.Icon, IsSelected = item.Id == CurrentDestinationId, IsEnabled = item.IsEnabled, IsIconOnly = false, Classes = { "xyui-bottom-navigation-item" } }; nav.Selected += (_, _) => SelectDestination(item.Id); Grid.SetColumn(nav, i); _grid.Children.Add(nav); if (item.Badge is not null) nav.Classes.Add("xyui-bottom-navigation-badge"); } if (PrimaryAction is not null) { PrimaryAction.Classes.Add("xyui-bottom-navigation-primary"); PrimaryAction.Click += (_, _) => PrimaryActionRequested?.Invoke(this, EventArgs.Empty); Grid.SetColumn(PrimaryAction, Items.Count); _grid.Children.Add(PrimaryAction); } }
    public void SelectDestination(string id) { var entry = NavigationState.Entries.FirstOrDefault(e => e.Id == id); if (entry is null) return; var request = new XYBottomNavigationRequest(entry); DestinationRequested?.Invoke(this, request); if (!request.IsRejected) { NavigationState.Select(id); DestinationChanged?.Invoke(this, id); } }
    public void CommitDestination(string id) => NavigationState.Select(id);
}
