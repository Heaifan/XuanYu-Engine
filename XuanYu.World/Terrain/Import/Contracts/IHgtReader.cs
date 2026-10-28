using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain.Import;

public interface IHgtReader
{
    TerrainElevationTile Read(Stream stream, string tileId, IProgress<TerrainImportProgress>? progress = null);
}

public enum TerrainImportStage { Preparing, ReadingArchive, ReadingElevation, Decoding, BuildingTerrain, Activating }

public readonly record struct TerrainImportProgress(
    TerrainImportStage Stage, long Current, long Total, double Percentage, string Message);
