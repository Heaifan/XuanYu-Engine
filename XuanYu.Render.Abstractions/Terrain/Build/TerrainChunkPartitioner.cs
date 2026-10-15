namespace XuanYu.Render.Abstractions;

public static class TerrainChunkPartitioner
{
    public const int CellsPerChunk = 240;

    public static IReadOnlyList<TerrainChunkDescriptor> Partition(
        TerrainHeightfield field, string terrainId, int revision)
        => Partition(field, terrainId, revision, CellsPerChunk);

    internal static IReadOnlyList<TerrainChunkDescriptor> Partition(
        TerrainHeightfield field, string terrainId, int revision, int cellsPerChunk)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (cellsPerChunk <= 0) throw new ArgumentOutOfRangeException(nameof(cellsPerChunk));
        if (string.IsNullOrWhiteSpace(terrainId))
            throw new ArgumentException("TerrainId 不能为空。", nameof(terrainId));
        var chunks = new List<TerrainChunkDescriptor>();
        var cellsX = field.Width - 1;
        var cellsY = field.Height - 1;
        for (var y = 0; y < cellsY; y += cellsPerChunk)
            for (var x = 0; x < cellsX; x += cellsPerChunk)
                chunks.Add(Create(field, terrainId, revision, x / cellsPerChunk, y / cellsPerChunk,
                    x, y, Math.Min(cellsPerChunk, cellsX - x), Math.Min(cellsPerChunk, cellsY - y)));
        return chunks;
    }

    static TerrainChunkDescriptor Create(TerrainHeightfield field, string id, int revision, int chunkX,
        int chunkY, int startX, int startY, int countX, int countY)
    {
        var min = double.PositiveInfinity;
        var max = double.NegativeInfinity;
        for (var y = startY; y <= startY + countY; y++)
            for (var x = startX; x <= startX + countX; x++)
            {
                var value = field.ElevationAt(y, x);
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        var sizeX = field.CellSizeMeters;
        var sizeY = field.CellSizeYMeters;
        return new(id, revision, chunkX, chunkY, startX, startY, countX, countY,
            new(startX * sizeX, startY * sizeY, min, (startX + countX) * sizeX,
                (startY + countY) * sizeY, max), min, max);
    }
}
