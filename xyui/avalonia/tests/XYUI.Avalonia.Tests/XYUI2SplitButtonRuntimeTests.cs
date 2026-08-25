using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

// XYUI-2-04 SplitButton（Soft Partition）运行时合同：
// 单一 Chrome + Main/Menu 两个独立 Hit Zone + 短 Divider + 跨全宽共享 Action Edge。
[Collection("XyuiHeadless")]
public sealed class XYUI2SplitButtonRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2SplitButtonRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    static Button Part(XYSplitButton split, string name) =>
        split.GetVisualDescendants().OfType<Button>().Single(b => b.Name == name);

    static void Click(Window window, Button zone)
    {
        var center = zone.TranslatePoint(new Point(zone.Bounds.Width / 2, zone.Bounds.Height / 2), window)
                     ?? new Point(zone.Bounds.Width / 2, zone.Bounds.Height / 2);
        window.MouseDown(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        window.MouseUp(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    [Fact]
    public void SplitButton_is_single_chrome_with_canonical_zone_and_divider_dimensions() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var split = new XYSplitButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(split);
        var main = Part(split, "PART_MainZone");
        var menu = Part(split, "PART_MenuZone");
        var divider = split.GetVisualDescendants().Single(c => c.Name == "PART_Divider");
        var grid = (Grid)split.GetVisualDescendants().Single(c => c.Name == "PART_Grid");
        Assert.Equal(34d, split.Height);
        Assert.Equal(XyuiSpatialTokens.RadiusButton, split.CornerRadius.TopLeft);
        Assert.Equal(XYSplitButton.MenuZoneWidth, grid.ColumnDefinitions[2].ActualWidth);
        Assert.Equal(grid.ColumnDefinitions[2].ActualWidth, menu.Bounds.Width, 3);
        Assert.Equal(XYSplitButton.DividerHeight, divider.Bounds.Height);
        Assert.False(main.Focusable, "Main 区不独立抢焦点");
        Assert.False(menu.Focusable, "Menu 区不独立抢焦点");
        window.Close();
    });

    [Fact]
    public void Main_and_menu_commands_fire_independently_without_cross_fire() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var mainCmd = new CountingCommand();
        var menuCmd = new CountingCommand();
        var split = new XYSplitButton
        {
            Content = "新建",
            MainCommand = mainCmd,
            MenuCommand = menuCmd,
        };
        var window = XyuiBatchTestHost.Show(split);
        var main = Part(split, "PART_MainZone");
        var menu = Part(split, "PART_MenuZone");

        Click(window, main);
        Assert.Equal(1, mainCmd.Executions);
        Assert.Equal(0, menuCmd.Executions);

        Click(window, menu);
        Assert.Equal(1, mainCmd.Executions);
        Assert.Equal(1, menuCmd.Executions);
        window.Close();
    });

    [Fact]
    public void Main_and_menu_hover_are_independent_zones() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var split = new XYSplitButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(split);
        var main = Part(split, "PART_MainZone");
        var menu = Part(split, "PART_MenuZone");
        var chevron = menu.GetVisualDescendants().OfType<VectorPath>().Single(p => p.Classes.Contains("xyui-icon"));
        var mainCp = main.GetVisualDescendants().OfType<ContentPresenter>().Single();
        var menuCp = menu.GetVisualDescendants().OfType<ContentPresenter>().Single();

        // MainHover：ONLY Main Zone 使用 Hover Surface；Menu Zone 与 Chevron 保持 Default。
        XyuiBatchTestHost.Hover(window, main);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(main.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(mainCp.Background));
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(menu.Background));
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(menuCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Secondary"), XyuiBatchTestHost.ColorOf(chevron.Stroke));

        // MenuHover：ONLY Menu Zone 使用 Hover Surface；Main Zone 保持 Default；Chevron 独立 Accent。
        XyuiBatchTestHost.Hover(window, menu);
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(main.Background));
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(mainCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(menu.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(menuCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(chevron.Stroke));
        window.Close();
    });

    [Fact]
    public void SplitButton_shares_single_action_edge_across_full_width() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var split = new XYSplitButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(split);
        var edge = XyuiBatchTestHost.Edge(split);
        Assert.True(edge.IsVisible);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(edge.Background));
        var inner = split.Bounds.Width - split.BorderThickness.Left - split.BorderThickness.Right;
        Assert.Equal(inner, edge.Bounds.Width, 3);
        window.Close();
    });

    sealed class CountingCommand : ICommand
    {
        public int Executions { get; private set; }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => Executions++;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
