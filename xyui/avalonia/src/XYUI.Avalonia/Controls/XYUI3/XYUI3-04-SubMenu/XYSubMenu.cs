using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSubMenu : Border
{
    XYMenu _parent = new(); XYMenu _child = new(); readonly Grid _grid = new(); bool _openLeft;
    public XYMenu ParentMenu { get => _parent; set { DetachTriggers(); _parent = value; Build(); AttachTriggers(); } }
    public XYMenu ChildMenu { get => _child; set { _child = value; Build(); } }
    public bool IsOpen { get; private set; } = true;
    public bool OpenLeft { get => _openLeft; set { _openLeft = value; Build(); } }
    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public XYSubMenu() { Classes.Add("xyui-sub-menu"); Child = _grid; Build(); InitializeInteraction(); AttachTriggers(); }
    void Build()
    {
        _parent.MinWidth = 270; _child.MinWidth = 260; _child.IsVisible = IsOpen;
        _grid.Children.Clear(); _grid.ColumnDefinitions = new ColumnDefinitions("270,40,260"); var grid = _grid;
        var connector = new XYSubMenuConnector { IsMirrored = OpenLeft };
        if (OpenLeft) { grid.Children.Add(_child); grid.Children.Add(connector); grid.Children.Add(_parent); Grid.SetColumn(connector, 1); Grid.SetColumn(_parent, 2); }
        else { grid.Children.Add(_parent); grid.Children.Add(connector); grid.Children.Add(_child); Grid.SetColumn(connector, 1); Grid.SetColumn(_child, 2); }
        Child = grid;
    }
    void AttachTriggers() { foreach (var item in ParentMenu.Items.OfType<XYMenuItem>()) { item.SubMenuRequested -= OnTriggerRequested; item.SubMenuRequested += OnTriggerRequested; } }
    void DetachTriggers() { foreach (var item in ParentMenu.Items.OfType<XYMenuItem>()) item.SubMenuRequested -= OnTriggerRequested; }
}
