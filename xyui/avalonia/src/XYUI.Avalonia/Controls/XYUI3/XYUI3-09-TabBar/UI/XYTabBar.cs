using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTabBar : Border
{
    readonly ScrollViewer _viewport;
    readonly Popup _overflowPopup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Height = 0, IsVisible = false };
    public XYTabs Tabs { get; }
    public XYIconButton PreviousButton { get; }
    public XYIconButton NextButton { get; }
    public XYIconButton OverflowButton { get; }
    public XYIconButton NewButton { get; }
    public Popup OverflowPopup => _overflowPopup;
    public double HorizontalOffset => _viewport.Offset.X;

    public XYTabBar(params XYTab[] tabs)
    {
        Classes.Add("xyui-tab-bar");
        Tabs = new XYTabs(tabs) { VerticalAlignment = VerticalAlignment.Center };
        PreviousButton = Action(XyuiVectorIcon.ChevronLeft, "xyui-tab-bar-previous");
        NextButton = Action(XyuiVectorIcon.ChevronRight, "xyui-tab-bar-next");
        OverflowButton = Action(XyuiVectorIcon.MoreHorizontal, "xyui-tab-bar-overflow");
        NewButton = Action(XyuiVectorIcon.Add, "xyui-tab-bar-new");
        _viewport = Viewport(); Child = Build(); InitializeInteraction();
    }

    Grid Build()
    {
        var viewport = _viewport;
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("32,*,32,40,50") };
        Add(grid, PreviousButton, 0); Add(grid, viewport, 1); Add(grid, NextButton, 2);
        Add(grid, OverflowButton, 3); Add(grid, NewButton, 4); Add(grid, _overflowPopup, 3); return grid;
    }

    ScrollViewer Viewport() => new()
        {
            Content = Tabs,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Classes = { "xyui-tab-viewport" }
        };

    static XYIconButton Action(XyuiVectorIcon icon, string styleClass) => new()
    {
        Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small },
        Classes = { "xyui-tab-bar-action", styleClass }
    };

    static void Add(Grid grid, Control control, int column)
    { grid.Children.Add(control); Grid.SetColumn(control, column); }
}
