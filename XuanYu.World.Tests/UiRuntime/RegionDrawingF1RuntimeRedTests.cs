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
    public void Region_drawing_runtime_text_uses_dark_foreground_in_normal_and_selected_states()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var right = new Right { DataContext = vm };
            host.Show(right, 420, 720);
            UiRuntimeTestHost.Descendants<TabItem>(right).Single(x => (string?)x.Header == "地图编辑器").IsSelected = true;
            right.UpdateLayout();
            right.UpdateLayout();
            var toggle = UiRuntimeTestHost.Descendants<ToggleButton>(right)
                .Single(x => x.Classes.Contains("mapTool"));
            var normal = UiRuntimeTestHost.Descendants<TextBlock>(toggle)
                .Single(x => x.Classes.Contains("mapToolLabel"));
            var normalColor = (normal.Foreground as SolidColorBrush)?.Color;
            vm.SelectToolCommand.Execute("区域绘制");
            right.UpdateLayout();
            var selectedColor = (normal.Foreground as SolidColorBrush)?.Color;
            return (normalColor, selectedColor, toggle.IsChecked);
        });

        Assert.Equal(Color.Parse("#243744"), color.normalColor);
        Assert.Equal(Color.Parse("#243744"), color.selectedColor);
        Assert.True(color.IsChecked);
    }

    [Fact]
    public void Real_region_tool_input_creates_first_draft_vertex()
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
            return (handled, vm.RegionDrawingHitCount, vm.LastRegionDrawingHit, vm.RenderProjection.Projection!.RegionModelResources.Count);
        });

        Assert.True(result.handled);
        Assert.Equal(1, result.RegionDrawingHitCount);
        Assert.NotNull(result.LastRegionDrawingHit);
        Assert.True(result.Item4 > 0);
    }
}
