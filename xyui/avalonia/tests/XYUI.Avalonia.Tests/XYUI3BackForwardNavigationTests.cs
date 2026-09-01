using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3BackForwardNavigationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3BackForwardNavigationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Surface_owns_content_and_compact_geometry_is_real() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); var window = XyuiBatchTestHost.Show(nav); var surface = Assert.IsType<Border>(nav.Child); var inner = Assert.IsType<Grid>(surface.Child);
        Assert.Equal(34, nav.Height); Assert.Equal(34, surface.Height); Assert.Equal(28, nav.BackButton.Width); Assert.Equal(28, nav.ForwardButton.Height); Assert.Same(nav.BackButton, inner.Children[0]); Assert.Same(nav.ForwardButton, inner.Children[1]); Assert.Equal(VerticalAlignment.Center, nav.BackButton.VerticalAlignment); Assert.Equal(VerticalAlignment.Center, nav.ForwardButton.VerticalAlignment); window.Close();
    });

    [Fact] public void Empty_history_shows_empty_location() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); var location = nav.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-location-text")); Assert.Equal("—", location.Text); Assert.False(nav.CanGoBack); Assert.False(nav.CanGoForward);
    });

    [Fact] public void Back_forward_and_new_navigation_keep_location_and_branch() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); var seen = new List<string>(); nav.LocationChanged += (_, value) => seen.Add(value); nav.Navigate("地图"); nav.Navigate("数据集"); nav.Navigate("roads"); nav.Back(); Assert.Equal("数据集", nav.CurrentLocation); nav.Forward(); Assert.Equal("roads", nav.CurrentLocation); nav.Back(); nav.Navigate("广东省"); Assert.False(nav.CanGoForward); Assert.Equal("广东省", nav.CurrentLocation); Assert.Equal(["地图", "数据集", "roads", "数据集", "roads", "数据集", "广东省"], seen);
    });

    [Fact] public void Right_click_back_opens_xy_menu_and_jump_updates_index() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); nav.Navigate("地图"); nav.Navigate("数据集"); nav.Navigate("roads"); nav.Navigate("广东省"); var window = XyuiBatchTestHost.Show(nav); var point = nav.BackButton.TranslatePoint(new Point(14, 14), window)!.Value; window.MouseDown(point, MouseButton.Right); Dispatcher.UIThread.RunJobs(); Assert.True(nav.BackHistoryPopup.IsOpen); Assert.All(nav.BackHistoryMenu.Items.Where(x => x is XYMenuItem), x => Assert.IsType<XYMenuItem>(x)); Assert.Equal("roads", ((XYMenuItem)nav.BackHistoryMenu.Items[0]).Label); ((XYMenuItem)nav.BackHistoryMenu.Items[1]).Activate(); Assert.Equal("数据集", nav.CurrentLocation); Assert.Equal(1, nav.CurrentIndex); Assert.False(nav.BackHistoryPopup.IsOpen); window.Close();
    });

    [Fact] public void Escape_closes_history_and_alt_keys_follow_enabled_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); nav.Navigate("一"); nav.Navigate("二"); var window = XyuiBatchTestHost.Show(nav); var point = nav.BackButton.TranslatePoint(new Point(14, 14), window)!.Value; window.MouseDown(point, MouseButton.Right); Dispatcher.UIThread.RunJobs(); nav.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(nav.BackHistoryPopup.IsOpen); nav.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Left, KeyModifiers = KeyModifiers.Alt }); Assert.Equal("一", nav.CurrentLocation); nav.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Left, KeyModifiers = KeyModifiers.Alt }); Assert.Equal("一", nav.CurrentLocation); nav.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Right, KeyModifiers = KeyModifiers.Alt }); Assert.Equal("二", nav.CurrentLocation); window.Close();
    });

    [Fact] public void Detach_closes_history_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); nav.Navigate("一"); nav.Navigate("二"); var window = XyuiBatchTestHost.Show(nav); var point = nav.BackButton.TranslatePoint(new Point(14, 14), window)!.Value; window.MouseDown(point, MouseButton.Right); Dispatcher.UIThread.RunJobs(); Assert.True(nav.BackHistoryPopup.IsOpen); window.Close(); Assert.False(nav.BackHistoryPopup.IsOpen);
    });
}
