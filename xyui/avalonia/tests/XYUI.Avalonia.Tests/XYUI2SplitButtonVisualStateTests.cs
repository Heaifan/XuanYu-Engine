using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed partial class XYUI2SplitButtonRuntimeTests
{
    [Fact]
    public void Main_and_menu_hover_are_independent_zones() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var split = new XYSplitButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(split);
        var main = Part(split, "PART_MainZone");
        var menu = Part(split, "PART_MenuZone");
        var chevron = menu.GetVisualDescendants().OfType<XYIcon>().Single(p => p.Classes.Contains("xyui-icon"));
        var mainCp = main.GetVisualDescendants().OfType<ContentPresenter>().Single();
        var menuCp = menu.GetVisualDescendants().OfType<ContentPresenter>().Single();
        XyuiBatchTestHost.Hover(window, main);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(main.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(mainCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt"), XyuiBatchTestHost.ColorOf(menu.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.PanelAlt"), XyuiBatchTestHost.ColorOf(menuCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Secondary"), XyuiBatchTestHost.ColorOf(chevron.Stroke));
        XyuiBatchTestHost.Hover(window, menu);
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(main.Background));
        Assert.Equal(Colors.Transparent, XyuiBatchTestHost.ColorOf(mainCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(menu.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(menuCp.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(chevron.Stroke));
        window.Close();
    });
}
