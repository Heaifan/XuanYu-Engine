using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3NavigationRailWorkspaceStateTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3NavigationRailWorkspaceStateTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Workspace_focus_uses_canonical_focus_and_disabled_item_cannot_navigate()
        => _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var state = new XYNavigationState([
                new("project", "项目", XyuiVectorIcon.Browse),
                new("region", "区域", XyuiVectorIcon.Section, IsEnabled: false)], "project");
            var rail = new XYNavigationRail { NavigationState = state, LayoutVariant = XyuiNavigationLayoutVariant.Workspace, Width = 64 };
            var window = XyuiBatchTestHost.Show(rail);
            var project = rail.Items.Single(x => x.Id == "project");
            Assert.True(project.Focus());
            Assert.Equal(XyuiBatchTestHost.Token("XY.Border.Color.Focus"), XyuiBatchTestHost.ColorOf(project.BorderBrush));
            Assert.False(rail.Items.Single(x => x.Id == "region").IsEnabled);
            Assert.False(state.RequestNavigation("region"));
            Assert.Equal("project", state.SelectedId);
            window.Close();
        });
}
