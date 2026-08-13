using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class GenericMarkerSnapIntegrationTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-snap-{Guid.NewGuid():N}");

    [Fact]
    public async Task Marker_vertex_snaps_to_region_vertex()
    {
        var (vm, marker) = await CreateAsync();
        var regionPoint = new MapPoint(2, 2);
        var source = Screen(vm, marker.Position); var target = Screen(vm, regionPoint);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        Assert.Equal(regionPoint, vm.MapSession.CurrentMap.Markers[0].Position);
        Assert.Equal(marker.MarkerId, vm.MapSession.CurrentMap.Markers[0].MarkerId);
    }

    [Fact]
    public async Task Marker_vertex_snaps_to_other_marker_vertex()
    {
        var (vm, marker) = await CreateAsync();
        var targetMarker = new MapMarker(MapMarkerId.New(), marker.LayerId, "目标", new(4, 4));
        Assert.True(vm.MapSession.CreateMarker(targetMarker).IsSuccess);
        var source = Screen(vm, marker.Position); var target = Screen(vm, targetMarker.Position);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X + 3, target.Y + 2, Viewport));
        Assert.Equal(targetMarker.Position, vm.MapSession.CurrentMap.Markers[0].Position);
    }

    [Fact]
    public async Task Marker_vertex_snaps_to_road_segment()
    {
        var (vm, marker) = await CreateAsync();
        var target = Screen(vm, new MapPoint(5.05, 3)); var source = Screen(vm, marker.Position);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.InRange(Math.Abs(vm.MapSession.CurrentMap.Markers[0].Position.X - 5), 0, 0.001);
        Assert.InRange(vm.MapSession.CurrentMap.Markers[0].Position.Y, 2, 6);
    }

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }

    async Task<(UiVm Vm, MapMarker Marker)> CreateAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Marker; Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("地图标记"); vm.SelectToolCommand.Execute("选择");
        var layer = vm.MapSession.ActiveRegionLayerId;
        var marker = new MapMarker(MapMarkerId.New(), layer, "标记", new(0, 0));
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
        Assert.True(vm.MapSession.CreateRegion(new MapRegion(MapRegionId.New(), layer, "区域", MapRegionKind.Generic,
            [new(2, 2), new(3, 2), new(3, 3)])).IsSuccess);
        Assert.True(vm.MapSession.CreateRoad(new MapRoad(MapRoadId.New(), layer, "道路", "generic",
            [new(5, -2), new(5, 2), new(5, 6)])).IsSuccess);
        return (vm, marker);
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(MapCoordinateContract.MapToWorld(point,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        return (screen.X, screen.Y);
    }
}
