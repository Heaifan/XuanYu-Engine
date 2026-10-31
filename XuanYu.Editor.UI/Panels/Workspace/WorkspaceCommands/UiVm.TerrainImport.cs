using System.Windows.Input;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    TerrainElevationTile? _terrainTile;
    TerrainTileSet? _terrainTiles;
    TerrainRenderResource[]? _terrainRenderResources;
    int _terrainRenderEditRevision;
    readonly TerrainImportCoordinator _terrainImportCoordinator = new();
    public TerrainImportState TerrainImportState => _terrainImportCoordinator.State;
    public bool IsTerrainImporting => TerrainImportState == TerrainImportState.Importing;
    public bool IsTerrainImportComplete => TerrainImportState == TerrainImportState.Completed;
    public double TerrainImportPercentage => _terrainImportCoordinator.Percentage;
    public string TerrainImportMessage => _terrainImportCoordinator.Message;
    public TerrainTileSet? TerrainTiles => _terrainTiles;
    public int TerrainTileCount => _terrainTiles?.Tiles.Count ?? 0;
    public string TerrainTileId => _terrainTiles is null ? "—" : _terrainTiles.Tiles.Count == 1
        ? _terrainTiles.Tiles[0].TileId : $"{_terrainTiles.Tiles.Count} Tiles";
    public string TerrainSourceName => _terrainTiles is null ? "—" : "NASADEM_HGT";
    public string TerrainDatumText => _terrainTiles is null ? "—" : "EGM96";
    public string TerrainResolutionText => _terrainTiles is null ? "—" : _terrainTiles.Tiles.Count == 1
        ? $"{_terrainTiles.Tiles[0].Width} × {_terrainTiles.Tiles[0].Height}" : $"{TerrainTileCount} Tiles";
    public string TerrainBoundsText => _terrainTiles is null ? "—" : FormatBounds(_terrainTiles.Bounds);
    public string TerrainProbeText => _terrainTiles is null ? "—" : Probe(_terrainTiles);
    public ICommand CancelTerrainImportCommand => new RelayCommand(_ => _terrainImportCoordinator.Cancel());

    public Task<bool> ImportTerrainSourceAsync(string path) => ImportTerrainSourcesAsync([path]);

    public Task<bool> ImportTerrainSourcesAsync(IReadOnlyList<string> paths) =>
        ImportTerrainSourcesCoreAsync(paths);

    void RaiseImportBindings()
    {
        foreach (var name in new[] { nameof(TerrainImportState), nameof(IsTerrainImporting),
            nameof(IsTerrainImportComplete), nameof(TerrainImportPercentage), nameof(TerrainImportMessage),
            nameof(TerrainTileId), nameof(TerrainTileCount), nameof(TerrainSourceName), nameof(TerrainDatumText),
            nameof(TerrainResolutionText), nameof(TerrainBoundsText), nameof(TerrainProbeText) })
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
