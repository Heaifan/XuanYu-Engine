using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYInspectorSection : Border
{
    readonly Grid _root = new() { RowDefinitions = new RowDefinitions("Auto,*") };
    string _header = "";
    Control? _content;
    bool _expanded = true;
    public string Header { get => _header; set { _header = value; Build(); } }
    public Control? Content { get => _content; set { _content = value; Build(); } }
    public bool IsCollapsible { get; set; } = true;
    public bool IsExpanded { get => _expanded; set { _expanded = value || !IsCollapsible; Build(); } }

    public XYInspectorSection() { Classes.Add("xyui-inspector-section"); Child = _root; Build(); }

    void Build()
    {
        _root.Children.Clear(); var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,28") };
        header.Children.Add(new TextBlock { Text = _header, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-inspector-section-header" } });
        var toggle = new XYIconButton { IsVisible = IsCollapsible, Content = new XYIcon { Icon = _expanded ? XyuiVectorIcon.ChevronDown : XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Small } };
        toggle.Click += (_, _) => IsExpanded = !IsExpanded; header.Children.Add(toggle); Grid.SetColumn(toggle, 1); _root.Children.Add(header);
        if (_content is not null) { _content.IsVisible = _expanded; _root.Children.Add(_content); Grid.SetRow(_content, 1); }
    }
}
