using XuanYu.Editor.UI;
using XuanYu.Editor.MapEditing;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetRegionRuntimeTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-ui-{Guid.NewGuid():N}");

    [Fact]
    public async Task Region_dataset_binds_drawing_save_reload_and_history_without_manifest_pollution()
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetSelectedId!;
        var layerId = XuanYu.Editor.MapDocument.MapDatasetLayerIdProjection.Project(id);
        Assert.Equal(layerId, vm.MapSession.ActiveRegionLayerId);

        var region = new MapRegion(MapRegionId.New(), layerId, "甲区", MapRegionKind.Generic,
            [new(10, 10), new(20, 10), new(10, 20)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        await vm.ToggleDatasetVisibilityAsync(id);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.False(vm.MapSession.CurrentMap.Layers.Single(item => item.LayerId == layerId).IsVisible);
        Assert.True(vm.MapSession.Undo().IsSuccess);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
        Assert.False(vm.MapSession.CurrentMap.Layers.Single(item => item.LayerId == layerId).IsVisible);
        Assert.True(vm.MapSession.Redo().IsSuccess);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.True(await vm.SaveMapManifestAsync(path));

        var reopened = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await reopened.OpenMapManifestAsync(path));
        var restored = Assert.Single(reopened.MapSession.CurrentMap.Regions);
        Assert.Equal(region.RegionId, restored.RegionId);
        Assert.Equal(layerId, restored.LayerId);
        Assert.Equal(region.Vertices.ToArray(), restored.Vertices.ToArray());
    }

    [Fact]
    public async Task Dataset_switch_and_lock_cancel_the_active_draft()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        var first = vm.DatasetSelectedId!;
        Assert.True(await vm.CreateDatasetAsync());
        var second = vm.DatasetSelectedId!;
        var hit = FindHit(vm);
        vm.SelectDataset(first); vm.SelectToolCommand.Execute("区域绘制");
        Assert.True(vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport));
        Assert.True(vm.IsRegionDrawingDraftActive);
        vm.SelectDataset(second);
        Assert.False(vm.IsRegionDrawingDraftActive);
        vm.SelectToolCommand.Execute("区域绘制");
        Assert.True(vm.RegionDrawingPointerPressed(hit.X, hit.Y, Viewport));
        await vm.ToggleDatasetLockAsync(second);
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.False(vm.IsRegionDrawingTool);
    }

    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    static (double X, double Y) FindHit(UiVm vm) => Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
        .Select(y => (X: x * 50.0, Y: y * 50.0))).First(point =>
            MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport), point.X, point.Y, out _));

    public void Dispose() { try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch (IOException) { } }
}
