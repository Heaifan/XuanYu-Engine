using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RegionDrawingF1RuntimeRedTests
{
    readonly UiHeadlessFixture _fixture;
    public RegionDrawingF1RuntimeRedTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Region_drawing_belongs_to_map_editor_workspace()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var found = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var right = new Right { DataContext = vm };
            host.Show(right, 420, 720);
            UiRuntimeTestHost.Descendants<TabItem>(right).Single(x => (string?)x.Header == "地图编辑器").IsSelected = true;
            right.UpdateLayout();
            var tool = UiRuntimeTestHost.Descendants<ToggleButton>(right)
                .Single(x => x.Classes.Contains("mapTool"));
            return tool.GetVisualAncestors().OfType<MapEditorPanel>().Any();
        });

        Assert.True(found);
    }

    [Fact]
    public void Region_drawing_selected_text_uses_dark_foreground()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var right = new Right { DataContext = vm };
            host.Show(right, 420, 720);
            UiRuntimeTestHost.Descendants<TabItem>(right).Single(x => (string?)x.Header == "地图编辑器").IsSelected = true;
            right.UpdateLayout();
            vm.SelectToolCommand.Execute("区域绘制");
            right.UpdateLayout();
            var toggle = UiRuntimeTestHost.Descendants<ToggleButton>(right)
                .Single(x => x.Classes.Contains("mapTool"));
            return (toggle.Foreground as SolidColorBrush)?.Color;
        });

        Assert.Equal(Color.Parse("#243744"), color);
    }

    [Fact]
    public void Real_region_tool_input_adds_first_draft_vertex()
    {
        var result = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.SelectToolCommand.Execute("区域绘制");
            var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
            var hit = Enumerable.Range(0, 17).SelectMany(ix => Enumerable.Range(0, 13)
                .Select(iy => (X: ix * 50.0, Y: iy * 50.0)))
                .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _));
            var handled = vm.RegionDrawingPointerPressed(hit.X, hit.Y, viewport);
            return (handled, vm.RenderProjection.Projection!.RegionModelResources.Count);
        });

        Assert.True(result.handled);
        Assert.True(result.Item2 > 0);
    }
}
