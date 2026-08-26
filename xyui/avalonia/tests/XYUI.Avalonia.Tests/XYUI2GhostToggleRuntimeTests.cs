using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

// XYUI-2-02 IconButton（Selected≠Checked）与 XYUI-2-03 ToggleButton（IsChecked→Persistent Edge）运行时合同。
[Collection("XyuiHeadless")]
public sealed partial class XYUI2GhostToggleRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2GhostToggleRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void IconButton_stays_button_with_independent_selected_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYIconButton();
        Assert.IsAssignableFrom<Button>(button);
        Assert.False(typeof(ToggleButton).IsAssignableFrom(typeof(XYIconButton)), "IconButton 是 Command，不得继承 ToggleButton");
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.False(button.IsSelected, "点击 IconButton 不得自动改变 Selected");
        button.IsSelected = true;
        Assert.Contains(":selected", button.Classes);
    });

    [Fact]
    public void IconButton_default_is_ghost_and_selected_reveals_surface_and_edge() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        var window = XyuiBatchTestHost.Show(button);
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(button.Background));
        Assert.False(XyuiBatchTestHost.Edge(button).IsVisible, "Ghost 默认不显示容器与 Action Edge");
        Assert.Equal(34d, button.Width);
        Assert.Equal(34d, button.Height);
        button.IsSelected = true;
        Assert.True(XyuiBatchTestHost.Edge(button).IsVisible);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Selected"), XyuiBatchTestHost.ColorOf(button.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(XyuiBatchTestHost.Edge(button).Background));
        window.Close();
    });

    [Fact]
    public void ToggleButton_checked_drives_persistent_edge_border_and_background() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var toggle = new XYToggleButton { Content = "网格吸附" };
        var window = XyuiBatchTestHost.Show(toggle);
        var edge = XyuiBatchTestHost.Edge(toggle);
        Assert.False(edge.IsVisible, "OFF 不显示 Action Edge");
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Raised"), XyuiBatchTestHost.ColorOf(toggle.Background));
        toggle.IsChecked = true;
        Assert.True(edge.IsVisible, "ON 必须持续显示 Action Edge");
        Assert.Equal(XyuiActionEdge.DefaultHeight, edge.Height);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Active"), XyuiBatchTestHost.ColorOf(toggle.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Selected"), XyuiBatchTestHost.ColorOf(toggle.BorderBrush));
        XyuiBatchTestHost.Hover(window, toggle);
        Assert.Equal(XyuiActionEdge.HoverHeight, edge.Height);
        window.Close();
    });

}
