using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenu : Border
{
    readonly AvaloniaList<Control> _items = [];
    readonly List<XYSubMenu> _subMenus = [];
    bool _embedded;
    bool _overlayStylesApplied;
    public bool IsOpen { get; private set; }
    public int FocusedIndex { get; private set; } = -1;
    public Control? FocusRestoreTarget { get; set; }
    public event EventHandler? Closed;
    public event EventHandler<XYMenuItem>? SubMenuRequested;

    [Content]
    public IList<Control> Items
    {
        get => _items;
        set { _items.Clear(); if (value is not null) _items.AddRange(value); }
    }

    public bool IsEmbedded { get => _embedded; set { _embedded = value; ApplyMode(); } }
    public XYMenu() { Classes.Add("xyui-menu"); _items.CollectionChanged += (_, _) => Build(); Build(); }
    public XYMenu(params Control[] items) : this() => _items.AddRange(items);
    public static XYMenu FromModels(IEnumerable<XYMenuItemModel> models)
    {
        var menu = new XYMenu(); var controls = new List<Control>();
        foreach (var model in models)
        {
            if (model.Children is { Count: > 0 })
            {
                var item = Item(model); item.HasSubMenu = true; item.SubMenu = new XYSubMenu { ParentMenu = menu, ChildMenu = FromModels(model.Children) }; controls.Add(item);
            }
            else controls.Add(Item(model));
        }
        menu.Items = controls; foreach (var item in controls.OfType<XYMenuItem>()) if (item.SubMenu is { } submenu) submenu.Trigger = item; return menu;
    }
    static XYMenuItem Item(XYMenuItemModel model) => new() { Id = model.Id, Label = model.Label, Icon = model.Icon, Shortcut = model.Shortcut, IsEnabled = model.IsEnabled, IsChecked = model.IsChecked, CheckKind = model.CheckKind, IsDestructive = model.IsDestructive };
    internal void RegisterSubMenu(XYSubMenu submenu) { if (!_subMenus.Contains(submenu)) _subMenus.Add(submenu); }
    internal void UnregisterSubMenu(XYSubMenu submenu) => _subMenus.Remove(submenu);
    internal IReadOnlyList<XYSubMenu> SubMenus => _subMenus;
    public static XYSeparator Separator() => new() { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-menu-separator" } };
    void Build()
    {
        var panel = Child as StackPanel;
        if (panel is null) { panel = new StackPanel { Classes = { "xyui-menu-items" } }; Child = panel; }
        else panel.Children.Clear();
        foreach (var item in Items)
        {
            if (item.Parent is Panel p) p.Children.Remove(item);
            if (item is XYMenuItem menuItem) Attach(menuItem);
            panel.Children.Add(item);
        }
        ApplyMode();
    }
    public XYMenuItem? SelectedItem => Items.OfType<XYMenuItem>().FirstOrDefault(x => x.IsSelected);
    public void ClearSelection() { foreach (var item in Items.OfType<XYMenuItem>()) { item.CloseSubMenu(); item.ClearInteractionState(); } }
    internal void ApplyOverlayStyling() { XyuiOverlayResourceBridge.Attach(this); if (!_overlayStylesApplied) { Styles.Add(XyuiComponentStyles.Create()); _overlayStylesApplied = true; } ApplyStyling(); foreach (var item in Items) item.ApplyStyling(); }
    void Attach(XYMenuItem item) { item.SelectionRequested -= OnSelectionRequested; item.SelectionRequested += OnSelectionRequested; item.Invoked -= OnItemInvoked; item.Invoked += OnItemInvoked; item.SubMenuRequested -= OnSubMenuRequested; item.SubMenuRequested += OnSubMenuRequested; item.PointerEntered -= OnItemPointerEntered; item.PointerEntered += OnItemPointerEntered; }
    void OnSelectionRequested(object? sender, EventArgs e)
    {
        if (sender is not XYMenuItem item) return;
        foreach (var other in Items.OfType<XYMenuItem>().Where(x => !ReferenceEquals(x, item)))
        { if (item.CheckKind == XyuiMenuCheckKind.Radio && other.CheckKind == XyuiMenuCheckKind.Radio) other.IsChecked = false; other.ClearInteractionState(); }
        item.IsSelected = true;
    }
    void OnItemInvoked(object? sender, EventArgs e) { if (sender is XYMenuItem { HasSubMenu: false, SubMenu: null }) Close(); }
    void OnItemPointerEntered(object? sender, global::Avalonia.Input.PointerEventArgs e) { if (sender is XYMenuItem { SubMenu: { } submenu }) submenu.Open(); }
    void OnSubMenuRequested(object? sender, EventArgs e) { if (sender is XYMenuItem item) SubMenuRequested?.Invoke(this, item); }
    void ApplyMode() { if (_embedded) Classes.Add("xyui-menu-embedded"); else Classes.Remove("xyui-menu-embedded"); }
}
