namespace XuanYu.World.Terrain;

public sealed class TerrainWorld
{
    public TerrainWorld(TerrainHeightLayer baseHeight, TerrainHeightLayer? editDelta = null)
        : this(baseHeight, FallbackMetadata(), editDelta) { }

    public TerrainWorld(TerrainHeightLayer baseHeight, TerrainMetadata metadata,
        TerrainHeightLayer? editDelta, bool renderRowsSouthToNorth = false)
    {
        BaseHeight = baseHeight ?? throw new ArgumentNullException(nameof(baseHeight));
        EditDelta = editDelta ?? TerrainHeightLayer.CreateEditDelta();
        if (!EditDelta.IsEditable)
            throw new ArgumentException("EditDelta 必须是可编辑层。", nameof(editDelta));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        RenderRowsSouthToNorth = renderRowsSouthToNorth;
    }

    public TerrainHeightLayer BaseHeight { get; }
    public TerrainHeightLayer EditDelta { get; }
    public TerrainMetadata Metadata { get; }
    public bool RenderRowsSouthToNorth { get; }

    public static TerrainWorld FromSource(Source.TerrainSourceData source) =>
        TerrainWorldFactory.FromSource(source);

    public static TerrainWorld FromElevationTile(Source.TerrainElevationTile tile) =>
        TerrainWorldFactory.FromElevationTile(tile);

    public double QueryHeight(TerrainSampleCoordinate coordinate) =>
        GetFinalHeight(coordinate).Meters;

    static TerrainMetadata FallbackMetadata() =>
        new(1, 1, new(1, 1), 0, 0, null, 0, new(0, 0, 1, 1));

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
