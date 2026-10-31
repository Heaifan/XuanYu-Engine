using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3OverlayResourceBridgeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3OverlayResourceBridgeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Overlay_menu_reuses_canonical_application_resources_for_radio_paint() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "地图编辑", CheckKind = XyuiMenuCheckKind.Radio, IsChecked = true }; var menu = new XYMenu(item);
        menu.ApplyOverlayStyling(); var window = XyuiBatchTestHost.Show(menu); Dispatcher.UIThread.RunJobs();
        var ring = item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-ring")); var dot = item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-dot"));
        Assert.Contains(menu.Resources.MergedDictionaries.OfType<ResourceDictionary>(), x => x.ThemeDictionaries.ContainsKey(ThemeVariant.Light)); Assert.True(ring.TryFindResource("XY.Brush.Accent.Default", ThemeVariant.Light, out var resource)); Assert.IsType<SolidColorBrush>(resource); Assert.NotNull(ring.Stroke); Assert.NotNull(dot.Fill); window.Close();
    });

    [Fact]
    public void Overlay_resource_bridge_tracks_canonical_light_and_dark_theme() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var app = Application.Current!; app.RequestedThemeVariant = ThemeVariant.Light; var item = new XYMenuItem { CheckKind = XyuiMenuCheckKind.Radio, IsChecked = true }; var menu = new XYMenu(item); menu.ApplyOverlayStyling(); var window = XyuiBatchTestHost.Show(menu); Dispatcher.UIThread.RunJobs();
        var ring = item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-ring")); var token = XyuiColorTokens.All.Single(x => x.TokenId == "XY.Accent.Default");
        Assert.Equal(token.ToColor(false), Assert.IsType<SolidColorBrush>(ring.Stroke).Color); app.RequestedThemeVariant = ThemeVariant.Dark; Dispatcher.UIThread.RunJobs(); Assert.Equal(token.ToColor(true), Assert.IsType<SolidColorBrush>(ring.Stroke).Color); app.RequestedThemeVariant = ThemeVariant.Light; window.Close();
    });

    [Fact]
    public void Overlay_popup_keeps_radio_paint_across_reopen() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var trigger = new Border { Width = 120, Height = 24 }; var host = new Window { Content = trigger }; host.Show();
        var selected = new XYMenuItem { Label = "地图编辑", CheckKind = XyuiMenuCheckKind.Radio, IsChecked = true }; var other = new XYMenuItem { Label = "区域编辑", CheckKind = XyuiMenuCheckKind.Radio }; var menu = new XYMenu(selected, other);
        var popup = new Popup { PlacementTarget = trigger, Child = menu }; popup.IsOpen = true; menu.ApplyOverlayStyling(); menu.Open(); Dispatcher.UIThread.RunJobs();
        var selectedDot = Shape(selected, "xyui-menu-radio-dot"); Assert.True(popup.IsOpen); Assert.True(selectedDot.IsVisible);
        selected.IsChecked = false; other.IsChecked = true; popup.IsOpen = false; popup.IsOpen = true; menu.Open(); Dispatcher.UIThread.RunJobs(); var otherDot = Shape(other, "xyui-menu-radio-dot"); Assert.True(popup.IsOpen); Assert.True(otherDot.IsVisible); popup.IsOpen = false; host.Close();
    });

    static Ellipse Shape(XYMenuItem item, string className) => item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains(className));
}
