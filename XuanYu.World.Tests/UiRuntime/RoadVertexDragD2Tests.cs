using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RoadVertexDragD2Tests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-road-d2-{Guid.NewGuid():N}");

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)]
    public async Task Any_open_polyline_vertex_can_be_dragged(int index)
    {
        var (vm, road) = await CreateAsync();
        var target = new MapPoint(road.Points[index].X + 0.5, road.Points[index].Y + 0.25);
        var before = vm.MapSession.CurrentMap.Roads[0].Points;
        Assert.True(vm.TryBeginMapGeometryVertexPointer(Screen(vm, road.Points[index]).X,
            Screen(vm, road.Points[index]).Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(Screen(vm, target).X, Screen(vm, target).Y, Viewport));
        Assert.Equal(before, vm.MapSession.CurrentMap.Roads[0].Points);
        Assert.True(vm.CommitMapGeometryPointer(Screen(vm, target).X, Screen(vm, target).Y, Viewport));
        var after = vm.MapSession.CurrentMap.Roads[0];
        Assert.InRange(Math.Abs(target.X - after.Points[index].X), 0, 0.001);
        Assert.InRange(Math.Abs(target.Y - after.Points[index].Y), 0, 0.001);
        Assert.Equal(before.Length, after.Points.Length);
        Assert.NotEqual(after.Points[0], after.Points[^1]);
        Assert.Equal(road.RoadId, after.RoadId);
        Assert.Equal(road.LayerId, after.LayerId);
    }

    [Fact]
    public async Task Preview_does_not_write_or_create_history_until_release()
    {
        var (vm, road) = await CreateAsync();
        var start = Screen(vm, road.Points[1]); var target = Screen(vm, new(1, 1));
        var state = vm.MapSession.CurrentStateId;
        Assert.True(vm.TryBeginMapGeometryVertexPointer(start.X, start.Y, Viewport));
        for (var i = 0; i < 5; i++) Assert.True(vm.PreviewMapGeometryPointer(target.X + i, target.Y, Viewport));
        Assert.Equal(state, vm.MapSession.CurrentStateId);
        Assert.Equal(road.Points, vm.MapSession.CurrentMap.Roads[0].Points);
        Assert.True(vm.CommitMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.Equal(state + 1, vm.MapSession.CurrentStateId);
    }

    [Fact]
    public async Task Escape_cancels_without_history_and_undo_redo_round_trip()
    {
        var (vm, road) = await CreateAsync();
        var start = Screen(vm, road.Points[1]); var target = Screen(vm, new(1, 1));
        Assert.True(vm.TryBeginMapGeometryVertexPointer(start.X, start.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.True(vm.CancelMapGeometryPointer("测试取消"));
        Assert.Equal(road.Points, vm.MapSession.CurrentMap.Roads[0].Points);
        Assert.Equal(1, vm.MapSession.CurrentStateId);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(start.X, start.Y, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X, target.Y, Viewport));
        vm.MapUndo(); Assert.Equal(road.Points, vm.MapSession.CurrentMap.Roads[0].Points);
        vm.MapRedo();
        Assert.InRange(Math.Abs(1 - vm.MapSession.CurrentMap.Roads[0].Points[1].X), 0, 0.001);
        Assert.InRange(Math.Abs(1 - vm.MapSession.CurrentMap.Roads[0].Points[1].Y), 0, 0.001);
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
        var road = new MapRoad(MapRoadId.New(), vm.MapSession.ActiveRegionLayerId, "道路", "generic",
            [new(-1, -1), new(0, 0), new(1, 1)]);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess); return (vm, road);
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(MapCoordinateContract.MapToWorld(point,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        return (screen.X, screen.Y);
    }
}
