using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;
namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF1FullRuntimeTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void R01_off_does_not_create_draft()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false); var hit = FindHit(vm, Viewport);
        vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport);
        Assert.False(vm.IsRegionDrawingDraftActive);
    }

    [Fact]
    public void R02_first_ground_hit_creates_visible_draft_vertex()
    {
        var vm = CreateVm(); var hit = FindHit(vm, Viewport);
        vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport);
        Assert.Equal(1, vm.RegionDrawingDraftVertexCount);
        Assert.Contains(vm.RenderProjection.Projection!.VectorOverlayResources, x => x.Key.Value == "map-vector-overlay");
    }

    [Fact]
    public void R04_three_clicks_grow_draft_without_creating_region()
    {
        var vm = CreateVm(); var points = FindHits(vm, Viewport, 3);
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.Equal(3, vm.RegionDrawingDraftVertexCount);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
    }

    [Fact]
    public void R07_enter_after_three_vertices_creates_formal_region()
    {
        var vm = CreateVm();
        foreach (var point in FindHits(vm, Viewport, 3)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.Contains(vm.RenderProjection.Projection!.VectorOverlayResources, x => x.Key.Value == "map-vector-overlay");
    }

    [Fact]
    public void R08_enter_before_three_vertices_keeps_draft()
    {
        var vm = CreateVm(); var hit = FindHit(vm, Viewport);
        vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        Assert.True(vm.IsRegionDrawingDraftActive);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
    }

    [Fact]
    public void R09_escape_clears_draft_without_region()
    {
        var vm = CreateVm(); var hit = FindHit(vm, Viewport);
        vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport);
        Assert.True(vm.CancelRegionDrawingFromEscape());
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
    }

    [Fact]
    public void R12_pointer_preview_reaches_draft_snapshot()
    {
        var vm = CreateVm(); var points = FindHits(vm, Viewport, 2);
        vm.RegionDrawingPointerPressed(points[0].X, points[0].Y, Viewport);
        Assert.True(vm.RegionDrawingPointerMoved(points[1].X, points[1].Y, Viewport));
        Assert.Contains(vm.RenderProjection.Projection!.VectorOverlayResources, x => x.Key.Value == "map-vector-overlay");
    }

    [Fact]
    public void R15_dpi_175_uses_logical_pointer_coordinates()
    {
        var vm = CreateVm(); var logical = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1); var scaled = new ViewportState(0, 0, 800, 600, 1400, 1050, 1.75, 2);
        var hit = FindHit(vm, logical); vm.RegionDrawingPointerPressed(hit.X, hit.Y, scaled);
        Assert.Equal(1, vm.RegionDrawingDraftVertexCount);
    }

    static UiVm CreateVm()
    { var vm = RegionDrawingTestVm.Create(); vm.SelectToolCommand.Execute("区域绘制"); return vm; }

    static (double X, double Y) FindHit(UiVm vm, ViewportState viewport)
    { return FindHits(vm, viewport, 1)[0]; }

    static List<(double X, double Y)> FindHits(UiVm vm, ViewportState viewport, int count)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var hits = new List<(double, double)>();
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _) && !hits.Contains((x, y))) hits.Add((x, y));
        return hits.Take(count).ToList();
    }
}
