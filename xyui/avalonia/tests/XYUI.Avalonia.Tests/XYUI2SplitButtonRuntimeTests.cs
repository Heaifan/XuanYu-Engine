using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
namespace XYUI.Avalonia.Tests;

// XYUI-2-04 SplitButton（Compact Icon Well）运行时合同：
// 单一 Chrome + Main/Menu 两个独立 Hit Zone + 固定图标槽 + 短 Divider。
[Collection("XyuiHeadless")]
public sealed partial class XYUI2SplitButtonRuntimeTests : IClassFixture<XyuiHeadlessFixture>
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
        Assert.Equal(36d, split.Height);
        Assert.Equal(3d, split.CornerRadius.TopLeft);
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
        var mainCmd = new SplitButtonCountingCommand();
        var menuCmd = new SplitButtonCountingCommand();
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
    public void SplitButton_has_no_permanent_action_edge_and_disabled_zones_attenuate() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var split = new XYSplitButton { Content = "新建", IsEnabled = false };
        var window = XyuiBatchTestHost.Show(split);
        Assert.DoesNotContain(split.GetVisualDescendants(), x => x is XyuiActionEdge);
        var menu = Part(split, "PART_MenuZone");
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(menu.Background));
        window.Close();
    });

    [Fact]
    public void Enter_and_space_execute_only_the_primary_command() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var command = new SplitButtonCountingCommand();
        var split = new XYSplitButton { Content = "新建", MainCommand = command };
        var window = XyuiBatchTestHost.Show(split);
        split.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        split.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Space });
        Assert.Equal(2, command.Executions);
        window.Close();
    });

}
