using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

internal sealed class GeometrySpatialIndex
{
    const double CellSize = 256;
    readonly Dictionary<(int X, int Y), HashSet<GeometryFeatureKey>> _cells = [];
    readonly Dictionary<GeometryFeatureKey, ImmutableArray<(int X, int Y)>> _memberships = [];

    public void Rebuild(MapDefinition map)
    {
        _cells.Clear(); _memberships.Clear();
        foreach (var region in map.Regions) Upsert(new(GeometryFeatureKind.Region, region.RegionId.ToString()), RegionSpatialBounds.From(region));
        foreach (var road in map.Roads.IsDefault ? [] : map.Roads)
            Upsert(new(GeometryFeatureKind.Road, road.RoadId.ToString()), Bounds(road.Points));
        foreach (var marker in map.Markers.IsDefault ? [] : map.Markers)
            Upsert(new(GeometryFeatureKind.Marker, marker.MarkerId.ToString()), new(marker.Position.X, marker.Position.Y, marker.Position.X, marker.Position.Y));
    }

    public void Upsert(GeometryFeatureKey key, RegionSpatialBounds bounds)
    {
        Remove(key);
        var cells = Cells(bounds).ToImmutableArray();
        _memberships[key] = cells;
        foreach (var cell in cells)
        {
            if (!_cells.TryGetValue(cell, out var keys)) _cells[cell] = keys = [];
            keys.Add(key);
        }
    }

    public void Remove(GeometryFeatureKey key)
    {
        if (!_memberships.Remove(key, out var cells)) return;
        foreach (var cell in cells)
            if (_cells.TryGetValue(cell, out var keys))
            {
                keys.Remove(key);
                if (keys.Count == 0) _cells.Remove(cell);
            }
    }

    public ImmutableArray<GeometryFeatureKey> Query(RegionSpatialBounds bounds)
    {
        var result = new HashSet<GeometryFeatureKey>();
        foreach (var cell in Cells(bounds))
            if (_cells.TryGetValue(cell, out var keys)) result.UnionWith(keys);
        return result.OrderBy(key => key.FeatureKind).ThenBy(key => key.FeatureId, StringComparer.Ordinal).ToImmutableArray();
    }

    static IEnumerable<(int X, int Y)> Cells(RegionSpatialBounds b)
    {
        var minX = (int)Math.Floor(b.MinX / CellSize); var maxX = (int)Math.Floor(b.MaxX / CellSize);
        var minY = (int)Math.Floor(b.MinY / CellSize); var maxY = (int)Math.Floor(b.MaxY / CellSize);
        for (var x = minX; x <= maxX; x++) for (var y = minY; y <= maxY; y++) yield return (x, y);
    }

    static RegionSpatialBounds Bounds(ImmutableArray<MapPoint> points)
    {
        var first = points[0]; var minX = first.X; var minY = first.Y; var maxX = first.X; var maxY = first.Y;
        foreach (var point in points) { minX = Math.Min(minX, point.X); minY = Math.Min(minY, point.Y); maxX = Math.Max(maxX, point.X); maxY = Math.Max(maxY, point.Y); }
        return new(minX, minY, maxX, maxY);
    }
}
