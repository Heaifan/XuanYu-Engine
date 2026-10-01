namespace XuanYu.Render.Abstractions;

public static class TerrainChunkPartitioner
{
    public const int CellsPerChunk = 240;

    public static IReadOnlyList<TerrainChunkDescriptor> Partition(
        TerrainHeightfield field, string terrainId, int revision)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (string.IsNullOrWhiteSpace(terrainId))
            throw new ArgumentException("TerrainId 不能为空。", nameof(terrainId));
        var chunks = new List<TerrainChunkDescriptor>();
        var cellsX = field.Width - 1;
        var cellsY = field.Height - 1;
        for (var y = 0; y < cellsY; y += CellsPerChunk)
            for (var x = 0; x < cellsX; x += CellsPerChunk)
                chunks.Add(Create(field, terrainId, revision, x / CellsPerChunk, y / CellsPerChunk,
                    x, y, Math.Min(CellsPerChunk, cellsX - x),
                    Math.Min(CellsPerChunk, cellsY - y)));
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
        var size = field.CellSizeMeters;
        return new(id, revision, chunkX, chunkY, startX, startY, countX, countY,
            new(startX * size, startY * size, min, (startX + countX) * size,
                (startY + countY) * size, max), min, max);
    }
}
