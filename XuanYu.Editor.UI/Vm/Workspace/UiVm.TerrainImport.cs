using System.Windows.Input;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    TerrainElevationTile? _terrainTile;
    ITerrainElevationQuery? _terrainElevationQuery;
    CancellationTokenSource? _terrainImportCancellation;
    public TerrainImportState TerrainImportState { get; private set; }
    public bool IsTerrainImporting => TerrainImportState == TerrainImportState.Importing;
    public bool IsTerrainImportComplete => TerrainImportState == TerrainImportState.Completed;
    public double TerrainImportPercentage { get; private set; }
    public string TerrainImportMessage { get; private set; } = "";
    public string TerrainTileId => _terrainTile?.TileId ?? "—";
    public string TerrainSourceName => _terrainTile is null ? "—" : "NASADEM_HGT";
    public string TerrainDatumText => _terrainTile is null ? "—" : "EGM96";
    public string TerrainResolutionText => _terrainTile is null ? "—" :
        $"{_terrainTile.Width} × {_terrainTile.Height}";
    public string TerrainBoundsText => _terrainTile is null ? "—" :
        $"{_terrainTile.Bounds.South:0}°N–{_terrainTile.Bounds.North:0}°N " +
        $"{_terrainTile.Bounds.West:0}°E–{_terrainTile.Bounds.East:0}°E";
    public string TerrainProbeText => _terrainTile is null ? "—" : Probe(_terrainTile);
    public ICommand CancelTerrainImportCommand => new RelayCommand(_ => CancelTerrainImport());

    public async Task<bool> ImportTerrainSourceAsync(string path)
    {
        if (IsTerrainImporting) return false;
        TerrainImportState = TerrainImportState.Importing; TerrainImportPercentage = 0;
        TerrainImportMessage = "准备导入"; RaiseImportBindings();
        _terrainImportCancellation = new CancellationTokenSource();
        var progress = new Progress<TerrainImportProgress>(ReportTerrainProgress);
        try
        {
            var tile = await Task.Run(() => ReadTile(path, progress), _terrainImportCancellation.Token);
            _terrainImportCancellation.Token.ThrowIfCancellationRequested();
            ReportTerrainProgress(new(TerrainImportStage.BuildingTerrain, 0, 0,
                TerrainImportPercentage, "正在构建地形"));
            _terrainTile = tile; _terrainElevationQuery = tile;
            TerrainWorld = TerrainWorld.FromElevationTile(tile);
            TerrainSource = new TerrainSourceData(tile.Raster, "EPSG:4326",
                new(tile.Bounds.West, tile.Bounds.South, tile.Bounds.East, tile.Bounds.North),
                tile.Resolution, new Dictionary<string, string> { ["format"] = "NASADEM_HGT" });
            TerrainStatus = "导入完成"; FooterMessage = $"导入完成：{TerrainTileId} · {TerrainResolutionText}"; FooterState = "状态：就绪";
            EnterTerrainContext();
            ReportTerrainProgress(new(TerrainImportStage.Activating, 1, 1, 100, "导入完成"));
            TerrainImportState = TerrainImportState.Completed; RaiseImportBindings();
            PublishSceneRenderSnapshot(); return true;
        }
        catch (OperationCanceledException) { TerrainImportState = TerrainImportState.Cancelled; FailTerrainImport("已取消导入。"); return false; }
        catch (TerrainSourceReadException error) { TerrainImportState = TerrainImportState.Error; FailTerrainImport(error.Message); return false; }
        catch (Exception error) { TerrainImportState = TerrainImportState.Error; FailTerrainImport($"Runtime 激活失败：{error.Message}"); return false; }
        finally { _terrainImportCancellation?.Dispose(); _terrainImportCancellation = null; RaiseImportBindings(); }
    }

    TerrainElevationTile ReadTile(string path, IProgress<TerrainImportProgress> progress)
    {
        if (!path.EndsWith(".hgt", StringComparison.OrdinalIgnoreCase))
            throw new TerrainSourceReadException("R1 仅支持 .hgt 地形源。");
        using var stream = File.OpenRead(path);
        return new HgtTerrainElevationTileReader().Read(stream, path, progress);
    }

    void ReportTerrainProgress(TerrainImportProgress value)
    {
        if (!IsTerrainImporting) return;
        TerrainImportPercentage = Math.Max(TerrainImportPercentage,
            Math.Clamp(value.Percentage, 0, 100));
        TerrainImportMessage = value.Message; RaiseImportBindings();
    }

    void CancelTerrainImport() => _terrainImportCancellation?.Cancel();

    void RaiseImportBindings()
    {
        foreach (var name in new[] { nameof(TerrainImportState), nameof(IsTerrainImporting), nameof(IsTerrainImportComplete), nameof(TerrainImportPercentage), nameof(TerrainImportMessage),
            nameof(TerrainTileId), nameof(TerrainSourceName), nameof(TerrainDatumText), nameof(TerrainResolutionText), nameof(TerrainBoundsText), nameof(TerrainProbeText) })
            OnPropertyChanged(name);
    }

    string Probe(TerrainElevationTile tile)
    {
        var latitude = (tile.Bounds.South + tile.Bounds.North) / 2;
        var longitude = (tile.Bounds.West + tile.Bounds.East) / 2;
        var sample = _terrainElevationQuery?.GetElevation(latitude, longitude) ?? TerrainElevationResult.Invalid;
        return $"Longitude {longitude:0.######}° · Latitude {latitude:0.######}° · " +
            $"Tile {tile.TileId} · Elevation {(sample.IsValid ? $"{sample.ElevationMeters:0.0} m" : "NoData")}";
    }
}
