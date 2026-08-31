using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYTabBar : Border
{
    public XYTabs Tabs { get; }
    public XYIconButton PreviousButton { get; }
    public XYIconButton NextButton { get; }
    public XYIconButton OverflowButton { get; }
    public XYIconButton NewButton { get; }

    public XYTabBar(params XYTab[] tabs)
    {
        Classes.Add("xyui-tab-bar");
        Tabs = new XYTabs(tabs);
        PreviousButton = Action(XyuiVectorIcon.ChevronLeft, "xyui-tab-bar-previous");
        NextButton = Action(XyuiVectorIcon.ChevronRight, "xyui-tab-bar-next");
        OverflowButton = Action(XyuiVectorIcon.MoreHorizontal, "xyui-tab-bar-overflow");
        NewButton = Action(XyuiVectorIcon.Add, "xyui-tab-bar-new");
        Child = Build();
    }

    Grid Build()
    {
        var viewport = new ScrollViewer
        {
            Content = Tabs,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Classes = { "xyui-tab-viewport" }
        };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("32,*,32,40,50") };
        Add(grid, PreviousButton, 0); Add(grid, viewport, 1); Add(grid, NextButton, 2);
        Add(grid, OverflowButton, 3); Add(grid, NewButton, 4); return grid;
    }

    static XYIconButton Action(XyuiVectorIcon icon, string styleClass) => new()
    {
        Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small },
        Classes = { "xyui-tab-bar-action", styleClass }
    };

    static void Add(Grid grid, Control control, int column)
    { grid.Children.Add(control); Grid.SetColumn(control, column); }
}
