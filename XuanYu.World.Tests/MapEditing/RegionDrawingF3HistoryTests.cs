using XuanYu.Editor.UI;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionDrawingF3HistoryTests
{
    [Fact]
    public void Draft_undo_redo_and_new_vertex_clear_redo()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        var points = FindHits(vm, 4);
        foreach (var point in points.Take(3)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);

        Assert.True(vm.UndoRegionDrawingVertex());
        Assert.Equal(2, vm.RegionDrawingDraftVertexCount);
        Assert.True(vm.RedoRegionDrawingVertex());
        Assert.Equal(3, vm.RegionDrawingDraftVertexCount);
        Assert.True(vm.UndoRegionDrawingVertex());
        vm.RegionDrawingPointerPressed(points[3].X, points[3].Y, Viewport);
        Assert.False(vm.CanRedoRegionDrawingVertex);
    }

    [Fact]
    public void Shortcut_undo_prefers_active_draft_then_map_history()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        foreach (var point in FindHits(vm, 3)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        vm.TryUndoFromShortcut();
        Assert.Equal(2, vm.RegionDrawingDraftVertexCount);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);

        vm.CancelRegionDrawing();
        Assert.True(vm.MapSession.CreateRegion(new XuanYu.World.Map.MapRegion(
            XuanYu.World.Map.MapRegionId.New(), vm.MapSession.ActiveRegionLayerId,
            "测试区域", XuanYu.World.Map.MapRegionKind.Generic,
            [new(0, 0), new(100, 0), new(0, 100)])).IsSuccess);
        vm.TryUndoFromShortcut();
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
        vm.TryRedoFromShortcut();
        Assert.Single(vm.MapSession.CurrentMap.Regions);
    }

    static readonly XuanYu.Core.Space.ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    static List<(double X, double Y)> FindHits(UiVm vm, int count)
    {
        var projection = XuanYu.Core.Space.ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var hits = new List<(double X, double Y)>();
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (XuanYu.Editor.MapEditing.MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _))
                hits.Add((x, y));
        return hits.Take(count).ToList();
    }
}
