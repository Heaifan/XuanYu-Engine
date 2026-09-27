namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainChunkBounds(
    double MinX, double MinY, double MinZ, double MaxX, double MaxY, double MaxZ);

public sealed record TerrainChunkDescriptor(
    string TerrainId,
    int Revision,
    int ChunkX,
    int ChunkY,
    int StartSampleX,
    int StartSampleY,
    int CellCountX,
    int CellCountY,
    TerrainChunkBounds WorldBounds,
    double MinElevation,
    double MaxElevation)
{
    public int SampleCountX => CellCountX + 1;
    public int SampleCountY => CellCountY + 1;

    public bool Matches(string terrainId, int revision) =>
        TerrainId == terrainId && Revision == revision;

    public bool HasValidSampleRange(TerrainHeightfield field) =>
        StartSampleX >= 0 && StartSampleY >= 0 &&
        CellCountX > 0 && CellCountY > 0 &&
        StartSampleX + CellCountX < field.Width &&
        StartSampleY + CellCountY < field.Height;
}
