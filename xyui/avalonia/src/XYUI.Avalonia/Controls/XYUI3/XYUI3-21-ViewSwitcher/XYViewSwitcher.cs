using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
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
    public bool Commit(string id) { if (CurrentViewId == id || !AvailableViews.Any(v => v.Id == id)) return false; CurrentViewId = id; Changed?.Invoke(this, EventArgs.Empty); return true; }
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
    readonly Dictionary<XYMenuItem, string> _menuIds = new();
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    public XYViewState State { get; } public IReadOnlyList<XYViewDefinition> Views => State.AvailableViews;
    public XYViewSwitcherVariant Variant { get; }
    public XYMenu ViewMenu => _menu; public Popup ViewPopup => _popup;
    public string? CurrentViewId => State.CurrentViewId;
    public event EventHandler<XYViewChangeRequest>? ViewChangeRequested; public event EventHandler<string>? ViewChanged;
    public XYViewSwitcher(XYViewState state, XYViewSwitcherVariant variant = XYViewSwitcherVariant.Segmented) { State = state; Variant = variant; State.Changed += (_, _) => Refresh(); Classes.Add("xyui-view-switcher"); _popup.Child = _menu; _popup.Closed += (_, _) => ClosePopup(); _menu.Closed += (_, _) => _popup.IsOpen = false; Child = _host; Refresh(); }
    public XYViewSwitcher(IEnumerable<XYViewDefinition> views, XYViewState? state = null, XYViewSwitcherVariant variant = XYViewSwitcherVariant.Segmented) : this(state ?? new XYViewState(views), variant) { }
    void Refresh() { _host.Children.Clear(); if (Variant == XYViewSwitcherVariant.Dropdown) _host.Children.Add(BuildTrigger()); else if (Variant == XYViewSwitcherVariant.PrimaryMore) BuildPrimaryMore(_host); else foreach (var view in Views) _host.Children.Add(BuildSegment(view)); _host.Children.Add(_popup); }
    XYButton BuildSegment(XYViewDefinition view) { var button = Button(view); button.Classes.Set("xyui-view-selected", view.Id == CurrentViewId); button.IsEnabled = view.IsEnabled; button.Click += (_, _) => Request(view); return button; }
    XYButton BuildTrigger() { var current = Views.FirstOrDefault(v => v.Id == CurrentViewId); var button = Button(current ?? new XYViewDefinition("", "选择视图", XyuiVectorIcon.Browse)); button.Classes.Add("xyui-view-dropdown-trigger"); button.Click += (_, _) => Open(button); return button; }
    void BuildPrimaryMore(StackPanel host) { var primary = Views.Where(v => v.Priority >= 0).Take(3).ToArray(); foreach (var view in primary) host.Children.Add(BuildSegment(view)); var rest = Views.Except(primary).ToArray(); if (rest.Length > 0) { var more = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-view-more" }, IsSelected = rest.Any(v => v.Id == CurrentViewId) }; more.Click += (_, _) => Open(more, rest); host.Children.Add(more); } }
    XYButton Button(XYViewDefinition view) => new() { Content = new Grid { RowDefinitions = new RowDefinitions("*,3"), Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Children = { new XYIcon { Icon = view.Icon, Size = XyuiIconSize.Small, VerticalAlignment = VerticalAlignment.Center }, new TextBlock { Text = view.Label, VerticalAlignment = VerticalAlignment.Center } } }, new Border { Classes = { "xyui-view-segment-accent" }, IsVisible = view.Id == CurrentViewId, [Grid.RowProperty] = 1 } } }, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-view-segment" }, Height = 30, VerticalContentAlignment = VerticalAlignment.Center };
    void Request(XYViewDefinition view) { if (view.Id == CurrentViewId) return; var request = new XYViewChangeRequest(view); ViewChangeRequested?.Invoke(this, request); if (request.IsAccepted && State.Commit(view.Id)) ViewChanged?.Invoke(this, view.Id); }
    public void SelectView(string id) { var view = Views.FirstOrDefault(v => v.Id == id); if (view is not null && view.IsEnabled) Request(view); }
    public void CommitCurrentView(string id) => State.Commit(id);
    public void Open(Control? trigger = null, IEnumerable<XYViewDefinition>? entries = null) { _menuIds.Clear(); var source = entries ?? Views; var items = source.Select(v => { var item = new XYMenuItem { Label = v.Label, Icon = v.Icon, Shortcut = v.Shortcut, IsEnabled = v.IsEnabled, IsChecked = v.Id == CurrentViewId, CheckKind = XyuiMenuCheckKind.Check, Classes = { "xyui-view-menu-item" } }; _menuIds[item] = v.Id; item.SelectionRequested += (_, _) => SelectView(_menuIds[item]); item.Invoked += (_, _) => ClosePopup(); return item; }).ToArray(); _menu.Items = items; _menu.Open(); foreach (var item in items) item.IsSelected = _menuIds[item] == CurrentViewId; _popup.PlacementTarget = trigger ?? this; _popup.IsOpen = true; }
    public void ClosePopup() { _menu.Close(); _popup.IsOpen = false; }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { ClosePopup(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => ClosePopup(); void OnHostWindowDeactivated(object? sender, EventArgs e) => ClosePopup(); void OnHostWindowClosed(object? sender, EventArgs e) => ClosePopup();
}
