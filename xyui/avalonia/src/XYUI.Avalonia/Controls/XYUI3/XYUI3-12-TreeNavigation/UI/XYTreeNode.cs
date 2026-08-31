using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYTreeNode : Border
{
    string _label = "";
    int _depth;
    int _activeGuideDepth;
    bool _selected;
    bool _hasChildren;
    bool _expanded;
    XyuiVectorIcon _icon = XyuiVectorIcon.Section;
    public string Label { get => _label; set { _label = value; Build(); } }
    public int Depth { get => _depth; set { _depth = Math.Max(0, value); Build(); } }
    public int ActiveGuideDepth { get => _activeGuideDepth; set { _activeGuideDepth = Math.Max(0, value); Build(); } }
    public bool IsSelected { get => _selected; set { _selected = value; Build(); } }
    public bool HasChildren { get => _hasChildren; set { _hasChildren = value; Build(); } }
    public bool IsExpanded { get => _expanded; set { _expanded = value; Build(); } }
    public XyuiVectorIcon Icon { get => _icon; set { _icon = value; Build(); } }

    public XYTreeNode() { Classes.Add("xyui-tree-node"); Build(); }

    void Build()
    {
        Classes.Set("xyui-tree-selected", IsSelected);
        var outer = new Grid { ColumnDefinitions = new ColumnDefinitions($"{Depth * XyuiCompactNavigationTokens.TreeIndent},*") };
        var guides = Guides(); outer.Children.Add(guides);
        var surface = Surface(); outer.Children.Add(surface); Grid.SetColumn(surface, 1);
        Child = outer;
    }

    Canvas Guides()
    {
        var canvas = new Canvas { Width = Depth * XyuiCompactNavigationTokens.TreeIndent };
        for (var level = 0; level < Depth; level++)
        {
            var active = level < ActiveGuideDepth;
            var line = Guide(active, XyuiCompactNavigationTokens.TreeRowHeight);
            canvas.Children.Add(line); Canvas.SetLeft(line, level * XyuiCompactNavigationTokens.TreeIndent + 7);
        }
        if (Depth > 0)
        {
            var active = ActiveGuideDepth >= Depth;
            var branch = Guide(active, active ? XyuiCompactNavigationTokens.TreeActiveGuideWidth : XyuiCompactNavigationTokens.TreeGuideWidth);
            branch.Width = 9; canvas.Children.Add(branch); Canvas.SetLeft(branch, Depth * XyuiCompactNavigationTokens.TreeIndent - 9); Canvas.SetTop(branch, 13.5);
        }
        return canvas;
    }

    Border Surface()
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("3,16,18,*") };
        var accent = new Border { Classes = { "xyui-tree-accent" }, IsVisible = IsSelected };
        Control chevron = HasChildren ? new XYIcon { Icon = IsExpanded ? XyuiVectorIcon.ChevronDown : XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Tiny, Classes = { "xyui-tree-chevron" } } : new Border();
        var icon = new XYIcon { Icon = Icon, Size = XyuiIconSize.Small, Classes = { "xyui-tree-icon" } };
        var label = new TextBlock { Text = Label, Classes = { "xyui-tree-label" }, VerticalAlignment = VerticalAlignment.Center, Margin = new global::Avalonia.Thickness(5, 0, 0, 0) };
        Add(grid, accent, 0); Add(grid, chevron, 1); Add(grid, icon, 2); Add(grid, label, 3);
        return new Border { Classes = { "xyui-tree-node-surface" }, Child = grid };
    }

    static Border Guide(bool active, double height) => new() { Classes = { "xyui-tree-guide", active ? "xyui-tree-guide-active" : "xyui-tree-guide-default" }, Width = active ? XyuiCompactNavigationTokens.TreeActiveGuideWidth : XyuiCompactNavigationTokens.TreeGuideWidth, Height = height };
    static void Add(Grid grid, Control control, int column) { grid.Children.Add(control); Grid.SetColumn(control, column); }
}
