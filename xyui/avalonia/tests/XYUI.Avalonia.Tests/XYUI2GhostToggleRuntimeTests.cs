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
public sealed class XYUI2GhostToggleRuntimeTests : IClassFixture<XyuiHeadlessFixture>
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

    [Fact]
    public void IconButton_default_is_ghost_with_no_border_and_radius_consumes_radius_button() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        var window = XyuiBatchTestHost.Show(button);
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(button.BorderBrush));
        Assert.Equal(XyuiSpatialTokens.RadiusButton, button.CornerRadius.TopLeft);
        window.Close();
    });

    [Fact]
    public void IconButton_hover_reveals_hover_surface_and_primary_icon() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        var window = XyuiBatchTestHost.Show(button);
        XyuiBatchTestHost.Hover(window, button);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(button.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Primary"), XyuiBatchTestHost.ColorOf(button.Foreground));
        window.Close();
    });

    [Fact]
    public void IconButton_selected_border_uses_border_selected_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Code } };
        var window = XyuiBatchTestHost.Show(button);
        button.IsSelected = true;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Selected"), XyuiBatchTestHost.ColorOf(button.BorderBrush));
        window.Close();
    });

    [Fact]
    public void ToggleButton_inherits_togglebutton_semantics() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var toggle = new XYToggleButton { Content = "网格吸附" };
        Assert.IsAssignableFrom<ToggleButton>(toggle);
        toggle.IsChecked = true;
        Assert.Contains(":checked", toggle.Classes);
        toggle.IsChecked = false;
        Assert.DoesNotContain(":checked", toggle.Classes);
    });

    [Fact]
    public void ToggleButton_off_hover_and_disabled_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var toggle = new XYToggleButton { Content = "网格吸附" };
        var window = XyuiBatchTestHost.Show(toggle);

        XyuiBatchTestHost.Hover(window, toggle);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(toggle.Background));
        XyuiBatchTestHost.Hover(window, toggle);
        window.MouseMove(new Point(-50, -50));
        Dispatcher.UIThread.RunJobs();

        toggle.IsEnabled = false;
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(toggle.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Border"), XyuiBatchTestHost.ColorOf(XyuiBatchTestHost.Edge(toggle).Background));
        window.Close();
    });
}



