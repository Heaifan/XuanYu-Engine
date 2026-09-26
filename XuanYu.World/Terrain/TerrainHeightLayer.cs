namespace XuanYu.World.Terrain;

public sealed class TerrainHeightLayer
{
    readonly Dictionary<TerrainSampleCoordinate, double> _values;
    readonly bool _editable;

    private TerrainHeightLayer(bool editable, IEnumerable<(TerrainSampleCoordinate Coordinate, double Meters)> values)
    {
        _editable = editable;
        _values = values.ToDictionary(item => item.Coordinate, item => item.Meters);
    }

    public static TerrainHeightLayer CreateBase(
        params (TerrainSampleCoordinate Coordinate, double Meters)[] values) =>
        new(false, values);

    public static TerrainHeightLayer CreateEditDelta() => new(true, []);

    public bool IsEditable => _editable;

    public TerrainHeightSample Read(TerrainSampleCoordinate coordinate)
    {
        return _values.TryGetValue(coordinate, out var meters)
            ? TerrainHeightSample.Valid(meters)
            : _editable
                ? TerrainHeightSample.Valid(0.0)
                : TerrainHeightSample.NoData;
    }

    public void Set(TerrainSampleCoordinate coordinate, double meters)
    {
        if (!_editable) throw new InvalidOperationException("BaseHeight 不可编辑。");
        _values[coordinate] = meters;
    }
}
