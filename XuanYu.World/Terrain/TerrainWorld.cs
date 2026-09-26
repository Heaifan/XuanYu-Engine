namespace XuanYu.World.Terrain;

public sealed class TerrainWorld
{
    public TerrainWorld(TerrainHeightLayer baseHeight, TerrainHeightLayer? editDelta = null)
    {
        BaseHeight = baseHeight ?? throw new ArgumentNullException(nameof(baseHeight));
        EditDelta = editDelta ?? TerrainHeightLayer.CreateEditDelta();
        if (!EditDelta.IsEditable)
            throw new ArgumentException("EditDelta 必须是可编辑层。", nameof(editDelta));
    }

    public TerrainHeightLayer BaseHeight { get; }
    public TerrainHeightLayer EditDelta { get; }

    public TerrainHeightSample GetFinalHeight(TerrainSampleCoordinate coordinate)
    {
        var baseSample = BaseHeight.Read(coordinate);
        if (baseSample.IsNoData) return TerrainHeightSample.NoData;
        return TerrainHeightSample.Valid(baseSample.Meters + EditDelta.Read(coordinate).Meters);
    }

    public bool TryGetFinalHeight(TerrainSampleCoordinate coordinate, out double meters)
    {
        var finalSample = GetFinalHeight(coordinate);
        if (finalSample.IsNoData)
        {
            meters = 0.0;
            return false;
        }

        meters = finalSample.Meters;
        return true;
    }
}
