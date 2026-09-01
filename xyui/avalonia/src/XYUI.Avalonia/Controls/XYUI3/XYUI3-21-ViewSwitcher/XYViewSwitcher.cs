using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYViewDefinition(string Id, string Label, XyuiVectorIcon Icon,
    string Shortcut = "", int Priority = 0, bool IsEnabled = true, bool SupportsSecondarySettings = false);
public sealed class XYViewState
{
    public IReadOnlyList<XYViewDefinition> AvailableViews { get; }
    public string? CurrentViewId { get; private set; }
    public event EventHandler? Changed;
    public XYViewState(IEnumerable<XYViewDefinition> views, string? currentViewId = null)
    { AvailableViews = views.ToArray(); CurrentViewId = currentViewId ?? AvailableViews.FirstOrDefault()?.Id; }
    public void Commit(string id) { if (AvailableViews.Any(v => v.Id == id)) { CurrentViewId = id; Changed?.Invoke(this, EventArgs.Empty); } }
}
public sealed class XYViewChangeRequest : EventArgs
{
    public XYViewDefinition View { get; } public bool IsAccepted { get; private set; } public bool IsRejected { get; private set; }
    public XYViewChangeRequest(XYViewDefinition view) => View = view;
    public void Accept() { IsRejected = false; IsAccepted = true; } public void Reject() { IsAccepted = false; IsRejected = true; }
}
public enum XYViewSwitcherVariant { Segmented, Dropdown, PrimaryMore }
public sealed partial class XYViewSwitcher : Border
{
    readonly StackPanel _host = new() { Orientation = Orientation.Horizontal, Spacing = 2 };
    readonly XYMenu _menu = new() { Classes = { "xyui-view-switcher-menu" } };
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    public XYViewState State { get; } public IReadOnlyList<XYViewDefinition> Views => State.AvailableViews;
    public XYViewSwitcherVariant Variant { get; set; } = XYViewSwitcherVariant.Segmented;
    public XYMenu ViewMenu => _menu; public Popup ViewPopup => _popup;
    public string? CurrentViewId => State.CurrentViewId;
    public event EventHandler<XYViewChangeRequest>? ViewChangeRequested; public event EventHandler<string>? ViewChanged;
    public XYViewSwitcher(XYViewState state, XYViewSwitcherVariant variant = XYViewSwitcherVariant.Segmented) { State = state; Variant = variant; State.Changed += (_, _) => Refresh(); Classes.Add("xyui-view-switcher"); _popup.Child = _menu; _popup.Closed += (_, _) => _menu.Close(); Child = _host; Refresh(); }
    public XYViewSwitcher(IEnumerable<XYViewDefinition> views, XYViewState? state = null, XYViewSwitcherVariant variant = XYViewSwitcherVariant.Segmented) : this(state ?? new XYViewState(views), variant) { }
    void Refresh() { _host.Children.Clear(); if (Variant == XYViewSwitcherVariant.Dropdown) _host.Children.Add(BuildTrigger()); else if (Variant == XYViewSwitcherVariant.PrimaryMore) BuildPrimaryMore(_host); else foreach (var view in Views) _host.Children.Add(BuildSegment(view)); _host.Children.Add(_popup); }
    XYButton BuildSegment(XYViewDefinition view) { var button = Button(view.Label, view.Icon); button.Classes.Set("xyui-view-selected", view.Id == CurrentViewId); button.IsEnabled = view.IsEnabled; button.Click += (_, _) => Request(view); return button; }
    XYButton BuildTrigger() { var current = Views.FirstOrDefault(v => v.Id == CurrentViewId); var button = Button(current?.Label ?? "选择视图", current?.Icon ?? XyuiVectorIcon.Browse); button.Click += (_, _) => Open(); return button; }
    void BuildPrimaryMore(StackPanel host) { var primary = Views.Where(v => v.Priority >= 0).Take(3).ToArray(); foreach (var view in primary) host.Children.Add(BuildSegment(view)); var rest = Views.Except(primary).ToArray(); if (rest.Length > 0) { var more = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-view-more" } }; more.Click += (_, _) => Open(rest); host.Children.Add(more); } }
    XYButton Button(string label, XyuiVectorIcon icon) => new() { Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center } } }, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-view-segment" }, Height = 36, VerticalContentAlignment = VerticalAlignment.Center };
    void Request(XYViewDefinition view) { var request = new XYViewChangeRequest(view); ViewChangeRequested?.Invoke(this, request); if (!request.IsRejected) { State.Commit(view.Id); ViewChanged?.Invoke(this, view.Id); } }
    public void SelectView(string id) { var view = Views.FirstOrDefault(v => v.Id == id); if (view is not null && view.IsEnabled) Request(view); }
    public void CommitCurrentView(string id) => State.Commit(id);
    public void Open(IEnumerable<XYViewDefinition>? entries = null) { var source = entries ?? Views; _menu.Items = source.Select(v => new XYMenuItem { Label = v.Label, Icon = v.Icon, Shortcut = v.Shortcut, IsEnabled = v.IsEnabled, Classes = { "xyui-view-menu-item" } }).ToArray(); foreach (var item in _menu.Items.OfType<XYMenuItem>()) item.SelectionRequested += (_, _) => SelectView(Views.First(v => v.Label == item.Label).Id); _popup.IsOpen = true; }
}
