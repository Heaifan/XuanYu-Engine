using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

public sealed partial class XYUI2GhostToggleRuntimeTests
{
    [Fact]
    public void IconButton_default_is_ghost_with_no_border_and_radius_consumes_radius_button() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        var window = XyuiBatchTestHost.Show(button);
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(button.BorderBrush));
        Assert.Equal(XyuiSpatialTokens.RadiusButton, button.CornerRadius.TopLeft); window.Close();
    });

    [Fact]
    public void IconButton_hover_reveals_hover_surface_and_primary_icon() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        var window = XyuiBatchTestHost.Show(button); XyuiBatchTestHost.Hover(window, button);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(button.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Primary"), XyuiBatchTestHost.ColorOf(button.Foreground)); window.Close();
    });

    [Fact]
    public void IconButton_selected_border_uses_border_selected_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var button = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Code } };
        var window = XyuiBatchTestHost.Show(button); button.IsSelected = true;
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Selected"), XyuiBatchTestHost.ColorOf(button.BorderBrush)); window.Close();
    });

    [Fact]
    public void ToggleButton_inherits_togglebutton_semantics() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var toggle = new XYToggleButton { Content = "网格吸附" };
        Assert.IsAssignableFrom<ToggleButton>(toggle); toggle.IsChecked = true; Assert.Contains(":checked", toggle.Classes);
        toggle.IsChecked = false; Assert.DoesNotContain(":checked", toggle.Classes);
    });

    [Fact]
    public void ToggleButton_off_hover_and_disabled_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var toggle = new XYToggleButton { Content = "网格吸附" };
        var window = XyuiBatchTestHost.Show(toggle); XyuiBatchTestHost.Hover(window, toggle);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(toggle.Background));
        window.MouseMove(new Point(-50, -50)); Dispatcher.UIThread.RunJobs(); toggle.IsEnabled = false;
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(toggle.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Border"), XyuiBatchTestHost.ColorOf(XyuiBatchTestHost.Edge(toggle).Background)); window.Close();
    });
}
