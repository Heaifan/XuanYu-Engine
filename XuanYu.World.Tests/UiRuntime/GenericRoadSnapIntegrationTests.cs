using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class GenericRoadSnapIntegrationTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-road-snap-{Guid.NewGuid():N}");

    [Fact]
    public async Task Road_vertex_snaps_to_region_vertex()
    {
        var (vm, road) = await CreateAsync();
        var source = Screen(vm, road.Points[0]); var target = Screen(vm, new(2, 2));
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        var point = vm.MapSession.CurrentMap.Roads[0].Points[0];
        Assert.InRange(Math.Abs(point.X - 2), 0, 0.001);
        Assert.InRange(Math.Abs(point.Y - 2), 0, 0.001);
    }

    [Fact]
    public async Task Road_vertex_snaps_to_other_road_segment()
    {
        var (vm, road) = await CreateAsync();
        var other = new MapRoad(MapRoadId.New(), road.LayerId, "道路 2", "generic",
            [new(5, -2), new(5, 2), new(5, 6)]);
        Assert.True(vm.MapSession.CreateRoad(other).IsSuccess);
        var source = Screen(vm, road.Points[1]); var target = Screen(vm, new(5.05, 3));
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X, target.Y, Viewport));
        var point = vm.MapSession.CurrentMap.Roads[0].Points[1];
        Assert.InRange(Math.Abs(point.X - 5), 0, 0.001);
        Assert.InRange(point.Y, 2, 6);
    }

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }

    async Task<(UiVm Vm, MapRoad Road)> CreateAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Road; Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("道路"); vm.SelectToolCommand.Execute("选择");
        var region = new MapRegion(MapRegionId.New(), vm.MapSession.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [new(2, 2), new(3, 2), new(3, 3)]);
        var road = new MapRoad(MapRoadId.New(), vm.MapSession.ActiveRegionLayerId, "道路", "generic",
            [new(0, 0), new(1, 0), new(2, 0)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        return (vm, road);
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(MapCoordinateContract.MapToWorld(point,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        return (screen.X, screen.Y);
    }
}
