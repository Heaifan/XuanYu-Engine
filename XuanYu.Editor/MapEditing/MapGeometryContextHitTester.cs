using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static partial class MapGeometryContextHitTester
{
    const double VertexRadius = 10;
    const double EdgeRadius = 10;

    public static bool TryHit(MapDefinition map, ViewProjectionState projection,
        double x, double y, double height, out MapGeometryContextHit hit)
    {
        if (TryVertex(map, projection, x, y, height, out hit)) return true;
        if (TryEdge(map, projection, x, y, height, out hit)) return true;
        if (TryFace(map, projection, x, y, height, out hit)) return true;
        hit = MapGeometryContextHit.Empty;
        return false;
    }

    static bool TryVertex(MapDefinition map, ViewProjectionState p, double x, double y, double height,
        out MapGeometryContextHit hit)
    {
        var best = MapGeometryContextHit.Empty;
        foreach (var region in map.Regions)
            if (EditableRegion(map, region)) TryPoints(map, p, region.Vertices, new(MapGeometryFeatureKind.Region, region.RegionId.ToString()),
                MapGeometryContextKind.Vertex, x, y, height, ref best);
        foreach (var road in map.Roads.Where(road => MapGeometryHitTester.IsEditable(map, road)))
            TryPoints(map, p, road.Points, new(MapGeometryFeatureKind.Road, road.RoadId.ToString()),
                MapGeometryContextKind.Vertex, x, y, height, ref best);
        foreach (var marker in map.Markers.IsDefault ? [] : map.Markers)
            TryPoints(map, p, [marker.Position], new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()),
                MapGeometryContextKind.Marker, x, y, height, ref best);
        hit = best;
        return best.Kind != MapGeometryContextKind.Empty;
    }

    static bool EditableRegion(MapDefinition map, MapRegion region) => region.IsVisible && !region.IsLocked &&
        MapLayerRules.Find(map.Layers, region.LayerId) is { IsVisible: true, IsLocked: false };

    static void TryPoints(MapDefinition map, ViewProjectionState p, IReadOnlyList<MapPoint> points,
        MapGeometrySelection selection, MapGeometryContextKind kind, double x, double y, double height,
        ref MapGeometryContextHit best)
    {
        if (!MapGeometryHitTester.IsEditable(map, selection)) return;
        for (var i = 0; i < points.Count; i++)
        {
            if (!p.TryProjectWorldPoint(World(points[i], height), out var screen)) continue;
            var distance = Distance(screen.X, screen.Y, x, y);
            if (distance <= VertexRadius && distance < best.DistanceDip)
                best = new(kind, selection, i, -1, distance);
        }
    }

    static bool TryEdge(MapDefinition map, ViewProjectionState p, double x, double y, double height,
        out MapGeometryContextHit hit)
    {
        var best = MapGeometryContextHit.Empty;
        foreach (var road in map.Roads.Where(road => MapGeometryHitTester.IsEditable(map, road)))
            TrySegments(p, road.Points, new(MapGeometryFeatureKind.Road, road.RoadId.ToString()), false,
                x, y, height, ref best);
        foreach (var region in map.Regions)
            if (EditableRegion(map, region)) TrySegments(p, region.Vertices,
                new(MapGeometryFeatureKind.Region, region.RegionId.ToString()), true, x, y, height, ref best);
        hit = best;
        return best.Kind == MapGeometryContextKind.Edge;
    }

}
