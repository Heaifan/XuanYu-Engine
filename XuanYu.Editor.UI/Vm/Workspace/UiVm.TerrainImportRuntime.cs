using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    async Task<bool> ImportTerrainSourcesCoreAsync(IReadOnlyList<string> paths)
    {
        if (IsTerrainImporting) return false;
        TerrainImportState = TerrainImportState.Importing; TerrainImportPercentage = 0;
        TerrainImportMessage = "准备导入"; RaiseImportBindings();
        _terrainImportCancellation = new CancellationTokenSource();
        var progress = new Progress<TerrainImportProgress>(ReportTerrainProgress);
        try
        {
            var tiles = await Task.Run(() => TerrainSourceImport.ReadMany(paths, progress,
                _terrainImportCancellation.Token), _terrainImportCancellation.Token);
            _terrainImportCancellation.Token.ThrowIfCancellationRequested();
            ActivateTerrainTiles(tiles);
            ReportTerrainProgress(new(TerrainImportStage.Activating, 1, 1, 100, "导入完成"));
            TerrainImportState = TerrainImportState.Completed; RaiseImportBindings();
            NotifySuccess("地形导入完成");
            return true;
        }
        catch (OperationCanceledException) { TerrainImportState = TerrainImportState.Cancelled; FailTerrainImport("已取消导入。", UiNotificationLevel.Warning); return false; }
        catch (TerrainSourceReadException error) { TerrainImportState = TerrainImportState.Error; FailTerrainImport(error.Message, UiNotificationLevel.Error); return false; }
        catch (Exception error) { TerrainImportState = TerrainImportState.Error; FailTerrainImport($"Runtime 激活失败：{error.Message}", UiNotificationLevel.Error); return false; }
        finally { _terrainImportCancellation?.Dispose(); _terrainImportCancellation = null; RaiseImportBindings(); }
    }

    void ActivateTerrainTiles(TerrainTileSet tiles)
    {
        _terrainTiles = tiles; _terrainTile = tiles.Tiles[0];
        if (tiles.Tiles.Count == 1) ActivateSingleTerrain(_terrainTile);
        else ActivateMultiTerrain(tiles);
        OnTerrainRuntimeEstablished(tiles);
        EnterTerrainContext();
    }

    void ActivateSingleTerrain(TerrainElevationTile tile)
    {
        TerrainWorld = TerrainWorld.FromElevationTile(tile);
        TerrainSource = new TerrainSourceData(tile.Raster, "EPSG:4326",
            new(tile.Bounds.West, tile.Bounds.South, tile.Bounds.East, tile.Bounds.North),
            tile.Resolution, new Dictionary<string, string> { ["format"] = "NASADEM_HGT" });
        TerrainStatus = "导入完成"; FooterMessage = $"导入完成：{TerrainTileId} · {TerrainResolutionText}";
        FooterState = "状态：就绪"; PublishSceneRenderSnapshot();
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

    void ReportTerrainProgress(TerrainImportProgress value)
    {
        if (!IsTerrainImporting) return;
        TerrainImportPercentage = Math.Max(TerrainImportPercentage, Math.Clamp(value.Percentage, 0, 100));
        TerrainImportMessage = value.Message; RaiseImportBindings();
    }

    void CancelTerrainImport() => _terrainImportCancellation?.Cancel();

    void RaiseImportBindings()
    {
        foreach (var name in new[] { nameof(TerrainImportState), nameof(IsTerrainImporting), nameof(IsTerrainImportComplete), nameof(TerrainImportPercentage), nameof(TerrainImportMessage), nameof(TerrainTileId), nameof(TerrainTileCount), nameof(TerrainSourceName), nameof(TerrainDatumText), nameof(TerrainResolutionText), nameof(TerrainBoundsText), nameof(TerrainProbeText) })
            OnPropertyChanged(name);
    }

    string Probe(TerrainTileSet tiles)
    {
        var latitude = (tiles.Bounds.South + tiles.Bounds.North) / 2;
        var longitude = (tiles.Bounds.West + tiles.Bounds.East) / 2;
        var sample = tiles.GetElevation(latitude, longitude);
        return $"Longitude {longitude:0.######}° · Latitude {latitude:0.######}° · " +
            $"Tiles {tiles.Tiles.Count} · Elevation {(sample.IsValid ? $"{sample.ElevationMeters:0.0} m" : "NoData")}";
    }

    static string FormatBounds(TerrainGeoBounds bounds) =>
        $"{bounds.South:0}°N–{bounds.North:0}°N {bounds.West:0}°E–{bounds.East:0}°E";
}
