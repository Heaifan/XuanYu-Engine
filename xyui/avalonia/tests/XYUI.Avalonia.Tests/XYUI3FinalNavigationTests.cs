using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3FinalNavigationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3FinalNavigationTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact] public void ViewSwitcher_request_then_commit_and_variants_share_state()
    {
        var views = new[] { new XYViewDefinition("a", "画布", XyuiVectorIcon.Locate), new XYViewDefinition("b", "表格", XyuiVectorIcon.Section) }; var state = new XYViewState(views, "a"); var segmented = new XYViewSwitcher(state); var dropdown = new XYViewSwitcher(state, XYViewSwitcherVariant.Dropdown); segmented.ViewChangeRequested += (_, request) => request.Accept(); segmented.SelectView("b"); Assert.Equal("b", dropdown.CurrentViewId); Assert.Same(state, dropdown.State);
    }
    [Fact] public void Toc_limits_depth_and_rejects_request()
    {
        var sections = new[] { new XYTocSection("a", "A", 1), new XYTocSection("b", "B", 3), new XYTocSection("c", "C", 2, "a") }; var toc = new XYTableOfContents(sections); Assert.Equal(2, toc.State.Sections.Count); toc.SectionRequested += (_, request) => request.Reject(); toc.SelectSection("c"); Assert.Equal("a", toc.CurrentSectionId);
    }
    [Fact] public void Bottom_navigation_keeps_primary_action_out_of_destination_state()
    {
        var primary = new XYButton(); var items = new[] { new XYBottomNavigationItem("home", "首页", XyuiVectorIcon.Locate) }; var nav = new XYBottomNavigation(new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon))), items, primary); Assert.Same(primary, nav.PrimaryAction); Assert.Single(nav.NavigationState.Entries);
    }
    [Fact] public void Bottom_navigation_has_equal_vertical_destination_slots() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var items = new[] { new XYBottomNavigationItem("a", "地图", XyuiVectorIcon.Locate), new XYBottomNavigationItem("b", "数据", XyuiVectorIcon.Code), new XYBottomNavigationItem("c", "实验", XyuiVectorIcon.Clear), new XYBottomNavigationItem("d", "日志", XyuiVectorIcon.Section), new XYBottomNavigationItem("e", "我的", XyuiVectorIcon.Info) }; var nav = new XYBottomNavigation(new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon)), "a"), items); var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var slots = nav.GetVisualDescendants().OfType<Border>().Where(x => x.Classes.Contains("xyui-bottom-navigation-destination")).ToArray(); Assert.Equal(5, slots.Length); Assert.All(slots, x => Assert.InRange(Math.Abs(slots[0].Bounds.Width - x.Bounds.Width), 0, 1)); Assert.Equal(66, nav.Bounds.Height, 1); window.Close(); });
    [Fact] public void Bottom_navigation_content_is_icon_above_label_and_badge_overlays() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYBottomNavigationItem("logs", "日志", XyuiVectorIcon.Section, "1"); var nav = new XYBottomNavigation([item]); var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var slot = nav.GetVisualDescendants().OfType<Border>().Single(x => x.Classes.Contains("xyui-bottom-navigation-destination")); var icon = slot.GetVisualDescendants().OfType<XYIcon>().Single(); var label = slot.GetVisualDescendants().OfType<TextBlock>().Single(); Assert.True(icon.Bounds.Center.Y < label.Bounds.Center.Y); Assert.NotEmpty(slot.GetVisualDescendants().OfType<XYStatusDot>()); window.Close(); });
    [Fact] public void Bottom_navigation_primary_action_does_not_select_destination() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code), new("c", "日志", XyuiVectorIcon.Section), new("d", "我的", XyuiVectorIcon.Info)], "a"); var primary = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add } }; var nav = new XYBottomNavigation(state, state.Entries.Select(x => new XYBottomNavigationItem(x.Id, x.Label, x.Icon)), primary); var requested = 0; nav.PrimaryActionRequested += (_, _) => requested++; var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); primary.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, requested); Assert.Equal("a", nav.CurrentDestinationId); Assert.Contains(nav.GetVisualDescendants().OfType<Grid>(), x => x.Classes.Contains("xyui-bottom-navigation-primary-host")); window.Close(); });
    [Fact] public void Bottom_navigation_accepts_destination_and_rejects_without_state_change() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code)], "a"); var nav = new XYBottomNavigation(state); nav.DestinationRequested += (_, request) => { if (request.Destination.Id == "b") request.Accept(); }; nav.SelectDestination("b"); Assert.Equal("b", nav.CurrentDestinationId); nav.DestinationRequested += (_, request) => request.Reject(); nav.SelectDestination("a"); Assert.Equal("b", nav.CurrentDestinationId); });
    [Fact] public void Bottom_navigation_current_destination_does_not_duplicate_request() => _fx.Run(() => { var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate)], "a"); var nav = new XYBottomNavigation(state); var count = 0; nav.DestinationRequested += (_, request) => { count++; request.Accept(); }; nav.SelectDestination("a"); Assert.Equal(0, count); });
    [Fact] public void Drawer_shares_navigation_state_and_has_lifecycle_surface()
    {
        var state = new XYNavigationState([new("home", "首页", XyuiVectorIcon.Locate)]); var drawer = new XYNavigationDrawer(state); Assert.Same(state, drawer.NavigationState); drawer.Open(); Assert.True(drawer.IsOpen); drawer.Close(); Assert.False(drawer.IsOpen); Assert.NotNull(drawer.Backdrop);
    }
    [Fact] public void Gallery_registers_all_final_components()
    { foreach (var id in new[] { "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24" }) Assert.NotNull(XYUI3GalleryCatalog.CreatePreview(id)); }
}
