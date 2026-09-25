using XuanYu.Core.Space;
using XuanYu.World.Map;
namespace XuanYu.Editor.MapEditing;

public static partial class MapGeometryHitTester
{
    public static bool TryHitFeature(MapDefinition map, ViewProjectionState projection,
        double x, double y, double height, out MapGeometryHit hit)
    {
        foreach (var marker in map.Markers.IsDefault ? [] : map.Markers)
            if (TryMarker(map, marker, projection, x, y, height, out hit)) return true;
        foreach (var road in map.Roads.IsDefault ? [] : map.Roads)
            if (IsEditable(map, road) && TryRoad(road, projection, x, y, height, out hit)) return true;
        foreach (var region in map.Regions)
            if (TryRegion(region, projection, x, y, height, out hit)) return true;
        hit = default;
        return false;
    }

    public static bool TryHitVertex(MapDefinition map, MapGeometrySelection selection,
        ViewProjectionState projection, double x, double y, double radius, double height, out int index)
    {
        if (selection.Kind == MapGeometryFeatureKind.Road &&
            !map.Roads.Any(road => road.RoadId.ToString() == selection.FeatureId && IsEditable(map, road)))
        { index = -1; return false; }
        if (selection.Kind == MapGeometryFeatureKind.Marker && !IsEditable(map, selection))
        { index = -1; return false; }
        var points = selection.Kind == MapGeometryFeatureKind.Region
            ? map.Regions.FirstOrDefault(r => r.RegionId.ToString() == selection.FeatureId)?.Vertices ?? []
            : selection.Kind == MapGeometryFeatureKind.Road
            ? map.Roads.FirstOrDefault(r => r.RoadId.ToString() == selection.FeatureId)?.Points ?? []
            : map.Markers.FirstOrDefault(r => r.MarkerId.ToString() == selection.FeatureId) is { } marker ? [marker.Position] : [];
        var nearest = -1; var distance = radius;
        for (var i = 0; i < points.Length; i++)
        {
            if (!projection.TryProjectWorldPoint(World(points[i], height), out var screen)) continue;
            var current = Distance(screen.X, screen.Y, x, y);
            if (current <= distance) { nearest = i; distance = current; }
        }
        index = nearest;
        return nearest >= 0;
    }

    static bool TryRoad(MapRoad road, ViewProjectionState p, double x, double y, double height, out MapGeometryHit hit)
    {
        var distance = double.MaxValue;
        for (var i = 0; i + 1 < road.Points.Length; i++)
        {
            if (!p.TryProjectWorldPoint(World(road.Points[i], height), out var a) ||
                !p.TryProjectWorldPoint(World(road.Points[i + 1], height), out var b)) continue;
            distance = Math.Min(distance, SegmentDistance(a.X, a.Y, b.X, b.Y, x, y));
        }
        hit = new(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()), -1, distance);
        return distance <= 10.0;
    }

    static bool TryRegion(MapRegion region, ViewProjectionState p, double x, double y, double height, out MapGeometryHit hit)
    {
        var points = region.Vertices.Select(point => p.TryProjectWorldPoint(World(point, height), out var screen)
            ? (true, screen.X, screen.Y) : (false, 0d, 0d)).ToArray();
        var inside = points.Length >= 3 && points.All(point => point.Item1) &&
            Inside(points.Select(point => (point.Item2, point.Item3)), x, y);
        hit = new(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()), -1, inside ? 0 : double.MaxValue);
        return inside;
    }

    public static bool IsEditable(MapDefinition map, MapRoad road) => road.IsVisible && !road.IsLocked &&
        MapLayerRules.Find(map.Layers, road.LayerId) is { IsVisible: true, IsLocked: false };

    public static bool IsEditable(MapDefinition map, MapGeometrySelection selection) => selection.Kind switch
    {
        MapGeometryFeatureKind.Road => map.Roads.FirstOrDefault(road => road.RoadId.ToString() == selection.FeatureId) is { } road && IsEditable(map, road),
        MapGeometryFeatureKind.Marker => IsEditableMarker(map, selection),
        _ => true
    };

    static bool Inside(IEnumerable<(double X, double Y)> source, double x, double y)
    {
        var points = source.ToArray(); var inside = false;
        for (var i = 0; i < points.Length; i++)
        {
            var a = points[i]; var b = points[(i + points.Length - 1) % points.Length];
            if ((a.Y > y) != (b.Y > y) && x < (b.X - a.X) * (y - a.Y) / (b.Y - a.Y) + a.X)
                inside = !inside;
        }
        return inside;
    }

    static double SegmentDistance(double ax, double ay, double bx, double by, double x, double y)
    {
        var dx = bx - ax; var dy = by - ay;
        if (dx == 0 && dy == 0) return Distance(ax, ay, x, y);
        var t = Math.Clamp(((x - ax) * dx + (y - ay) * dy) / (dx * dx + dy * dy), 0, 1);
        return Distance(ax + t * dx, ay + t * dy, x, y);
    }

    static double Distance(double ax, double ay, double bx, double by) =>
        Math.Sqrt(Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2));
    static XuanYu.Core.Math.Vector3d World(MapPoint point, double height) => new(point.X, point.Y, height);
}
