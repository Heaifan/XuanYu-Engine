using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextDropdownBoard : Border
{
    readonly IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> _actions;
    readonly Border _rootSurface = new() { Classes = { "xyui-context-board-surface" }, Padding = new Thickness(0), Width = 156 };
    readonly List<Border> _childSurfaces = [];
    readonly Border _compatPopupChild = new() { Width = 1, Height = 1 };
    readonly List<ScrollViewer> _scrollHosts = [];
    XYContextOverlayHost? _overlayHost; Window? _ownerWindow; Control? _anchor; Point _rootPosition; bool _open;
    public Popup Popup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Height = 0, IsVisible = false };
    public XYMenu Menu { get; }
    public IReadOnlyList<XYSubMenu> SubMenus { get; }
    public IReadOnlyList<Border> ChildMenuSurfaces => _childSurfaces;
    public Border RootMenuSurface => _rootSurface;
    public XYContextOverlayHost? OverlayHost => _overlayHost;
    public Control? Anchor => _anchor;
    public XYContextCategoryList CategoryList { get; }
    public XYContextActionPane ActionPane { get; } = new();
    public XYContextHintBar HintBar { get; } = new();
    public string Header { get; }
    public bool IsOpen => _open;
    public XYContextCategory? SelectedCategory => CategoryList.SelectedCategory;
    public XYContextAction? SelectedAction => ActionPane.SelectedAction;
    public event EventHandler<XYContextAction>? ActionExecuted;
    public XYContextDropdownBoard(string header, IEnumerable<XYContextCategory> categories, IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> actions)
    {
        Header = header; _actions = actions; Classes.Add("xyui-context-dropdown-board"); CategoryList = new XYContextCategoryList(categories); CategoryList.SelectionChanged += (_, category) => RefreshActions(category); ActionPane.ActionExecuted += (_, action) => ActionExecuted?.Invoke(this, action); Menu = BuildMenu(categories); SubMenus = Menu.Items.OfType<XYMenuItem>().Select(x => x.SubMenu!).ToArray(); Popup.Child = _compatPopupChild; Popup.Closed += (_, _) => Close(); Child = new Border { Width = 1, Height = 1 }; BuildSurface(); CategoryList.Select(CategoryList.Categories.FirstOrDefault()?.Id ?? "");
    }
    public void AttachTrigger(Control trigger) { if (IsOpen && !ReferenceEquals(_anchor, trigger)) Close(); _anchor = trigger; Popup.PlacementTarget = trigger; }
    public void Open() { if (_anchor is null) return; _open = true; Popup.PlacementTarget = _anchor; Menu.ApplyOverlayStyling(); foreach (var submenu in SubMenus) submenu.ChildMenu.ApplyOverlayStyling(); _overlayHost = XYContextOverlayHost.Attach(_anchor); Menu.Open(); if (_overlayHost is null) return; _ownerWindow = TopLevel.GetTopLevel(_anchor) as Window; _ownerWindow?.AddHandler(InputElement.PointerPressedEvent, OnWindowPointerPressed, RoutingStrategies.Tunnel); _overlayHost.AddOverlay(_rootSurface); foreach (var child in _childSurfaces) _overlayHost.AddOverlay(child); _overlayHost.SizeChanged += OnOverlaySizeChanged; _anchor.LayoutUpdated += OnAnchorLayoutUpdated; AttachScrollHosts(); Dispatcher.UIThread.Post(PlaceRoot, DispatcherPriority.Render); Focus(); }
    public void Open(Control target) { AttachTrigger(target); Open(); }
    public void Close() { if (!_open && _overlayHost is null) return; _open = false; if (_anchor is not null) _anchor.LayoutUpdated -= OnAnchorLayoutUpdated; DetachScrollHosts(); _ownerWindow?.RemoveHandler(InputElement.PointerPressedEvent, OnWindowPointerPressed); _ownerWindow = null; foreach (var submenu in SubMenus) submenu.Close(); Menu.Close(); if (_overlayHost is not null) { _overlayHost.SizeChanged -= OnOverlaySizeChanged; _overlayHost.RemoveOverlay(_rootSurface); foreach (var child in _childSurfaces) _overlayHost.RemoveOverlay(child); _overlayHost.IsHitTestVisible = _overlayHost.Children.Count > 0; } _overlayHost = null; Popup.IsOpen = false; Popup.IsVisible = false; }
    public void Toggle() { if (IsOpen) Close(); else Open(); }
    public void Toggle(Control target) { AttachTrigger(target); Toggle(); }
    public void DismissOutside() => Close();
    public void SelectCategory(string id) => CategoryList.Select(id);
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) Close(); else if (e.Key == Key.Down) ActionPane.Move(1); else if (e.Key == Key.Up) ActionPane.Move(-1); else if (e.Key == Key.Left) SelectRelative(-1); else if (e.Key == Key.Right) SelectRelative(1); else if (e.Key == Key.Enter) ActionPane.ExecuteSelected(); else { base.OnKeyDown(e); return; } e.Handled = true; }
    void SelectRelative(int delta) { var categories = CategoryList.Categories; var index = Math.Clamp(Array.IndexOf(categories.ToArray(), SelectedCategory) + delta, 0, categories.Count - 1); CategoryList.Select(categories[index].Id); }
    void RefreshActions(XYContextCategory category) => ActionPane.SetActions(_actions.TryGetValue(category.Id, out var actions) ? actions : []);
    void BuildSurface()
    {
        _rootSurface.Child = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = Header, Height = 22, Classes = { "xyui-context-board-header" } }, Menu, new XYSeparator(), HintBar } };
        for (var i = 0; i < SubMenus.Count; i++) BuildChildSurface(i, SubMenus[i]);
    }
    void BuildChildSurface(int index, XYSubMenu submenu)
    {
        var surface = new Border { Width = 142, IsVisible = false, Classes = { "xyui-context-menu-surface" }, Child = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = ((XYMenuItem)Menu.Items[index]).Label, Height = 22, Classes = { "xyui-context-board-header" } }, submenu.ChildMenu } } };
        _childSurfaces.Add(surface); submenu.Opened += (_, _) => { surface.IsVisible = true; Dispatcher.UIThread.Post(() => PlaceChild(index, true), DispatcherPriority.Render); }; submenu.Closed += (_, _) => surface.IsVisible = false;
    }
    void PlaceRoot()
    {
        if (!_open || _overlayHost is null || _anchor is null) return; var point = _anchor.TranslatePoint(new Point(0, _anchor.Bounds.Height), _overlayHost); if (point is null) return;
        if (_ownerWindow is not null) _overlayHost.SyncToOwner(_ownerWindow.ClientSize);
        var owner = _overlayHost.Bounds; var anchorRect = new Rect(_anchor.TranslatePoint(new Point(0, 0), _overlayHost)!.Value, _anchor.Bounds.Size);
        if (!new Rect(owner.Size).Intersects(anchorRect)) { _rootSurface.IsVisible = false; foreach (var child in _childSurfaces) child.IsVisible = false; return; }
        _rootSurface.IsVisible = true; var size = _rootSurface.Bounds.Size; var x = Math.Clamp(point.Value.X, 0, Math.Max(0, owner.Width - size.Width)); var y = point.Value.Y;
        if (y + size.Height > owner.Height) y = point.Value.Y - _anchor.Bounds.Height - size.Height; _rootPosition = new Point(x, Math.Clamp(y, 0, Math.Max(0, owner.Height - size.Height))); Canvas.SetLeft(_rootSurface, _rootPosition.X); Canvas.SetTop(_rootSurface, _rootPosition.Y);
        for (var i = 0; i < SubMenus.Count; i++) if (SubMenus[i].EffectiveVisible) { _childSurfaces[i].IsVisible = true; PlaceChild(i); }
    }
    void PlaceChild(int index, bool opening = false)
    {
        if (!_open || !_rootSurface.IsVisible || _overlayHost is null || index >= _childSurfaces.Count) return; var surface = _childSurfaces[index]; var owner = _overlayHost.Bounds; var size = surface.Bounds.Size; var x = _rootPosition.X + _rootSurface.Bounds.Width + 5;
        if (x + size.Width > owner.Width) x = _rootPosition.X - size.Width - 5; var trigger = Menu.Items.OfType<XYMenuItem>().ElementAt(index); var y = _rootPosition.Y + 1 + 22 + 4 + 4 + trigger.Bounds.Top + (opening ? 5 : 0); x = Math.Clamp(x, 0, Math.Max(0, owner.Width - size.Width)); y = Math.Clamp(y, 0, Math.Max(0, owner.Height - size.Height)); Canvas.SetLeft(surface, x); Canvas.SetTop(surface, y);
    }
    void OnOverlaySizeChanged(object? sender, SizeChangedEventArgs e) { PlaceRoot(); for (var i = 0; i < _childSurfaces.Count; i++) if (_childSurfaces[i].IsVisible) PlaceChild(i, true); }
    void OnAnchorLayoutUpdated(object? sender, EventArgs e) { if (!_open) return; PlaceRoot(); for (var i = 0; i < _childSurfaces.Count; i++) if (_childSurfaces[i].IsVisible) PlaceChild(i); }
    void AttachScrollHosts() { DetachScrollHosts(); if (_anchor is null) return; foreach (var scroll in _anchor.GetVisualAncestors().OfType<ScrollViewer>()) { scroll.ScrollChanged += OnAnchorScrollChanged; _scrollHosts.Add(scroll); } }
    void DetachScrollHosts() { foreach (var scroll in _scrollHosts) scroll.ScrollChanged -= OnAnchorScrollChanged; _scrollHosts.Clear(); }
    void OnAnchorScrollChanged(object? sender, ScrollChangedEventArgs e) => Dispatcher.UIThread.Post(() => OnAnchorLayoutUpdated(sender, EventArgs.Empty), DispatcherPriority.Render);
    void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.Source is not Visual source || IsInside(source, _anchor) || IsInside(source, _rootSurface) || _childSurfaces.Any(x => x.IsVisible && IsInside(source, x))) return; Close(); }
    static bool IsInside(Visual source, Control? target) => target is not null && (ReferenceEquals(source, target) || source.GetVisualAncestors().Contains(target));
    XYMenu BuildMenu(IEnumerable<XYContextCategory> categories)
    {
        var menu = new XYMenu { Width = 156, Classes = { "xyui-context-menu-body" } };
        foreach (var category in categories) { var item = new XYMenuItem { Id = category.Id, Label = category.Label, Icon = CategoryIcon(category.Id), HasSubMenu = true, Height = 30 }; var child = new XYMenu((_actions.TryGetValue(category.Id, out var actions) ? actions.Select(ActionItem) : []).Cast<Control>().ToArray()) { Width = 142, MinWidth = 0, Classes = { "xyui-context-submenu-body" } }; var submenu = new XYSubMenu { ChildMenuWidth = 142, ParentMenu = menu, ChildMenu = child, Trigger = item, ShowParentMenu = false, OverlayHosted = true }; item.SubMenu = submenu; menu.Items.Add(item); }
        return menu;
    }
    XYMenuItem ActionItem(XYContextAction action) { var item = new XYMenuItem { Id = action.Id, Label = action.Label, Icon = ActionIcon(action.Id), IsEnabled = action.IsEnabled, Height = 30 }; item.Invoked += (_, _) => { ActionPane.SelectAction(action.Id); ActionExecuted?.Invoke(this, action); }; return item; }
    static XyuiVectorIcon? CategoryIcon(string id) => id switch { "point" => XyuiVectorIcon.Select, "line" => XyuiVectorIcon.Move, "area" => XyuiVectorIcon.BoxSelect, _ => null };
    static XyuiVectorIcon? ActionIcon(string id) => id switch { "marker" => XyuiVectorIcon.Locate, "poi" => XyuiVectorIcon.Tag, "road" => XyuiVectorIcon.Move, "boundary" => XyuiVectorIcon.BoxSelect, "river" => XyuiVectorIcon.Pan, "region" => XyuiVectorIcon.Section, "blocked" => XyuiVectorIcon.Stop, "parcel" => XyuiVectorIcon.Browse, _ => null };
}
