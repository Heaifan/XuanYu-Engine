namespace XuanYu.World.Terrain;

public sealed class TerrainWorldStorage
{
    public TerrainWorldStorage(TerrainHeightLayer baseHeight,
        TerrainHeightLayer editDelta)
    {
        BaseHeight = baseHeight ?? throw new ArgumentNullException(nameof(baseHeight));
        EditDelta = editDelta ?? throw new ArgumentNullException(nameof(editDelta));
        if (!editDelta.IsEditable)
            throw new ArgumentException("EditDelta 必须是可编辑层。", nameof(editDelta));
    }

    public TerrainHeightLayer BaseHeight { get; }
    public TerrainHeightLayer EditDelta { get; }

    public TerrainHeightSample Read(TerrainSampleCoordinate coordinate)
    {
        var baseSample = BaseHeight.Read(coordinate);
        return baseSample.IsNoData
            ? TerrainHeightSample.NoData
            : TerrainHeightSample.Valid(baseSample.Meters + EditDelta.Read(coordinate).Meters);
    }
}
