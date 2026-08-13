using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RoadVertexSelectionD1Tests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-road-d1-{Guid.NewGuid():N}");

    [Fact]
    public async Task Road_selection_projects_all_dataset_vertices()
    {
        var (vm, road) = await CreateAsync();
        var body = Screen(vm, new(0, 0));

        Assert.True(vm.TryBeginMapGeometryPointer(body.X, body.Y, Viewport));
        Assert.Equal("已选择道路", vm.SelectedMapGeometryText);
        Assert.Equal(road.Points.Length, vm.MapGeometryPreview!.Value.Points.Length);
        Assert.Equal(-1, vm.SelectedMapGeometryVertexIndex);
    }

    [Fact]
    public async Task Clicking_road_vertices_reports_the_dataset_index()
    {
        var (vm, road) = await CreateAsync();
        for (var i = 0; i < road.Points.Length; i++)
        {
            var screen = Screen(vm, road.Points[i]);
            Assert.True(vm.TrySelectMapGeometryVertex(screen.X, screen.Y, Viewport));
            Assert.Equal(i, vm.SelectedMapGeometryVertexIndex);
        }
    }

    [Fact]
    public async Task Switching_road_replaces_selection_and_vertex_state()
    {
        var (vm, first) = await CreateAsync();
        var second = new MapRoad(MapRoadId.New(), first.LayerId, "道路 2", "generic",
            [new(-2, 1), new(0, 1), new(2, 1)]);
        Assert.True(vm.MapSession.CreateRoad(second).IsSuccess);
        var firstScreen = Screen(vm, first.Points[0]);
        var secondScreen = Screen(vm, second.Points[1]);

        Assert.True(vm.TrySelectMapGeometryVertex(firstScreen.X, firstScreen.Y, Viewport));
        Assert.True(vm.TrySelectMapGeometryVertex(secondScreen.X, secondScreen.Y, Viewport));
        Assert.Equal(second.RoadId.ToString(), vm.MapGeometryPreview!.Value.Selection.FeatureId);
        Assert.Equal(1, vm.SelectedMapGeometryVertexIndex);
    }

    [Fact]
    public async Task Switching_to_region_mode_clears_road_vertex_state()
    {
        var (vm, road) = await CreateAsync();
        var screen = Screen(vm, road.Points[0]);
        Assert.True(vm.TrySelectMapGeometryVertex(screen.X, screen.Y, Viewport));

        vm.SelectRegionAuthoringMode("区域面");

        Assert.Equal("未选择几何", vm.SelectedMapGeometryText);
        Assert.Equal(-1, vm.SelectedMapGeometryVertexIndex);
        Assert.Null(vm.MapGeometryPreview);
    }

    [Theory]
    [InlineData(false, false, true, false)]
    [InlineData(true, true, true, false)]
    [InlineData(true, false, false, false)]
    [InlineData(true, false, true, true)]
    public async Task Hidden_or_locked_road_is_not_editable(
        bool layerVisible, bool layerLocked, bool roadVisible, bool roadLocked)
    {
        var (vm, road) = await CreateAsync(layerVisible, layerLocked, roadVisible, roadLocked);
        var screen = Screen(vm, road.Points[0]);

        Assert.False(vm.TrySelectMapGeometryVertex(screen.X, screen.Y, Viewport));
        Assert.False(vm.TryBeginMapGeometryPointer(screen.X, screen.Y, Viewport));
        Assert.Equal("未选择几何", vm.SelectedMapGeometryText);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
