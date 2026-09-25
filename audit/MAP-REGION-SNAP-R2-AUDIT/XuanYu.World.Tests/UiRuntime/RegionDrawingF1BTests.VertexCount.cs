namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingF1BTests
{
    [Fact]
    public void B08_repeated_snap_point_does_not_create_hidden_extra_vertex()
    {
        var vm = CreateVm(); var hit = FindHit(vm);
        vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport);
        vm.RegionDrawingPointerPressed(hit.ScreenX, hit.ScreenY, Viewport);
        Assert.Equal(1, vm.RegionDrawingDraftVertexCount);
    }
}
