using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF1BTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void B01_region_tool_off_does_not_send_hit()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var hit = FindHit(vm);
        Assert.False(vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport));
        Assert.Equal(0, vm.RegionDrawingHitCount);
    }

    [Fact]
    public void B02_region_tool_on_reports_ground_hit()
    {
        var vm = CreateVm();
        var hit = FindHit(vm);
        Assert.True(vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport));
        Assert.Equal(1, vm.RegionDrawingHitCount);
        Assert.Equal(hit.MapPoint, vm.LastRegionDrawingHit);
    }

    [Fact]
    public void B03_different_screen_points_report_different_world_points()
    {
        var vm = CreateVm();
        var first = FindHit(vm);
        var second = FindHit(vm, first.ScreenX, first.ScreenY);
        vm.RegionDrawingPointerPressed(first.ScreenX, first.ScreenY, Viewport);
        vm.RegionDrawingPointerPressed(second.ScreenX, second.ScreenY, Viewport);
        Assert.Equal(2, vm.RegionDrawingHitCount);
        Assert.NotEqual(first.MapPoint, vm.LastRegionDrawingHit);
    }

    [Fact]
    public void B04_picking_miss_does_not_create_hit() {
        var vm = CreateVm();
        Assert.True(vm.RegionDrawingPointerPressed(-1, 20, Viewport));
        Assert.Equal(0, vm.RegionDrawingHitCount);
    }

    [Fact]
    public void B05_switching_to_selection_stops_region_input()
    {
        var vm = CreateVm(); var hit = FindHit(vm);
        vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport);
        vm.SelectToolCommand.Execute("选择");
        vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport);
        Assert.Equal(1, vm.RegionDrawingHitCount);
    }

    [Fact]
    public void B06_tool_round_trip_does_not_duplicate_input()
    {
        var vm = CreateVm(); var hit = FindHit(vm);
        vm.SelectToolCommand.Execute("选择");
        vm.SelectToolCommand.Execute("区域绘制");
        vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport);
        Assert.Equal(1, vm.RegionDrawingHitCount);
    }

    [Fact]
    public void B07_one_pointer_press_produces_at_most_one_hit()
    {
        var vm = CreateVm(); var hit = FindHit(vm);
        Assert.True(vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport));
        Assert.Equal(1, vm.RegionDrawingHitCount);
    }

    static UiVm CreateVm()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        return vm;
    }

    static (double ScreenX, double ScreenY, MapPoint MapPoint) FindHit(
        UiVm vm, double excludeX = double.NaN, double excludeY = double.NaN)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if ((x != excludeX || y != excludeY) && MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out var point))
                return (x, y, point);
        throw new InvalidOperationException("未找到测试用地面命中点。");
    }
}
