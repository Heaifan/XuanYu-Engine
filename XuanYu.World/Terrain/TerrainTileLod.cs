namespace XuanYu.World.Terrain;

public readonly record struct TerrainTileLod
{
    public TerrainTileLod(TerrainSampleCoordinate origin, int sampleSize, int level)
    {
        if (sampleSize <= 0) throw new ArgumentOutOfRangeException(nameof(sampleSize));
        if (level < 0) throw new ArgumentOutOfRangeException(nameof(level));
        Origin = origin;
        SampleSize = sampleSize;
        Level = level;
    }

    public TerrainSampleCoordinate Origin { get; }
    public int SampleSize { get; }
    public int Level { get; }
}
