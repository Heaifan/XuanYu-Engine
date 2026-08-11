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
    public void Region_drawing_is_not_rendered_as_a_map_editor_tool()
    {
        var map = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapPagePanel.axaml"));
        Assert.DoesNotContain("mapTool", map);
        Assert.DoesNotContain("区域绘制", map);
    }

    [Fact]
    public void Region_edit_has_no_drawing_tool_or_draft_surface()
    {
        var editor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));
        Assert.DoesNotContain("RegionDrawing", editor);
        Assert.DoesNotContain("Draft", editor);
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
            return (handled, vm.RegionDrawingHitCount, vm.LastRegionDrawingHit, vm.RenderProjection.Projection!.VectorOverlayResources.Count);
        });

        Assert.True(result.handled);
        Assert.Equal(1, result.RegionDrawingHitCount);
        Assert.NotNull(result.LastRegionDrawingHit);
        Assert.True(result.Item4 > 0);
    }
}
