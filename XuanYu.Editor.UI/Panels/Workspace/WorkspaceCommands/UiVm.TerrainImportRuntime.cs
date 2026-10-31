using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    async Task<bool> ImportTerrainSourcesCoreAsync(IReadOnlyList<string> paths)
    {
        if (IsTerrainImporting) return false;
        var succeeded = await _terrainImportCoordinator.RunAsync(paths, ActivateTerrainTiles);
        if (succeeded) NotifySuccess("地形导入完成");
        else FailTerrainImport(TerrainImportMessage, TerrainImportState == TerrainImportState.Cancelled
            ? UiNotificationLevel.Warning : UiNotificationLevel.Error);
        return succeeded;
    }

    void ActivateTerrainTiles(TerrainTileSet tiles)
    {
        var initialEstablishment = _terrainTiles is null;
        _terrainTiles = tiles; _terrainTile = tiles.Tiles[0];
        if (tiles.Tiles.Count == 1) ActivateSingleTerrain(_terrainTile);
        else ActivateMultiTerrain(tiles);
        RebuildTerrainRenderResources();
        if (initialEstablishment) OnTerrainRuntimeEstablished(tiles);
        EnterTerrainContext();
        PublishSceneRenderSnapshot();
    }

    void ActivateSingleTerrain(TerrainElevationTile tile)
    {
        TerrainWorld = TerrainWorld.FromElevationTile(tile);
        TerrainSource = new TerrainSourceData(tile.Raster, "EPSG:4326",
            new(tile.Bounds.West, tile.Bounds.South, tile.Bounds.East, tile.Bounds.North),
            tile.Resolution, new Dictionary<string, string> { ["format"] = "NASADEM_HGT" });
        TerrainStatus = "导入完成"; FooterMessage = $"导入完成：{TerrainTileId} · {TerrainResolutionText}";
        FooterState = "状态：就绪";
    }

    void ActivateMultiTerrain(TerrainTileSet tiles)
    {
        TerrainWorld = null; TerrainSource = null;
        TerrainStatus = "多 Tile 数据已建立，等待多地形渲染契约。";
        FooterMessage = $"已导入 {tiles.Tiles.Count} 个 Tile；等待多地形渲染契约";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(TerrainSource)); OnPropertyChanged(nameof(TerrainWorld));
        OnPropertyChanged(nameof(TerrainStatus));
    }
}
