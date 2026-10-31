using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
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
    [Fact] public void Bottom_navigation_has_equal_vertical_destination_slots() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var items = new[] { new XYBottomNavigationItem("a", "地图", XyuiVectorIcon.Locate), new XYBottomNavigationItem("b", "数据", XyuiVectorIcon.Code), new XYBottomNavigationItem("c", "实验", XyuiVectorIcon.Clear), new XYBottomNavigationItem("d", "日志", XyuiVectorIcon.Section, "12"), new XYBottomNavigationItem("e", "我的", XyuiVectorIcon.Info) }; var nav = new XYBottomNavigation(new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon)), "a"), items) { Width = 360 }; var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var slots = nav.GetVisualDescendants().OfType<Border>().Where(x => x.Classes.Contains("xyui-bottom-navigation-destination")).ToArray(); Assert.Equal(5, slots.Length); Assert.All(slots, x => Assert.InRange(Math.Abs(slots[0].Bounds.Width - x.Bounds.Width), 0, 1)); Assert.Equal(64, nav.Bounds.Height, 1); window.Close(); });
    [Fact] public void Bottom_navigation_content_is_icon_above_label_and_badge_count_overlays() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var item = new XYBottomNavigationItem("logs", "日志", XyuiVectorIcon.Section, "12"); var nav = new XYBottomNavigation([item]); var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var slot = nav.GetVisualDescendants().OfType<Border>().Single(x => x.Classes.Contains("xyui-bottom-navigation-destination")); var icon = slot.GetVisualDescendants().OfType<XYIcon>().Single(); var label = slot.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-bottom-navigation-label")); var badge = slot.GetVisualDescendants().OfType<XYStatusBadge>().Single(); var well = slot.GetVisualDescendants().OfType<Border>().Single(x => x.Classes.Contains("xyui-bottom-navigation-icon-well")); Assert.True(icon.Bounds.Center.Y < label.Bounds.Center.Y); Assert.Equal("12", badge.Text); Assert.InRange(badge.Bounds.Height, 14, 16); Assert.Equal(36, well.Bounds.Width, 1); Assert.Equal(28, well.Bounds.Height, 1); Assert.Contains("xyui-bottom-navigation-selected-well", well.Classes); window.Close(); });
    [Fact] public void Bottom_navigation_selected_well_does_not_resize_slots() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var items = new[] { new XYBottomNavigationItem("a", "地图", XyuiVectorIcon.Locate), new XYBottomNavigationItem("b", "数据", XyuiVectorIcon.Code), new XYBottomNavigationItem("c", "实验", XyuiVectorIcon.Clear), new XYBottomNavigationItem("d", "日志", XyuiVectorIcon.Section), new XYBottomNavigationItem("e", "我的", XyuiVectorIcon.Info) }; var state = new XYNavigationState(items.Select(i => new XYNavigationEntry(i.Id, i.Label, i.Icon)), "a"); var nav = new XYBottomNavigation(state, items) { Width = 360 }; var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var before = nav.GetVisualDescendants().OfType<Border>().Where(x => x.Classes.Contains("xyui-bottom-navigation-destination")).Select(x => x.Bounds.Width).ToArray(); state.Select("e"); Dispatcher.UIThread.RunJobs(); var after = nav.GetVisualDescendants().OfType<Border>().Where(x => x.Classes.Contains("xyui-bottom-navigation-destination")).Select(x => x.Bounds.Width).ToArray(); Assert.Equal(before.Length, after.Length); Assert.All(before, width => Assert.Equal(before[0], width, 1)); Assert.All(after, width => Assert.Equal(after[0], width, 1)); Assert.Equal(before[0], after[0], 1); window.Close(); });
    [Fact] public void Bottom_navigation_safe_area_adds_inset_without_shrinking_destination() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var items = new[] { new XYBottomNavigationItem("a", "地图", XyuiVectorIcon.Locate), new XYBottomNavigationItem("b", "数据", XyuiVectorIcon.Code), new XYBottomNavigationItem("c", "实验", XyuiVectorIcon.Clear), new XYBottomNavigationItem("d", "日志", XyuiVectorIcon.Section), new XYBottomNavigationItem("e", "我的", XyuiVectorIcon.Info) }; var nav = new XYBottomNavigation(items) { Width = 360, SafeAreaBottom = 24 }; var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); var slots = nav.GetVisualDescendants().OfType<Border>().Where(x => x.Classes.Contains("xyui-bottom-navigation-destination")).ToArray(); Assert.Equal(88, nav.Bounds.Height, 1); Assert.All(slots, x => Assert.InRange(Math.Abs(slots[0].Bounds.Width - x.Bounds.Width), 0, 1)); Assert.All(slots, x => Assert.True(x.Bounds.Height >= 50)); window.Close(); });
    [Fact] public void Bottom_navigation_primary_action_does_not_select_destination() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code), new("c", "日志", XyuiVectorIcon.Section), new("d", "我的", XyuiVectorIcon.Info)], "a"); var primary = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add } }; var nav = new XYBottomNavigation(state, state.Entries.Select(x => new XYBottomNavigationItem(x.Id, x.Label, x.Icon)), primary); var requested = 0; nav.PrimaryActionRequested += (_, _) => requested++; var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); primary.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, requested); Assert.Equal("a", nav.CurrentDestinationId); Assert.Contains(nav.GetVisualDescendants().OfType<Grid>(), x => x.Classes.Contains("xyui-bottom-navigation-primary-host")); window.Close(); });
    [Fact] public void Bottom_navigation_safe_area_and_primary_hit_target_are_explicit() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code)], "a"); var primary = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add } }; var nav = new XYBottomNavigation(state, state.Entries.Select(x => new XYBottomNavigationItem(x.Id, x.Label, x.Icon)), primary); var window = XyuiBatchTestHost.Show(nav); nav.SafeAreaBottom = 0; Dispatcher.UIThread.RunJobs(); var baseHeight = nav.Bounds.Height; nav.SafeAreaBottom = 24; Dispatcher.UIThread.RunJobs(); var host = nav.GetVisualDescendants().OfType<Grid>().Single(x => x.Classes.Contains("xyui-bottom-navigation-primary-host")); var contentGrid = Assert.IsType<Grid>(nav.Child); Assert.Equal(64, baseHeight, 1); Assert.Equal(88, nav.Bounds.Height, 1); Assert.Equal(24, contentGrid.Margin.Bottom, 1); Assert.True(primary.IsHitTestVisible); Assert.Equal(48, primary.Width); Assert.Equal(48, primary.Height); Assert.False(nav.ClipToBounds); Assert.False(host.ClipToBounds); Assert.Equal(-16, host.Margin.Top); window.Close(); });
    [Fact] public void Bottom_navigation_accepts_destination_and_rejects_without_state_change() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code)], "a"); var nav = new XYBottomNavigation(state); nav.DestinationRequested += (_, request) => { if (request.Destination.Id == "b") request.Accept(); }; nav.SelectDestination("b"); Assert.Equal("b", nav.CurrentDestinationId); var slotA = nav.GetVisualDescendants().OfType<Border>().First(x => x.Classes.Contains("xyui-bottom-navigation-destination")); var wA = slotA.Bounds.Width; nav.DestinationRequested += (_, request) => request.Reject(); nav.SelectDestination("a"); Assert.Equal("b", nav.CurrentDestinationId); Assert.Equal(wA, slotA.Bounds.Width); });
    [Fact] public void Bottom_navigation_current_destination_does_not_duplicate_request() => _fx.Run(() => { var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate)], "a"); var nav = new XYBottomNavigation(state); var count = 0; nav.DestinationRequested += (_, request) => { count++; request.Accept(); }; nav.SelectDestination("a"); Assert.Equal(0, count); });
    [Fact] public void Drawer_host_authority_and_geometry_bounds_and_hit_test() => _fx.Run(() => {
        XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code)], "map");
        var drawer = new XYNavigationDrawer(state); var host = new Border { Classes = { "xyui-navigation-drawer-host" }, Width = 640, Height = 270, Child = drawer };
        var window = XyuiBatchTestHost.Show(host); Dispatcher.UIThread.RunJobs(); drawer.Open(); Dispatcher.UIThread.RunJobs();
        Assert.Equal(640, drawer.Backdrop.Bounds.Width, 1); Assert.Equal(270, drawer.Backdrop.Bounds.Height, 1);
        Assert.Equal(280, drawer.DrawerSurface.Bounds.Width, 1); Assert.Equal(270, drawer.DrawerSurface.Bounds.Height, 1);
        var leftInWin = drawer.DrawerSurface.TranslatePoint(new Point(0, 0), window)!.Value.X; var hostLeft = host.TranslatePoint(new Point(0, 0), window)!.Value.X;
        Assert.Equal(hostLeft, leftInWin, 1); Assert.Equal(hostLeft + 280, drawer.DrawerSurface.TranslatePoint(new Point(drawer.DrawerSurface.Bounds.Width, 0), window)!.Value.X, 1);
        Assert.Same(host, drawer.Host); Assert.Same(host, drawer.DrawerPopup.PlacementTarget); Assert.False(ReferenceEquals(window, drawer.Host));
        Assert.False(drawer.Backdrop.Bounds.Contains(new Point(750, 140)));
        Assert.True(drawer.Backdrop.Bounds.Contains(new Point(400, 100)));
        Assert.False(drawer.DrawerSurface.Bounds.Contains(new Point(400, 100)));
        Assert.True(drawer.Backdrop.IsHitTestVisible);
        window.Close();
    });
    [Fact] public void Drawer_lifecycle_light_dismiss_escape_focus_and_navigation_sync() => _fx.Run(() => {
        XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code)], "map");
        var drawer = new XYNavigationDrawer(state); var host = new Border { Classes = { "xyui-navigation-drawer-host" }, Width = 640, Height = 270, Child = drawer };
        var window = XyuiBatchTestHost.Show(host); Dispatcher.UIThread.RunJobs(); drawer.Open(); Dispatcher.UIThread.RunJobs(); Assert.True(drawer.IsOpen);
        drawer.Backdrop.RaiseEvent(new PointerPressedEventArgs(drawer.Backdrop, null!, window, new Point(400, 100), 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None)); Dispatcher.UIThread.RunJobs();
        Assert.False(drawer.IsOpen); drawer.Open(); Dispatcher.UIThread.RunJobs();
        drawer.DrawerSurface.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
        Assert.False(drawer.IsOpen); Dispatcher.UIThread.RunJobs(); Assert.True(drawer.OpenTrigger.IsFocused);
        state.Select("data"); Assert.Equal("data", drawer.NavigationState.SelectedId);
        drawer.SelectDestination("map"); Assert.Equal("map", state.SelectedId); window.Close();
    });
    [Fact] public void Drawer_without_host_stays_closed_without_fallback_to_window()
    {
        var orphan = new XYNavigationDrawer(new XYNavigationState([new("a", "A", XyuiVectorIcon.Locate)]));
        orphan.Open(); Assert.False(orphan.IsOpen); Assert.Null(orphan.Host); Assert.Null(orphan.DrawerPopup.PlacementTarget);
    }
    [Fact] public void Bottom_navigation_primary_action_floating_hit_target_does_not_change_destination() => _fx.Run(() => {
        XyuiBatchTestHost.Prepare(); var state = new XYNavigationState([new("a", "地图", XyuiVectorIcon.Locate), new("b", "数据", XyuiVectorIcon.Code), new("c", "日志", XyuiVectorIcon.Section)], "a");
        var primary = new XYButton { Content = new XYIcon { Icon = XyuiVectorIcon.Add } }; var nav = new XYBottomNavigation(state, null, primary) { Width = 360 };
        var hit = 0; nav.PrimaryActionRequested += (_, _) => hit++;
        var host = new Canvas { Width = 500, Height = 300, Children = { nav } }; Canvas.SetLeft(nav, 50); Canvas.SetTop(nav, 100);
        var window = XyuiBatchTestHost.Show(host); Dispatcher.UIThread.RunJobs();
        var upperPt = primary.TranslatePoint(new Point(24, 6), window)!.Value; var navTop = nav.TranslatePoint(new Point(0, 0), window)!.Value.Y;
        Assert.True(upperPt.Y < navTop); var hitVisual = window.InputHitTest(upperPt); Assert.True(hitVisual is Visual visual && (ReferenceEquals(visual, primary) || visual.GetVisualAncestors().Contains(primary))); window.MouseMove(upperPt); window.MouseDown(upperPt, MouseButton.Left); window.MouseUp(upperPt, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.Equal(1, hit); Assert.Equal("a", nav.CurrentDestinationId); window.Close();
    });
    [Fact] public void BackForward_truncates_without_squishing_actions() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation { Width = 140 }; nav.Navigate("VERY_LONG_PATH_NAME_THAT_EXCEEDS_NORMAL_BOUNDS_AND_SHOULD_TRUNCATE"); var window = XyuiBatchTestHost.Show(nav); Dispatcher.UIThread.RunJobs(); Assert.Equal(28, nav.BackButton.Bounds.Width, 1); Assert.Equal(28, nav.ForwardButton.Bounds.Width, 1); Assert.False(nav.CanGoBack); Assert.False(nav.CanGoForward); window.Close(); });
    [Fact] public void Gallery_registers_all_final_components()
    { foreach (var id in new[] { "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24" }) Assert.NotNull(XYUI3GalleryCatalog.CreatePreview(id)); }
}
