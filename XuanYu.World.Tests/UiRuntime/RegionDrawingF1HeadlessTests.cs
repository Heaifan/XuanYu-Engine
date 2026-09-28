using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed partial class RegionDrawingF1FullRuntimeTests
{
    readonly UiHeadlessFixture _fixture;

    public RegionDrawingF1FullRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void R07_enter_after_three_vertices_creates_formal_region()
    {
        var vm = CreateVm();
        foreach (var point in FindHits(vm, Viewport, 3)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        _fixture.Run(() => Assert.Contains(vm.RenderProjection.Projection!.VectorOverlayResources,
            x => x.Key.Value == "map-vector-overlay"));
    }
}
