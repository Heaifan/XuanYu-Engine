using System.Windows.Input;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    TerrainElevationTile? _terrainTile;
    TerrainTileSet? _terrainTiles;
    CancellationTokenSource? _terrainImportCancellation;
    public TerrainImportState TerrainImportState { get; private set; }
    public bool IsTerrainImporting => TerrainImportState == TerrainImportState.Importing;
    public bool IsTerrainImportComplete => TerrainImportState == TerrainImportState.Completed;
    public double TerrainImportPercentage { get; private set; }
    public string TerrainImportMessage { get; private set; } = "";
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
    public ICommand CancelTerrainImportCommand => new RelayCommand(_ => CancelTerrainImport());

    public Task<bool> ImportTerrainSourceAsync(string path) => ImportTerrainSourcesAsync([path]);

    public Task<bool> ImportTerrainSourcesAsync(IReadOnlyList<string> paths) =>
        ImportTerrainSourcesCoreAsync(paths);
}
