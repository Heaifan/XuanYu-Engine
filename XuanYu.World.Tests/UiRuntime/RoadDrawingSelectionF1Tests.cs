using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RoadDrawingSelectionF1Tests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-road-f1-{Guid.NewGuid():N}");

    [Fact]
    public async Task Finish_road_exits_drawing_and_selects_new_road()
    {
        var vm = await CompleteAsync();
        Assert.False(vm.IsRoadDrawingDraftActive);
        Assert.False(vm.IsRoadDrawingTool);
        Assert.Equal("选择", vm.ActiveTool);
        Assert.Equal("已选择道路", vm.SelectedMapGeometryText);
        Assert.Equal(2, vm.MapGeometryPreview!.Value.Points.Length);
        Assert.Equal(-1, vm.SelectedMapGeometryVertexIndex);
    }

    [Fact]
    public async Task Select_empty_ground_does_not_create_a_draft_or_road()
    {
        var vm = await CompleteAsync(); var count = vm.RoadContentCount;
        Assert.False(vm.TryBeginMapGeometryPointer(799, 599, Viewport));
        Assert.Equal(count, vm.RoadContentCount);
        Assert.False(vm.IsRoadDrawingDraftActive);
    }

    [Fact]
    public async Task Select_tool_picks_existing_road_line()
    {
        var vm = await CompleteAsync();
        var screen = Screen(vm, new(0, 0));
        Assert.True(vm.TryBeginMapGeometryPointer(screen.X, screen.Y, Viewport));
        Assert.Equal("已选择道路", vm.SelectedMapGeometryText);
    }

    [Fact]
    public async Task Consecutive_select_pointer_down_does_not_increase_road_count()
    {
        var vm = await CompleteAsync(); var count = vm.RoadContentCount;
        vm.TryBeginMapGeometryPointer(799, 599, Viewport);
        vm.TryBeginMapGeometryPointer(798, 598, Viewport);
        Assert.Equal(count, vm.RoadContentCount);
        Assert.False(vm.IsRoadDrawingDraftActive);
    }

    [Fact]
    public async Task Explicit_draw_command_is_required_for_second_road()
    {
        var vm = await CompleteAsync();
        Assert.True(await vm.BeginRoadDrawingAsync());
        AddNodes(vm, new(-2, -2), new(2, 2));
        Assert.True(vm.CompleteRoadDrawing());
        Assert.Equal(2, vm.RoadContentCount);
        Assert.False(vm.IsRoadDrawingTool);
    }

    [Fact]
    public async Task Direct_road_drawing_pointer_is_ignored_after_finish()
    {
        var vm = await CompleteAsync(); var count = vm.RoadContentCount;
        Assert.False(vm.RoadDrawingPointerPressed(400, 300, Viewport));
        Assert.Equal(count, vm.RoadContentCount);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
