using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    [Fact]
    public async Task Snapped_vertex_survives_save_and_reload_exactly()
    {
        var root = Path.Combine(Path.GetTempPath(), $"xuanyu-snap-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "map.json");
        try
        {
            var vm = await CreatePersistentVm(path);
            var target = AddInitialRegion(vm);
            AddSnappedRegion(vm, target);
            Assert.True(await vm.SaveMapManifestAsync(path), vm.FooterMessage);
            var reopened = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await reopened.OpenMapManifestAsync(path));
            Assert.Equal(2, reopened.MapSession.CurrentMap.Regions.Length);
            Assert.Contains(reopened.MapSession.CurrentMap.Regions[1].Vertices,
                point => point.X == target.X && point.Y == target.Y);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    static async Task<UiVm> CreatePersistentVm(string path)
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.ToggleEditorMode(); vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(await vm.BeginRegionDrawingAsync());
        return vm;
    }

    static XuanYu.World.Map.MapPoint AddInitialRegion(UiVm vm)
    {
        vm.SelectToolCommand.Execute("区域绘制");
        var points = new[] { FindHit(vm, 180, 180), FindHit(vm, 620, 180), FindHit(vm, 620, 420) };
        var target = Pick(vm, points[0]);
        foreach (var point in points) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
        return target;
    }

    static void AddSnappedRegion(UiVm vm, XuanYu.World.Map.MapPoint target)
    {
        vm.SelectToolCommand.Execute("区域绘制");
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(target.X, target.Y, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        vm.RegionDrawingPointerPressed(screen.X + 4, screen.Y, Viewport);
        foreach (var point in new[] { FindHit(vm, 620, 450), FindHit(vm, 500, 500) })
            vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CommitRegionDrawingFromEnter());
    }
}
