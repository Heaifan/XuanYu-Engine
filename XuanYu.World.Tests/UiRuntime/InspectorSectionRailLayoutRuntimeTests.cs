using Avalonia.Controls;
using Avalonia;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class InspectorSectionRailLayoutRuntimeTests
{
    readonly UiHeadlessFixture _fixture;

    public InspectorSectionRailLayoutRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData(300)]
    [InlineData(360)]
    [InlineData(480)]
    public void Entity_inspector_uses_single_focus_property_content_and_readonly_presenters(double width)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity();
            vm.SelectInspectorCategoryCommand.Execute("基础");
            var tabs = new EditorRightTabs { DataContext = vm };
            host.Show(tabs, width, 420); tabs.UpdateLayout();
            var panel = tabs.FindControl<InspectorPanel>("InspectorWorkspace")!;
            var rows = panel.FindControl<ItemsControl>("InspectorPropertyRows")!;
            var title = UiRuntimeTestHost.Descendants<XYHeading>(tabs).Single(x => x.Text == "立方体");
            var subtitle = UiRuntimeTestHost.Descendants<XYCaption>(tabs).Single(x => x.Text == "Cube · Entity");
            return (Rows: rows.ItemCount, Presenters: UiRuntimeTestHost.Descendants<InspectorReadOnlyValuePresenter>(panel).Count(),
                Selectable: UiRuntimeTestHost.Descendants<XYSelectableText>(panel).Count(),
                Navigation: UiRuntimeTestHost.Descendants<XYNavigationItem>(panel).Count(),
                Title: title.Text, Subtitle: subtitle.Text);
        });

        Assert.True(result.Rows > 0);
        Assert.True(result.Presenters > 0);
        Assert.Equal(0, result.Selectable);
        Assert.True(result.Navigation >= 3);
        Assert.Equal("立方体", result.Title); Assert.Equal("Cube · Entity", result.Subtitle);
    }

    [Fact]
    public void Entity_inspector_uses_compact_icon_rail_with_centered_icons()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity();
            var tabs = new EditorRightTabs { DataContext = vm }; host.Show(tabs, 360, 420); tabs.UpdateLayout();
            var panel = tabs.FindControl<InspectorPanel>("InspectorWorkspace")!;
            var rail = panel.FindControl<XYNavigationRail>("InspectorNavigationRail")!;
            Assert.Equal(40, rail.Bounds.Width, 1);
            Assert.All(rail.Items, item =>
            {
                Assert.Equal(36, item.Bounds.Width, 1); Assert.Equal(36, item.Bounds.Height, 1);
                var icon = item.GetVisualDescendants().OfType<XYIcon>().Single();
                var iconCenter = icon.TranslatePoint(new Point(icon.Bounds.Width / 2, icon.Bounds.Height / 2), item)!.Value;
                Assert.InRange(Math.Abs(iconCenter.X - item.Bounds.Size.Width / 2), 0, 1);
                Assert.InRange(Math.Abs(iconCenter.Y - item.Bounds.Size.Height / 2), 0, 1);
                Assert.Empty(item.GetVisualDescendants().OfType<XyuiActionEdge>());
                Assert.NotNull(ToolTip.GetTip(item));
            });
            Assert.Contains("最近", rail.Items.Select(item => ToolTip.GetTip(item)?.ToString()));
            Assert.DoesNotContain(rail.GetVisualDescendants().OfType<TextBlock>(), x => x.Classes.Contains("xyui-navigation-label"));
            Assert.Single(rail.Items, item => item.IsSelected);
        });
    }
}
