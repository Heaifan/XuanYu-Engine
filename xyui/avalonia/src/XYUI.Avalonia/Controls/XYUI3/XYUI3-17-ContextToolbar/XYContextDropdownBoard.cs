using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextDropdownBoard : Border
{
    readonly IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> _actions;
    readonly Border _surface = new() { Classes = { "xyui-context-board-surface" }, Padding = new Thickness(0) };
    readonly Canvas _host = new() { Width = 1, Height = 1, ClipToBounds = true };
    public Popup Popup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Height = 0, IsVisible = false };
    public XYMenu Menu { get; }
    public IReadOnlyList<XYSubMenu> SubMenus { get; }
    public XYContextCategoryList CategoryList { get; }
    public XYContextActionPane ActionPane { get; } = new();
    public XYContextHintBar HintBar { get; } = new();
    public string Header { get; }
    public bool IsOpen { get => Popup.IsOpen; set => Popup.IsOpen = value; }
    public XYContextCategory? SelectedCategory => CategoryList.SelectedCategory;
    public XYContextAction? SelectedAction => ActionPane.SelectedAction;
    public event EventHandler<XYContextAction>? ActionExecuted;
    public XYContextDropdownBoard(string header, IEnumerable<XYContextCategory> categories, IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> actions)
    {
        Header = header; _actions = actions; Classes.Add("xyui-context-dropdown-board"); CategoryList = new XYContextCategoryList(categories); CategoryList.SelectionChanged += (_, category) => RefreshActions(category); ActionPane.ActionExecuted += (_, action) => ActionExecuted?.Invoke(this, action); Menu = BuildMenu(categories); SubMenus = Menu.Items.OfType<XYMenuItem>().Select(x => x.SubMenu!).ToArray(); Popup.Child = _surface; Popup.Closed += (_, _) => { Menu.Close(); Popup.Height = 0; Popup.IsVisible = false; }; _host.Children.Add(new Border { Width = 1, Height = 1 }); _host.Children.Add(Popup); Child = _host; BuildSurface(); CategoryList.Select(CategoryList.Categories.FirstOrDefault()?.Id ?? "");
    }
    public void Open(Control? target = null) { Menu.ApplyOverlayStyling(); foreach (var submenu in SubMenus) submenu.ChildMenu.ApplyOverlayStyling(); Popup.PlacementTarget = target ?? this; Popup.IsVisible = true; Popup.Height = double.NaN; Menu.Open(); Popup.IsOpen = true; Focus(); }
    public void Close() { Menu.Close(); Popup.IsOpen = false; Popup.Height = 0; Popup.IsVisible = false; }
    public void Toggle(Control? target = null) { if (IsOpen) Close(); else Open(target); }
    public void DismissOutside() => Close();
    public void SelectCategory(string id) => CategoryList.Select(id);
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close(); else if (e.Key == Key.Down) ActionPane.Move(1); else if (e.Key == Key.Up) ActionPane.Move(-1); else if (e.Key == Key.Left) SelectRelative(-1); else if (e.Key == Key.Right) SelectRelative(1); else if (e.Key == Key.Enter) ActionPane.ExecuteSelected(); else { base.OnKeyDown(e); return; } e.Handled = true;
    }
    void SelectRelative(int delta) { var categories = CategoryList.Categories; var index = Math.Max(0, Array.IndexOf(categories.ToArray(), SelectedCategory) + delta); index = Math.Min(index, categories.Count - 1); CategoryList.Select(categories[index].Id); }
    void RefreshActions(XYContextCategory category) => ActionPane.SetActions(_actions.TryGetValue(category.Id, out var actions) ? actions : []);
    XYMenu BuildMenu(IEnumerable<XYContextCategory> categories)
    {
        var menu = new XYMenu { Width = 156, Classes = { "xyui-context-menu-body" } }; foreach (var category in categories) { var item = new XYMenuItem { Id = category.Id, Label = category.Label, Icon = CategoryIcon(category.Id), HasSubMenu = true }; var child = new XYMenu(_actions.TryGetValue(category.Id, out var actions) ? actions.Select(action => ActionItem(action)).ToArray() : []) { Width = 142, MinWidth = 0, Classes = { "xyui-context-submenu-body" } }; var submenu = new XYSubMenu { ChildMenuWidth = 142, ParentMenu = menu, ChildMenu = child, Trigger = item, ShowParentMenu = false }; submenu.Opened += (_, _) => CategoryList.Select(category.Id); item.SubMenu = submenu; menu.Items.Add(item); }
        return menu;
    }
    XYMenuItem ActionItem(XYContextAction action) { var item = new XYMenuItem { Id = action.Id, Label = action.Label, Icon = ActionIcon(action.Id), IsEnabled = action.IsEnabled }; item.Invoked += (_, _) => { ActionPane.SelectAction(action.Id); ActionExecuted?.Invoke(this, action); }; return item; }
    static XyuiVectorIcon? CategoryIcon(string id) => id switch { "point" => XyuiVectorIcon.Select, "line" => XyuiVectorIcon.Move, "area" => XyuiVectorIcon.BoxSelect, _ => null };
    static XyuiVectorIcon? ActionIcon(string id) => id switch { "marker" => XyuiVectorIcon.Locate, "poi" => XyuiVectorIcon.Tag, "road" => XyuiVectorIcon.Move, "boundary" => XyuiVectorIcon.BoxSelect, "river" => XyuiVectorIcon.Pan, "region" => XyuiVectorIcon.Section, "blocked" => XyuiVectorIcon.Stop, "parcel" => XyuiVectorIcon.Browse, _ => null };
    void BuildSurface()
    {
        var main = new Border { Width = 156, Classes = { "xyui-context-menu-surface" }, Child = new StackPanel { Children = { new TextBlock { Text = Header, Classes = { "xyui-context-board-header" } }, Menu, new XYSeparator { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-context-menu-separator" } }, HintBar } } };
        var cascade = new Grid { VerticalAlignment = VerticalAlignment.Top }; foreach (var submenu in SubMenus) cascade.Children.Add(submenu); _surface.Child = new StackPanel { Orientation = Orientation.Horizontal, Children = { main, cascade } };
    }
}
