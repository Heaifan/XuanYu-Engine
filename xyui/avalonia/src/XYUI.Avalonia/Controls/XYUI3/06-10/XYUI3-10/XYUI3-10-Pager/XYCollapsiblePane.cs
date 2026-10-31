using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XYCollapsiblePaneState { Expanded, Collapsed }

public sealed class XYCollapsiblePane : Border
{
    readonly Grid _root = new() { RowDefinitions = new RowDefinitions("Auto,*") };
    string _header = "";
    Control? _content;
    XYCollapsiblePaneState _state = XYCollapsiblePaneState.Expanded;
    public string Header { get => _header; set { _header = value; Build(); } }
    public Control? Content { get => _content; set { _content = value; Build(); } }
    public bool IsCollapsible { get; set; } = true;
    public bool IsCollapsed { get => _state == XYCollapsiblePaneState.Collapsed; set => PaneState = value ? XYCollapsiblePaneState.Collapsed : XYCollapsiblePaneState.Expanded; }
    public XYCollapsiblePaneState PaneState { get => _state; private set { if (!IsCollapsible && value == XYCollapsiblePaneState.Collapsed) return; _state = value; Build(); } }

    public XYCollapsiblePane() { Classes.Add("xyui-collapsible-pane"); Child = _root; Build(); }

    void Build()
    {
        _root.Children.Clear(); var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,28") };
        header.Children.Add(new TextBlock { Text = _header, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-collapsible-pane-header" } });
        var toggle = new XYIconButton { IsVisible = IsCollapsible, Content = new XYIcon { Icon = IsCollapsed ? XyuiVectorIcon.ChevronRight : XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small } };
        toggle.Click += (_, _) => IsCollapsed = !IsCollapsed; header.Children.Add(toggle); Grid.SetColumn(toggle, 1); _root.Children.Add(header);
        if (_content is not null) { _content.IsVisible = !IsCollapsed; _root.Children.Add(_content); Grid.SetRow(_content, 1); }
    }
}
