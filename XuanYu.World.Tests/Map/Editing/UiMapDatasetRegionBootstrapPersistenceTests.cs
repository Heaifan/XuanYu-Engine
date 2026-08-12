using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionBootstrapPersistenceTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-bootstrap-save-{Guid.NewGuid():N}");
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public async Task Auto_created_dataset_and_four_point_region_survive_save_reload()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(await vm.BeginRegionDrawingAsync());
        var points = FindHits(vm, 4);
        Assert.Equal(4, points.Count);
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        Assert.True(await vm.SaveMapManifestAsync(path));

        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        Assert.Single(reopened.RegionDatasetItems);
        var region = Assert.Single(reopened.MapSession.CurrentMap.Regions);
        Assert.Equal(4, region.Vertices.Length);
    }

    static List<(double X, double Y)> FindHits(UiVm vm, int count)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var hits = new List<(double X, double Y)>();
        foreach (var point in new[] { (100.0, 100.0), (700.0, 100.0), (700.0, 500.0), (100.0, 500.0) })
        {
            var (x, y) = point;
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _)) hits.Add(point);
        }
        if (hits.Count == count) return hits;
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _) && !hits.Contains((x, y)))
                hits.Add((x, y));
        return hits.Take(count).ToList();
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch (IOException) { }
    }
}
