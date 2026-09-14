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
    public void Entity_inspector_keeps_base_rows_header_and_inline_axes(double width)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity();
            vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Basic);
            vm.ToggleInspectorSectionCommand.Execute(InspectorSectionId.Position);
            var tabs = new EditorRightTabs { DataContext = vm };
            host.Show(tabs, width, 420); tabs.UpdateLayout();
            var panel = UiRuntimeTestHost.Descendants<EntityInspectorPanel>(tabs).Single();
            var grid = panel.FindControl<Grid>("BaseInfoGrid")!;
            var name = panel.FindControl<XYTextField>("NameBox")!;
            var type = UiRuntimeTestHost.Descendants<XYText>(panel).Single(x => x.Text == "Entity");
            var vectors = UiRuntimeTestHost.Descendants<XYVectorProperty>(panel).Select(vector =>
                (vector.Layout, Axes: UiRuntimeTestHost.Descendants<Border>(vector)
                    .Where(axis => axis.IsVisible && axis.Classes.Contains("xyui-vector-axis-host"))
                    .Select(axis => axis.Bounds).ToArray())).ToArray();
            var title = UiRuntimeTestHost.Descendants<XYHeading>(tabs)
                .Single(x => x.Name == "EntityHeaderTitle");
            var subtitle = UiRuntimeTestHost.Descendants<XYCaption>(tabs)
                .Single(x => x.Name == "EntityHeaderSubtitle");
            return (Grid: grid, NameBounds: name.Bounds, TypeBounds: type.Bounds,
                Vectors: vectors, Title: title.Text, Subtitle: subtitle.Text);
        });

        Assert.Equal(2, result.Grid.RowDefinitions.Count);
        Assert.True(result.NameBounds.Bottom <= result.TypeBounds.Top);
        Assert.Equal("立方体", result.Title); Assert.Equal("Cube · Entity", result.Subtitle);
        Assert.Equal(3, result.Vectors.Length);
        Assert.All(result.Vectors, vector => AssertInline(vector.Layout, vector.Axes));
    }

    static void AssertInline(XYVectorPropertyLayout layout, Rect[] axes)
    {
        Assert.Equal(XYVectorPropertyLayout.Inline, layout);
        Assert.Equal(3, axes.Length); Assert.All(axes, axis => Assert.Equal(axes[0].Y, axis.Y));
        Assert.True(axes.Zip(axes.Skip(1)).All(pair => pair.First.Right <= pair.Second.X));
    }
}
