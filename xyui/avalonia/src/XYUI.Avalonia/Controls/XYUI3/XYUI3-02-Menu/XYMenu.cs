using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenu : Border
{
    IReadOnlyList<Control> _items = [];
    readonly List<XYSubMenu> _subMenus = [];
    bool _embedded;
    bool _overlayStylesApplied;
    public bool IsOpen { get; private set; }
    public int FocusedIndex { get; private set; } = -1;
    public event EventHandler? Closed;
    public event EventHandler<XYMenuItem>? SubMenuRequested;
    public IReadOnlyList<Control> Items { get => _items; set { _items = value; Build(); } }
    public bool IsEmbedded { get => _embedded; set { _embedded = value; ApplyMode(); } }
    public XYMenu() { Classes.Add("xyui-menu"); Build(); }
    public XYMenu(params Control[] items) : this() => Items = items;
    internal void RegisterSubMenu(XYSubMenu submenu) { if (!_subMenus.Contains(submenu)) _subMenus.Add(submenu); }
    internal void UnregisterSubMenu(XYSubMenu submenu) => _subMenus.Remove(submenu);
    internal IReadOnlyList<XYSubMenu> SubMenus => _subMenus;
    public static XYSeparator Separator() => new() { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-menu-separator" } };
    void Build() { var panel = new StackPanel { Classes = { "xyui-menu-items" } }; foreach (var item in Items) { if (item is XYMenuItem menuItem) Attach(menuItem); panel.Children.Add(item); } Child = panel; ApplyMode(); }
    public XYMenuItem? SelectedItem => Items.OfType<XYMenuItem>().FirstOrDefault(x => x.IsSelected);
    public void ClearSelection() { foreach (var item in Items.OfType<XYMenuItem>()) item.ClearInteractionState(); }
    internal void ApplyOverlayStyling() { if (!_overlayStylesApplied) { Styles.Add(XyuiComponentStyles.Create()); _overlayStylesApplied = true; } ApplyStyling(); foreach (var item in Items) item.ApplyStyling(); }
    void Attach(XYMenuItem item) { item.SelectionRequested -= OnSelectionRequested; item.SelectionRequested += OnSelectionRequested; item.Invoked -= OnItemInvoked; item.Invoked += OnItemInvoked; item.SubMenuRequested -= OnSubMenuRequested; item.SubMenuRequested += OnSubMenuRequested; }
    void OnSelectionRequested(object? sender, EventArgs e) { if (sender is XYMenuItem item) { foreach (var other in Items.OfType<XYMenuItem>().Where(x => !ReferenceEquals(x, item))) other.ClearInteractionState(); item.IsSelected = true; } }
    void OnItemInvoked(object? sender, EventArgs e) { var wasOpen = IsOpen; Close(); if (!wasOpen) Closed?.Invoke(this, EventArgs.Empty); }
    void OnSubMenuRequested(object? sender, EventArgs e) { if (sender is XYMenuItem item) SubMenuRequested?.Invoke(this, item); }
    void ApplyMode() { if (_embedded) Classes.Add("xyui-menu-embedded"); else Classes.Remove("xyui-menu-embedded"); }
}
