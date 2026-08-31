using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3CompactInteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3CompactInteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Shared_navigation_state_has_one_selection() => _fx.Run(() =>
    {
        var state = new XYNavigationState([new("map", "地图", XyuiVectorIcon.Locate), new("data", "数据", XyuiVectorIcon.Code)], "map");
        state.Select("data"); Assert.Equal("data", state.SelectedId); Assert.Equal("data", state.Selected!.Id);
    });

    [Fact] public void Sidebar_collapsed_rail_keeps_shared_state_and_expand_path() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var sidebar = new XYSidebar { PrimaryItems = [new XYNavigationItem { Id = "map", Label = "地图", Icon = XyuiVectorIcon.Locate }] };
        sidebar.IsCollapsed = true; var rail = Assert.IsType<XYNavigationRail>(sidebar.Child!.GetVisualDescendants().OfType<XYNavigationRail>().FirstOrDefault() ?? sidebar.Child);
        Assert.Contains(rail.Items, x => x.Id == "map"); Assert.Contains(rail.Items, x => x.Id == "settings"); Assert.Same(sidebar.NavigationState, rail.NavigationState);
    });

    [Fact] public void Tabs_close_selected_selects_neighbor_and_close_all_is_empty() => _fx.Run(() =>
    {
        var first = new XYTab { Label = "一" }; var second = new XYTab { Label = "二", IsSelected = true }; var tabs = new XYTabs(first, second);
        tabs.Close(second); Assert.Single(tabs.Items); Assert.True(first.IsSelected); tabs.CloseAll(); Assert.Empty(tabs.Items);
    });
}
