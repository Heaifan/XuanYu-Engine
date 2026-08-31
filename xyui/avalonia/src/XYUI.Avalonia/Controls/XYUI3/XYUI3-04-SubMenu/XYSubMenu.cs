using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSubMenu : Border
{
    XYMenu _parent = new(); XYMenu _child = new(); readonly Grid _grid = new(); readonly XYSubMenuConnector _connector = new(); readonly List<XYSubMenu> _children = []; XYSubMenu? _parentSubMenu; bool _openLeft;
    public XYMenu ParentMenu { get => _parent; set { if (ReferenceEquals(_parent, value)) return; DetachTriggers(); _parent.UnregisterSubMenu(this); _parent = value; _parent.RegisterSubMenu(this); Build(); AttachTriggers(); } }
    public XYMenu ChildMenu { get => _child; set { DetachChild(); _child = value; Build(); AttachChild(); } }
    public XYSubMenu? ParentSubMenu { get => _parentSubMenu; set { if (ReferenceEquals(_parentSubMenu, value)) return; _parentSubMenu?._children.Remove(this); _parentSubMenu = value; if (value is not null && !value._children.Contains(this)) value._children.Add(this); if (value?.EffectiveVisible == false) Close(); else SyncVisibility(); } }
    public IReadOnlyList<XYSubMenu> ChildSubMenus => _children;
    public bool IsOpen { get; private set; } = true;
    public bool EffectiveVisible => IsOpen && (_parentSubMenu?.EffectiveVisible ?? true);
    public XYSubMenuConnector Connector => _connector;
    public bool OpenLeft { get => _openLeft; set { _openLeft = value; Build(); } }
    public bool ShowParentMenu { get; set; } = true;
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public XYSubMenu() { Classes.Add("xyui-sub-menu"); Child = _grid; Build(); InitializeInteraction(); AttachTriggers(); }
    void Build()
    {
        _parent.MinWidth = 270; _child.MinWidth = 260; _connector.IsMirrored = OpenLeft;
        _grid.Children.Clear(); _grid.ColumnDefinitions = new ColumnDefinitions("270,40,260");
        if (OpenLeft) { _grid.Children.Add(_child); _grid.Children.Add(_connector); _grid.Children.Add(_parent); Grid.SetColumn(_connector, 1); Grid.SetColumn(_parent, 2); }
        else { _grid.Children.Add(_parent); _grid.Children.Add(_connector); _grid.Children.Add(_child); Grid.SetColumn(_connector, 1); Grid.SetColumn(_child, 2); }
        Child = _grid; SyncVisibility();
    }
    void AttachTriggers() { ParentMenu.RegisterSubMenu(this); foreach (var item in ParentMenu.Items.OfType<XYMenuItem>()) { item.SubMenuRequested -= OnTriggerRequested; item.SubMenuRequested += OnTriggerRequested; } ParentMenu.Closed -= OnParentClosed; ParentMenu.Closed += OnParentClosed; }
    void DetachTriggers() { foreach (var item in ParentMenu.Items.OfType<XYMenuItem>()) item.SubMenuRequested -= OnTriggerRequested; ParentMenu.Closed -= OnParentClosed; }
    void AttachChild() { foreach (var item in ChildMenu.Items.OfType<XYMenuItem>()) { item.Invoked -= OnChildInvoked; item.Invoked += OnChildInvoked; } }
    void DetachChild() { foreach (var item in ChildMenu.Items.OfType<XYMenuItem>()) item.Invoked -= OnChildInvoked; }
    void OnChildInvoked(object? sender, EventArgs e) => Close();
    void OnParentClosed(object? sender, EventArgs e) => Close();
}
