using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF2PolygonTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void Four_points_close_into_one_region()
    {
        var vm = RegionDrawingTestVm.Create();
        vm.SelectToolCommand.Execute("区域绘制");
        var points = FindQuadrilateralHits(vm);
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);

        Assert.Equal(4, vm.RegionDrawingDraftVertexCount);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        var region = Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.Equal(4, region.Vertices.Length);
    }

    static List<(double X, double Y)> FindQuadrilateralHits(UiVm vm)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var hits = new List<(double X, double Y)>();
        foreach (var point in new[] { (100.0, 100.0), (700.0, 100.0), (700.0, 500.0), (100.0, 500.0) })
        {
            var (x, y) = point;
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _)) hits.Add(point);
        }
        if (hits.Count == 4) return hits;
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _) && !hits.Contains((x, y)))
                hits.Add((x, y));
        return hits.Take(4).ToList();
    }
}
