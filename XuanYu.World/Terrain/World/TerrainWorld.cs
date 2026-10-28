namespace XuanYu.World.Terrain;

public sealed class TerrainWorld
{
    public TerrainWorld(TerrainHeightLayer baseHeight, TerrainHeightLayer? editDelta = null)
        : this(baseHeight, FallbackMetadata(), editDelta) { }

    public TerrainWorld(TerrainHeightLayer baseHeight, bool renderRowsSouthToNorth)
        : this(baseHeight, FallbackMetadata(), null, renderRowsSouthToNorth) { }

    public TerrainWorld(TerrainHeightLayer baseHeight, TerrainMetadata metadata,
        TerrainHeightLayer? editDelta, bool renderRowsSouthToNorth = false)
    {
        BaseHeight = baseHeight ?? throw new ArgumentNullException(nameof(baseHeight));
        EditDelta = editDelta ?? TerrainHeightLayer.CreateEditDelta();
        if (!EditDelta.IsEditable)
            throw new ArgumentException("EditDelta 必须是可编辑层。", nameof(editDelta));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        Storage = new TerrainWorldStorage(BaseHeight, EditDelta);
        ElevationQuery = new TerrainElevationQuery(Storage);
        RasterOrientation = new TerrainRasterOrientation(renderRowsSouthToNorth);
    }

    public TerrainHeightLayer BaseHeight { get; }
    public TerrainHeightLayer EditDelta { get; }
    public TerrainMetadata Metadata { get; }
    public TerrainWorldStorage Storage { get; }
    public TerrainElevationQuery ElevationQuery { get; }
    public TerrainRevision TerrainRevision => TerrainRevision.From(Storage);
    public TerrainRasterOrientation RasterOrientation { get; }
    public bool RenderRowsSouthToNorth => RasterOrientation.RowsSouthToNorth;

    public static TerrainWorld FromSource(Source.TerrainSourceData source) =>
        TerrainWorldFactory.FromSource(source);

    public static TerrainWorld FromElevationTile(Source.TerrainElevationTile tile) =>
        TerrainWorldFactory.FromElevationTile(tile);

    public double QueryHeight(TerrainSampleCoordinate coordinate) =>
        GetFinalHeight(coordinate).Meters;

    public XuanYu.World.ElevationQueryResult QueryElevation(
        TerrainSampleCoordinate coordinate) => ElevationQuery.Query(coordinate);

    static TerrainMetadata FallbackMetadata() =>
        new(1, 1, new(1, 1), 0, 0, null, 0, new(0, 0, 1, 1));

    public TerrainHeightSample GetFinalHeight(TerrainSampleCoordinate coordinate)
        => Storage.Read(coordinate);

    public bool TryGetFinalHeight(TerrainSampleCoordinate coordinate, out double meters)
    {
        var result = QueryElevation(coordinate);
        if (!result.IsValid)
        {
            meters = 0.0;
            return false;
        }

        meters = result.ElevationMeters;
        return true;
    }
}
