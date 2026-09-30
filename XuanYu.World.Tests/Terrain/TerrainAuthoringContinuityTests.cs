using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Terrain;

public sealed partial class TerrainAuthoringContinuityTests
{
    [Fact]
    public void Terrain_hit_uses_terrain_extent_not_map_bounds()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var camera = new CameraState(new(6000, 0, 100), new(0, 0, -1),
            new(0, 1, 0), 45, 0.1, 1000, 1, ProjectionMode.Orthographic, 100);
        var projection = ViewProjectionState.Create(camera,
            new(0, 0, 100, 100, 100, 100, 1, 1));
        var surface = new FlatTerrainSurface();
        var ray = WorldRayFactory.FromViewportPoint(projection, 50, 50);
        var raw = GroundPickResolver.Resolve(ray, surface, 0);
        Assert.True(raw.IsValid, $"ray={ray}");

        Assert.True(MapSurfacePicker.TryPickGround(map, projection, 50, 50,
            surface, out var result));
        Assert.Equal(SurfaceBindingKind.Terrain, result.SurfaceBinding.Kind);
    }

    [Fact]
    public async Task Imported_terrain_survives_region_authoring_and_accepts_two_clicks()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.DatasetCreateType = MapDatasetTypes.Region;
        Assert.True(await vm.CreateDatasetAsync());
        vm.ToggleEditorMode();
        vm.SelectDataset(vm.DatasetSelectedId!);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        var directory = Path.Combine(Path.GetTempPath(), $"xye-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "n23e121.hgt");
        try
        {
            WriteHgt(path, 100, 200, 300, 400);
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            AssertTerrainAuthoringFrame(vm, new(0, 0, 800, 600, 800, 600, 1, 1), 0);
            vm.EnterRegionContext();
            var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            AssertTerrainAuthoringFrame(vm, viewport, 0);
            vm.SelectToolCommand.Execute("区域绘制");
            AssertTerrainAuthoringFrame(vm, viewport, 0);
            var points = FindTerrainClicks(vm, viewport, 2);
            Assert.Equal(2, points.Count);
            Assert.True(vm.RegionDrawingPointerPressed(points[0].X, points[0].Y, viewport));
            Assert.Equal(1, vm.RegionDrawingDraftVertexCount);
            AssertTerrainAuthoringFrame(vm, viewport, 1);
            Assert.True(vm.RegionDrawingPointerPressed(points[1].X, points[1].Y, viewport));
            Assert.Equal(2, vm.RegionDrawingDraftVertexCount);
            AssertTerrainAuthoringFrame(vm, viewport, 2);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

}
