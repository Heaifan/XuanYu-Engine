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
    readonly Border _rootSurface = new() { Classes = { "xyui-context-board-surface" }, Padding = new Thickness(0), Width = 128, CornerRadius = new CornerRadius(6) };
    readonly List<Border> _childSurfaces = [];
    readonly List<ScrollViewer> _scrollHosts = [];
    readonly XYContextPopupHost _popupHost = new();
    Window? _ownerWindow; Control? _anchor; bool _open;
    public Popup Popup => _popupHost.Popup;
    public XYContextOverlayHost? OverlayHost => null;
    public XYMenu Menu { get; }
    public IReadOnlyList<XYSubMenu> SubMenus { get; }
    public IReadOnlyList<Border> ChildMenuSurfaces => _childSurfaces;
    public Border RootMenuSurface => _rootSurface;
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
        Header = header; _actions = actions; Classes.Add("xyui-context-dropdown-board"); CategoryList = new XYContextCategoryList(categories); CategoryList.SelectionChanged += (_, category) => RefreshActions(category); ActionPane.ActionExecuted += (_, action) => ActionExecuted?.Invoke(this, action); Menu = BuildMenu(categories); SubMenus = Menu.Items.OfType<XYMenuItem>().Select(x => x.SubMenu!).ToArray(); Popup.Closed += (_, _) => Close(); LogicalChildren.Add(Popup); Child = new Border { Width = 1, Height = 1 }; BuildSurface(); CategoryList.Select(CategoryList.Categories.FirstOrDefault()?.Id ?? "");
    }
    public void AttachTrigger(Control trigger) { if (IsOpen && !ReferenceEquals(_anchor, trigger)) Close(); _anchor = trigger; Popup.PlacementTarget = trigger; }
    public void Open() { if (_anchor is null) return; _open = true; Menu.ApplyOverlayStyling(); foreach (var submenu in SubMenus) submenu.ChildMenu.ApplyOverlayStyling(); Menu.Open(); _popupHost.Attach(_anchor, _rootSurface, _childSurfaces); _popupHost.Open(); _ownerWindow = TopLevel.GetTopLevel(_anchor) as Window; if (_ownerWindow is not null) { _ownerWindow.PositionChanged += OnOwnerPositionChanged; _ownerWindow.SizeChanged += OnOwnerSizeChanged; _ownerWindow.AddHandler(InputElement.PointerPressedEvent, OnWindowPointerPressed, RoutingStrategies.Tunnel); } _anchor.LayoutUpdated += OnAnchorLayoutUpdated; AttachScrollHosts(); Dispatcher.UIThread.Post(PlaceRoot, DispatcherPriority.Render); Focus(); }
    public void Open(Control target) { AttachTrigger(target); Open(); }
    public void Close() { if (!_open && !_popupHost.IsOpen) return; _open = false; if (_anchor is not null) _anchor.LayoutUpdated -= OnAnchorLayoutUpdated; DetachScrollHosts(); if (_ownerWindow is not null) { _ownerWindow.PositionChanged -= OnOwnerPositionChanged; _ownerWindow.SizeChanged -= OnOwnerSizeChanged; _ownerWindow.RemoveHandler(InputElement.PointerPressedEvent, OnWindowPointerPressed); } _ownerWindow = null; foreach (var submenu in SubMenus) submenu.Close(); Menu.Close(); _popupHost.Close(); }
    public void Toggle() { if (IsOpen) Close(); else Open(); }
    public void Toggle(Control target) { AttachTrigger(target); Toggle(); }
    public void DismissOutside() => Close();
    public void SelectCategory(string id) => CategoryList.Select(id);
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) Close(); else if (e.Key == Key.Down) ActionPane.Move(1); else if (e.Key == Key.Up) ActionPane.Move(-1); else if (e.Key == Key.Left) SelectRelative(-1); else if (e.Key == Key.Right) SelectRelative(1); else if (e.Key == Key.Enter) ActionPane.ExecuteSelected(); else { base.OnKeyDown(e); return; } e.Handled = true; }
    void SelectRelative(int delta) { var categories = CategoryList.Categories; var index = Math.Clamp(Array.IndexOf(categories.ToArray(), SelectedCategory) + delta, 0, categories.Count - 1); CategoryList.Select(categories[index].Id); }
    void RefreshActions(XYContextCategory category) => ActionPane.SetActions(_actions.TryGetValue(category.Id, out var actions) ? actions : []);
    void BuildSurface()
    {
        _rootSurface.Child = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = Header, Height = 32, Padding = new Thickness(8, 4), Classes = { "xyui-context-board-header" } }, Menu, new XYSeparator(), HintBar } };
        for (var i = 0; i < SubMenus.Count; i++) BuildChildSurface(i, SubMenus[i]);
    }
    void BuildChildSurface(int index, XYSubMenu submenu)
    {
        var surface = new Border { Width = 128, CornerRadius = new CornerRadius(6), IsVisible = false, Classes = { "xyui-context-menu-surface" }, Child = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = ((XYMenuItem)Menu.Items[index]).Label, Height = 32, Padding = new Thickness(8, 4), Classes = { "xyui-context-board-header" } }, submenu.ChildMenu } } };
        _childSurfaces.Add(surface); submenu.Opened += (_, _) => { surface.IsVisible = true; Dispatcher.UIThread.Post(() => PlaceChild(index), DispatcherPriority.Render); }; submenu.Closed += (_, _) => { surface.IsVisible = false; Dispatcher.UIThread.Post(PlaceRoot, DispatcherPriority.Render); };
    }
    void PlaceRoot()
    {
        if (!_open || _anchor is null) return; _rootSurface.IsVisible = true; _popupHost.RefreshBounds();
        for (var i = 0; i < SubMenus.Count; i++) if (SubMenus[i].EffectiveVisible) { _childSurfaces[i].IsVisible = true; PlaceChild(i); }
    }
    void PlaceChild(int index)
    {
        if (!_open || !_rootSurface.IsVisible || index >= _childSurfaces.Count) return; var surface = _childSurfaces[index]; var size = surface.Bounds.Size; var x = _rootSurface.Bounds.Width + 4; var trigger = Menu.Items.OfType<XYMenuItem>().ElementAt(index); var triggerPoint = trigger.TranslatePoint(new Point(0, 0), _popupHost.Surface); var y = triggerPoint?.Y ?? 0; _popupHost.SetChildPosition(surface, x, y); _popupHost.RefreshBounds();
    }
    void OnAnchorLayoutUpdated(object? sender, EventArgs e) { if (!_open) return; PlaceRoot(); for (var i = 0; i < _childSurfaces.Count; i++) if (_childSurfaces[i].IsVisible) PlaceChild(i); }
    void AttachScrollHosts() { DetachScrollHosts(); if (_anchor is null) return; foreach (var scroll in _anchor.GetVisualAncestors().OfType<ScrollViewer>()) { scroll.ScrollChanged += OnAnchorScrollChanged; _scrollHosts.Add(scroll); } }
    void DetachScrollHosts() { foreach (var scroll in _scrollHosts) scroll.ScrollChanged -= OnAnchorScrollChanged; _scrollHosts.Clear(); }
    void OnAnchorScrollChanged(object? sender, ScrollChangedEventArgs e) => Dispatcher.UIThread.Post(() => OnAnchorLayoutUpdated(sender, EventArgs.Empty), DispatcherPriority.Render);
    void OnOwnerPositionChanged(object? sender, PixelPointEventArgs e) => _popupHost.RefreshPlacement();
    void OnOwnerSizeChanged(object? sender, SizeChangedEventArgs e) => _popupHost.RefreshPlacement();
    void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e) { if (e.Source is not Visual source || IsInside(source, _anchor) || IsInside(source, _rootSurface) || _childSurfaces.Any(x => x.IsVisible && IsInside(source, x))) return; Close(); }
    static bool IsInside(Visual source, Control? target) => target is not null && (ReferenceEquals(source, target) || source.GetVisualAncestors().Contains(target));
    XYMenu BuildMenu(IEnumerable<XYContextCategory> categories)
    {
        var menu = new XYMenu { Width = 128, Classes = { "xyui-context-menu-body" } };
        foreach (var category in categories) { var item = new XYMenuItem { Id = category.Id, Label = category.Label, Icon = CategoryIcon(category.Id), HasSubMenu = true, Height = 32, Padding = new Thickness(8, 4) }; var child = new XYMenu((_actions.TryGetValue(category.Id, out var actions) ? actions.Select(ActionItem) : []).Cast<Control>().ToArray()) { Width = 128, MinWidth = 0, Classes = { "xyui-context-submenu-body" } }; var submenu = new XYSubMenu { ChildMenuWidth = 128, ParentMenu = menu, ChildMenu = child, Trigger = item, ShowParentMenu = false, OverlayHosted = true }; item.SubMenu = submenu; menu.Items.Add(item); }
        return menu;
    }
    XYMenuItem ActionItem(XYContextAction action) { var item = new XYMenuItem { Id = action.Id, Label = action.Label, Icon = ActionIcon(action.Id), IsEnabled = action.IsEnabled, Height = 32, Padding = new Thickness(8, 4) }; item.Invoked += (_, _) => { ActionPane.SelectAction(action.Id); ActionExecuted?.Invoke(this, action); }; return item; }
    static XyuiVectorIcon? CategoryIcon(string id) => id switch { "point" => XyuiVectorIcon.Select, "line" => XyuiVectorIcon.Move, "area" => XyuiVectorIcon.BoxSelect, _ => null };
    static XyuiVectorIcon? ActionIcon(string id) => id switch { "marker" => XyuiVectorIcon.Locate, "poi" => XyuiVectorIcon.Tag, "road" => XyuiVectorIcon.Move, "boundary" => XyuiVectorIcon.BoxSelect, "river" => XyuiVectorIcon.Pan, "region" => XyuiVectorIcon.Section, "blocked" => XyuiVectorIcon.Stop, "parcel" => XyuiVectorIcon.Browse, _ => null };
}
