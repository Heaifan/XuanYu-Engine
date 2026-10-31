using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3NavigationRailWorkspaceTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3NavigationRailWorkspaceTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Workspace_variant_keeps_labels_and_places_icons_above_them()
        => _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var state = State();
            var rail = new XYNavigationRail { NavigationState = state, LayoutVariant = XyuiNavigationLayoutVariant.Workspace, Width = 64 };
            var window = XyuiBatchTestHost.Show(rail);
            Assert.Equal(4, rail.Items.Count);
            Assert.All(rail.Items, item =>
            {
                var icon = item.GetVisualDescendants().OfType<XYIcon>().Single();
                var label = item.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-navigation-label"));
                Assert.True(label.IsVisible);
                Assert.True(icon.Bounds.Center.Y < label.Bounds.Center.Y);
                Assert.Equal(56, item.Bounds.Width, 1);
                Assert.Equal(58, item.Bounds.Height, 1);
            });
            window.Close();
        });

    [Fact]
    public void Workspace_selected_state_has_one_left_mark_and_no_tab_accent()
        => _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var rail = new XYNavigationRail { NavigationState = State(), LayoutVariant = XyuiNavigationLayoutVariant.Workspace, Width = 64 };
            var window = XyuiBatchTestHost.Show(rail);
            var selected = rail.Items.Single(x => x.IsSelected);
            var mark = selected.GetVisualDescendants().OfType<Border>().Single(x => x.Classes.Contains("xyui-navigation-accent"));
            Assert.True(mark.IsVisible);
            Assert.Equal(3, mark.Bounds.Width, 1);
            Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Selected"), XyuiBatchTestHost.ColorOf(selected.Background));
            Assert.DoesNotContain(selected.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-tab-accent"));
            window.Close();
        });

    [Fact]
    public void Workspace_hover_and_keyboard_preserve_single_navigation_state()
        => _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var state = State();
            var rail = new XYNavigationRail { NavigationState = state, LayoutVariant = XyuiNavigationLayoutVariant.Workspace, Width = 64 };
            var window = XyuiBatchTestHost.Show(rail);
            var target = rail.Items[1];
            XyuiBatchTestHost.Hover(window, target);
            Assert.Equal("project", state.SelectedId);
            Assert.Equal(XyuiBatchTestHost.Token("XY.State.Color.Hover"), XyuiBatchTestHost.ColorOf(target.Background));
            target.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
            Assert.Equal("hierarchy", state.SelectedId);
            Assert.Single(rail.Items, x => x.IsSelected);
            window.Close();
        });

    [Fact]
    public void Default_variant_keeps_icon_only_rail_behavior()
        => _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var rail = new XYNavigationRail { NavigationState = State(), Width = 64 };
            var window = XyuiBatchTestHost.Show(rail);
            Assert.All(rail.Items, item => Assert.DoesNotContain(item.GetVisualDescendants().OfType<TextBlock>(), x => x.Classes.Contains("xyui-navigation-label")));
            window.Close();
        });

    static XYNavigationState State() => new(
        [new("project", "项目", XyuiVectorIcon.Browse), new("hierarchy", "层级", XyuiVectorIcon.Locate), new("map", "地图", XyuiVectorIcon.Eye), new("region", "区域", XyuiVectorIcon.Section, IsEnabled: false)],
        "project");
}
