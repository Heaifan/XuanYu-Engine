using Avalonia;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

public sealed partial class XYUI2ButtonRuntimeTests
{
    [Fact]
    public void Chrome_height_is_34_dip_and_radius_consumes_radius_button_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(button);
        Assert.Equal(34d, button.Height);
        Assert.Equal(XyuiSpatialTokens.RadiusButton, button.CornerRadius.TopLeft);
        window.Close();
    });

    [Fact]
    public void Hover_reveals_state_hover_background() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(button);
        XyuiBatchTestHost.Hover(window, button);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(button.Background));
        window.Close();
    });

    [Fact]
    public void Pressed_reveals_state_pressed_background() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(button);
        XyuiBatchTestHost.Hover(window, button);
        var center = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)
                     ?? new Point(button.Bounds.Width / 2, button.Bounds.Height / 2);
        window.MouseDown(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Pressed"), XyuiBatchTestHost.ColorOf(button.Background));
        window.MouseUp(center, MouseButton.Left);
        window.Close();
    });
}
