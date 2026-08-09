using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF1ResizeTests
{
    [Fact]
    public void R16_resize_keeps_region_drawing_input_alive()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.SelectToolCommand.Execute("区域绘制");
        var firstViewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var secondViewport = new ViewportState(0, 0, 1200, 800, 1200, 800, 1, 2);
        var first = FindHit(vm, firstViewport); var second = FindHit(vm, secondViewport);
        vm.RegionDrawingPointerPressed(first.X, first.Y, firstViewport);
        vm.RegionDrawingPointerPressed(second.X, second.Y, secondViewport);
        Assert.Equal(2, vm.RegionDrawingDraftVertexCount);
    }

    static (double X, double Y) FindHit(UiVm vm, ViewportState viewport)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _)) return (x, y);
        throw new InvalidOperationException("未找到测试用地面命中点。");
    }
}
