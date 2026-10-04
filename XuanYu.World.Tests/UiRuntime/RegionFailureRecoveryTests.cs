using System.Reflection;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionFailureRecoveryTests
{
    [Fact]
    public async Task Terrain_region_blocked_round_trip_has_no_stale_state()
    {
        var (vm, path) = await CreateVm();
        var id = vm.DatasetSelectedId!;
        vm.EnterTerrainContext();
        vm.EnterRegionContext();
        await vm.ToggleDatasetLockAsync(id);

        Assert.False(await vm.BeginRegionDrawingAsync());
        Assert.True(vm.HasNotification);
        vm.EnterTerrainContext();
        vm.EnterRegionContext();
        await vm.ToggleDatasetLockAsync(id);

        Assert.True(await vm.BeginRegionDrawingAsync());
        Assert.True(vm.IsRegionDrawingTool);
        Assert.True(vm.CanOpenContextSelector == false);
        Assert.Equal(path, vm.CurrentMapManifestPath);
    }

    [Fact]
    public async Task Identity_failure_then_reload_valid_manifest_allows_region_tool()
    {
        var (vm, path) = await CreateVm();
        var owner = (MapManifestOwner)typeof(UiVm).GetField("_mapManifestOwner",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        owner.Modify(vm.CurrentMapManifest with { Id = MapId.New().Value });

        Assert.False(await vm.BeginRegionDrawingAsync());
        Assert.False(vm.IsRegionDrawingTool);
        Assert.True(await vm.OpenMapManifestAsync(path));
        Assert.True(await vm.BeginRegionDrawingAsync());
        Assert.True(vm.IsRegionDrawingTool);
    }

    static async Task<(UiVm Vm, string Path)> CreateVm()
    {
        var root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-recovery-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        return (vm, path);
    }
}
