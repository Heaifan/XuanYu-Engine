using System.Reflection;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionPointerSafetyF2Tests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Crash_repro_01_empty_region_tool_move_is_no_op()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        var state = vm.MapSession.CurrentStateId; var sequence = vm.MapSession.ChangeSequence;

        var error = Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport));

        Assert.Null(error); Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Equal(state, vm.MapSession.CurrentStateId); Assert.Equal(sequence, vm.MapSession.ChangeSequence);
    }

    [Fact]
    public void Empty_region_draft_move_does_not_read_first_vertex()
    {
        var vm = RegionDrawingTestVm.Create(); vm.SelectToolCommand.Execute("区域绘制");
        var field = typeof(UiVm).GetField("_regionDrawing", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var drawing = (RegionDrawingState)field.GetValue(vm)!;
        drawing.Start(vm.MapSession.ActiveRegionLayerId, "测试", MapRegionKind.Generic);

        var error = Record.Exception(() => vm.RegionDrawingPointerMoved(400, 300, Viewport));

        Assert.Null(error); Assert.Equal(0, vm.RegionDrawingDraftVertexCount);
        Assert.Null(drawing.Cursor); Assert.False(drawing.IsCloseCandidate);
    }

    [Fact]
    public void First_anchor_allows_preview_without_adding_a_vertex()
    {
        var vm = RegionDrawingTestVm.Create(); vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var point = Enumerable.Range(0, 17).SelectMany(ix => Enumerable.Range(0, 13)
            .Select(iy => (X: ix * 50.0, Y: iy * 50.0)))
            .First(item => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, item.X, item.Y, out _));
        Assert.True(vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport));
        Assert.True(vm.RegionDrawingPointerMoved(point.X + 10, point.Y + 10, Viewport));
        Assert.Equal(1, vm.RegionDrawingDraftVertexCount);
    }

    [Fact]
    public void Existing_region_vertex_wins_over_region_preview_and_drag()
    {
        var vm = RegionDrawingTestVm.Create();
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var anchor = Enumerable.Range(0, 17).SelectMany(ix => Enumerable.Range(0, 13)
            .Select(iy => (X: ix * 50.0, Y: iy * 50.0)))
            .Select(point => (point, hit: MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection,
                point.X, point.Y, out var mapPoint) ? mapPoint : default))
            .First(item => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection,
                item.point.X, item.point.Y, out _)).hit;
        var region = new MapRegion(MapRegionId.New(), vm.MapSession.ActiveRegionLayerId, "区域",
            MapRegionKind.Generic, [anchor, new(anchor.X + 100, anchor.Y),
                new(anchor.X + 100, anchor.Y + 100), new(anchor.X, anchor.Y + 100)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectToolCommand.Execute("区域绘制");
        var screen = projection.ProjectWorldPoint(MapCoordinateContract.MapToWorld(
            anchor, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        Assert.True(vm.RegionDrawingPointerMoved(screen.X, screen.Y, Viewport));
        Assert.Equal("已选择区域", vm.SelectedMapGeometryText);
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(screen.X, screen.Y, Viewport));
        Assert.True(vm.IsMapGeometryDragActive);
        Assert.True(vm.PreviewMapGeometryPointer(screen.X + 20, screen.Y + 10, Viewport));
        Assert.True(vm.CancelMapGeometryPointer("测试取消"));
        Assert.False(vm.IsMapGeometryDragActive);
        Assert.Equal(region.Vertices, vm.MapSession.CurrentMap.Regions[0].Vertices);
    }

    [Fact]
    public void Cancel_then_move_and_mode_round_trip_are_safe()
    {
        var vm = RegionDrawingTestVm.Create(); vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(0, 0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        Assert.True(vm.RegionDrawingPointerPressed(screen.X, screen.Y, Viewport));
        Assert.True(vm.CancelRegionDrawingFromEscape());
        Assert.Null(Record.Exception(() => vm.RegionDrawingPointerMoved(screen.X, screen.Y, Viewport)));
        for (var i = 0; i < 10; i++)
        {
            vm.SelectRegionAuthoringMode(i % 2 == 0 ? "道路" : "区域面");
            Assert.Null(Record.Exception(() => vm.RegionDrawingPointerMoved(screen.X, screen.Y, Viewport)));
        }
        Assert.False(vm.IsRegionDrawingDraftActive);
    }
}
