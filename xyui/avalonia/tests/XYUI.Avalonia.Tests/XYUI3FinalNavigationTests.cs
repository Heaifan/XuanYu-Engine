using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3FinalNavigationTests
{
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
    [Fact] public void Drawer_shares_navigation_state_and_has_lifecycle_surface()
    {
        var state = new XYNavigationState([new("home", "首页", XyuiVectorIcon.Locate)]); var drawer = new XYNavigationDrawer(state); Assert.Same(state, drawer.NavigationState); drawer.Open(); Assert.True(drawer.IsOpen); drawer.Close(); Assert.False(drawer.IsOpen); Assert.NotNull(drawer.Backdrop);
    }
    [Fact] public void Gallery_registers_all_final_components()
    { foreach (var id in new[] { "XYUI-3-3.21", "XYUI-3-3.22", "XYUI-3-3.23", "XYUI-3-3.24" }) Assert.NotNull(XYUI3GalleryCatalog.CreatePreview(id)); }
}
