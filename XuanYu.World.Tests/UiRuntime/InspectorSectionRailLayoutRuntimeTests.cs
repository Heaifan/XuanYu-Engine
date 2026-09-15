using Avalonia.Controls;
using Avalonia;
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
                Navigation: UiRuntimeTestHost.Descendants<XYToggleButton>(panel).Count(x => x.Classes.Contains("inspectorNavigationItem")),
                Title: title.Text, Subtitle: subtitle.Text);
        });

        Assert.True(result.Rows > 0);
        Assert.True(result.Presenters > 0);
        Assert.Equal(0, result.Selectable);
        Assert.True(result.Navigation >= 3);
        Assert.Equal("立方体", result.Title); Assert.Equal("Cube · Entity", result.Subtitle);
    }
}
