using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// XYUI-2-05 视觉状态：主体 Chrome 与装饰槽由控件级伪类同步；chevron 保持 Text.Secondary；
// 焦点走 Foundation Outline（Border.Color.Focus + 2 DIP）；Disabled 三件套随家族衰减。
public sealed partial class XYUI2DropDownButtonRuntimeTests
{
    [Fact]
    public void Default_chrome_matches_family_and_canonical_tokens() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "导出" };
        var window = XyuiBatchTestHost.Show(dropdown);
        Assert.Equal(34d, dropdown.Height);
        Assert.Equal(4d, dropdown.CornerRadius.TopLeft);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Raised"), XyuiBatchTestHost.ColorOf(dropdown.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Default"), XyuiBatchTestHost.ColorOf(dropdown.BorderBrush));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt"), XyuiBatchTestHost.ColorOf(Track(dropdown).Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Secondary"),
            XyuiBatchTestHost.ColorOf(ChevronOf(dropdown).Stroke));
        window.Close();
    });

    [Fact]
    public void Hover_covers_zone_and_track_while_zone_stays_transparent() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "筛选" };
        var window = XyuiBatchTestHost.Show(dropdown);
        var zone = OpenZone(dropdown);
        XyuiBatchTestHost.Hover(window, dropdown);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(dropdown.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(Track(dropdown).Background));
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(zone.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Secondary"), XyuiBatchTestHost.ColorOf(ChevronOf(dropdown).Stroke));
        window.Close();
    });

    [Fact]
    public void Pressed_covers_track_and_releases_to_hover() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "排序" };
        var window = XyuiBatchTestHost.Show(dropdown);
        var point = dropdown.TranslatePoint(new Point(dropdown.Bounds.Width / 2, dropdown.Bounds.Height / 2), window)
                    ?? new Point(dropdown.Bounds.Width / 2, dropdown.Bounds.Height / 2);
        window.MouseMove(point);
        Dispatcher.UIThread.RunJobs();
        window.MouseDown(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Pressed"), XyuiBatchTestHost.ColorOf(dropdown.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Pressed"), XyuiBatchTestHost.ColorOf(Track(dropdown).Background));
        window.MouseUp(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(dropdown.Background));
        window.Close();
    });

    [Fact]
    public void Disabled_attenuates_chrome_track_and_chevron() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "保存", IsEnabled = false };
        var window = XyuiBatchTestHost.Show(dropdown);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(dropdown.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(Track(dropdown).Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(ChevronOf(dropdown).Stroke));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Border"),
            XyuiBatchTestHost.ColorOf(XyuiBatchTestHost.Edge(dropdown).Background));
        window.Close();
    });

    [Fact]
    public void Focus_switches_to_foundation_outline_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "构建配置" };
        var window = XyuiBatchTestHost.Show(dropdown);
        dropdown.Focus();
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Focus"), XyuiBatchTestHost.ColorOf(dropdown.BorderBrush));
        Assert.Equal(2d, dropdown.BorderThickness.Left);
        window.Close();
    });

    static XYIcon ChevronOf(XYDropDownButton dropdown) =>
        dropdown.GetVisualDescendants().OfType<XYIcon>().Single();
}
