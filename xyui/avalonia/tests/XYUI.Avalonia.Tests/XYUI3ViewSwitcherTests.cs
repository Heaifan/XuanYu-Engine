using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3ViewSwitcherTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    static XYViewDefinition[] Views => [new("canvas", "画布", XyuiVectorIcon.Locate, Priority: 3), new("table", "表格", XyuiVectorIcon.Section, Priority: 2), new("preview", "预览", XyuiVectorIcon.Eye, Priority: 1), new("log", "日志", XyuiVectorIcon.Code, Priority: -1)];
    public XYUI3ViewSwitcherTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact] public void Segmented_has_single_outer_surface_and_30_DIP_segments() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "canvas")); var window = XyuiBatchTestHost.Show(switcher); Dispatcher.UIThread.RunJobs(); Assert.Equal(36, switcher.Height); Assert.Equal(1, switcher.BorderThickness.Left); Assert.All(((StackPanel)switcher.Child!).Children.OfType<XYButton>(), x => Assert.Equal(30, x.Height)); window.Close(); });
    [Fact] public void Selected_segment_has_one_visible_bottom_accent() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "canvas")); var window = XyuiBatchTestHost.Show(switcher); var selected = ((StackPanel)switcher.Child!).Children.OfType<XYButton>().Single(x => x.Classes.Contains("xyui-view-selected")); Assert.Single(selected.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-view-segment-accent") && x.IsVisible); window.Close(); });
    [Fact] public void Request_requires_accept_and_rejected_request_keeps_current() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "canvas")); switcher.SelectView("table"); Assert.Equal("canvas", switcher.CurrentViewId); switcher.ViewChangeRequested += (_, request) => request.Reject(); switcher.SelectView("table"); Assert.Equal("canvas", switcher.CurrentViewId); var accepted = new XYViewSwitcher(new XYViewState(Views, "canvas")); accepted.ViewChangeRequested += (_, request) => request.Accept(); accepted.SelectView("table"); Assert.Equal("table", accepted.CurrentViewId); });
    [Fact] public void Current_click_does_not_raise_duplicate_change() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "canvas")); var changes = 0; switcher.ViewChangeRequested += (_, request) => { changes++; request.Accept(); }; switcher.SelectView("canvas"); Assert.Equal(0, changes); });
    [Fact] public void Dropdown_check_uses_id_binding_and_actual_trigger() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "canvas"), XYViewSwitcherVariant.Dropdown); var window = XyuiBatchTestHost.Show(switcher); var trigger = ((StackPanel)switcher.Child!).Children.OfType<XYButton>().Single(); switcher.Open(trigger); var current = switcher.ViewMenu.Items.OfType<XYMenuItem>().Single(x => x.Label == "画布"); Assert.True(current.IsSelected); Assert.True(current.IsChecked); Assert.Same(trigger, switcher.ViewPopup.PlacementTarget); window.Close(); });
    [Fact] public void More_is_active_when_current_view_is_hidden() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views, "log"), XYViewSwitcherVariant.PrimaryMore); var more = ((StackPanel)switcher.Child!).Children.OfType<XYIconButton>().Single(); Assert.True(more.IsSelected); });
    [Fact] public void Escape_closes_popup() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var switcher = new XYViewSwitcher(new XYViewState(Views)); var window = XyuiBatchTestHost.Show(switcher); switcher.Open(switcher); switcher.ViewMenu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(switcher.ViewPopup.IsOpen); window.Close(); });
}
