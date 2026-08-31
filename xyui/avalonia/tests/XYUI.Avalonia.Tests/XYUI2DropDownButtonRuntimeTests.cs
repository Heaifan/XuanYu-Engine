using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// XYUI-2-05 DropDownButton（方案 4 · Chevron Track）运行时合同：整钮唯一命中区（含槽区域），槽不可命中且无 Divider。
[Collection("XyuiHeadless")]
public sealed partial class XYUI2DropDownButtonRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2DropDownButtonRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    internal static Button OpenZone(XYDropDownButton dropdown) =>
        dropdown.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_OpenZone");

    internal static Border Track(XYDropDownButton dropdown) =>
        (Border)dropdown.GetVisualDescendants().Single(c => c.Name == "PART_ChevronTrack");

    static void Click(Window window, Control target)
    {
        var center = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)
                     ?? new Point(target.Bounds.Width / 2, target.Bounds.Height / 2);
        window.MouseDown(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        window.MouseUp(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    [Fact]
    public void Dropdown_is_single_chrome_with_two_column_track_and_no_divider() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "导出" };
        var window = XyuiBatchTestHost.Show(dropdown);
        var grid = (Grid)dropdown.GetVisualDescendants().Single(c => c.Name == "PART_Grid");
        Assert.Equal(2, grid.ColumnDefinitions.Count);
        Assert.Equal(XYDropDownButton.ChevronTrackWidth, grid.ColumnDefinitions[1].Width.Value);
        Assert.Equal(XYDropDownButton.ChevronTrackWidth, Track(dropdown).Bounds.Width, 3);
        var inner = dropdown.Bounds.Width - dropdown.BorderThickness.Left - dropdown.BorderThickness.Right;
        Assert.Equal(inner, OpenZone(dropdown).Bounds.Width, 3);
        Assert.False(Track(dropdown).IsHitTestVisible, "Chevron 槽是纯装饰区，不得独立命中");
        Assert.False(OpenZone(dropdown).Focusable, "命中区不独立抢焦点");
        Assert.True(!dropdown.GetVisualDescendants().Any(c => c.Name == "PART_Divider"),
            "无 Divider 是与 SplitButton 的正式分界");
        Assert.Contains(dropdown.GetVisualDescendants(), c => c is XyuiActionEdge);
        Assert.Contains("xyui-icon", dropdown.GetVisualDescendants().OfType<XYIcon>().Single().Classes);
        window.Close();
    });

    [Fact]
    public void Click_anywhere_executes_open_command_through_single_zone() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var open = new SplitButtonCountingCommand();
        var dropdown = new XYDropDownButton { Content = "导出", OpenCommand = open };
        var window = XyuiBatchTestHost.Show(dropdown);
        Click(window, OpenZone(dropdown));
        Assert.Equal(1, open.Executions);
        Click(window, Track(dropdown));
        Assert.Equal(2, open.Executions);
        window.Close();
    });

    [Fact]
    public void Chevron_region_has_no_second_command_surface() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var open = new SplitButtonCountingCommand();
        var dropdown = new XYDropDownButton { Content = "导出", OpenCommand = open };
        var window = XyuiBatchTestHost.Show(dropdown);
        var commandProps = typeof(XYDropDownButton).GetProperties()
            .Where(p => typeof(ICommand).IsAssignableFrom(p.PropertyType)).ToList();
        Assert.Equal(nameof(XYDropDownButton.OpenCommand), Assert.Single(commandProps).Name);
        var parts = dropdown.GetVisualDescendants().OfType<Button>()
            .Where(b => b.Name is not null && b.Name.StartsWith("PART_")).ToList();
        Assert.Equal("PART_OpenZone", Assert.Single(parts).Name);
        Assert.Equal(0, open.Executions);
        window.Close();
    });

    [Fact]
    public void Enter_and_space_execute_open_command() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var open = new SplitButtonCountingCommand();
        var dropdown = new XYDropDownButton { Content = "导出", OpenCommand = open };
        var window = XyuiBatchTestHost.Show(dropdown);
        dropdown.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        dropdown.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Space });
        Assert.Equal(2, open.Executions);
        window.Close();
    });
}
