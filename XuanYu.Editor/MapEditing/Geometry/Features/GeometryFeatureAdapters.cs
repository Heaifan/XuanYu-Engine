using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class GeometryFeatureAdapters
{
    public static bool TryGet(MapDefinition map, GeometryFeatureKey key,
        out GeometryFeatureAdapter adapter)
    {
        if (key.FeatureKind == GeometryFeatureKind.Region &&
            map.Regions.FirstOrDefault(item => item.RegionId.ToString() == key.FeatureId) is { } region)
        {
            adapter = new(key, region.LayerId, GeometryKind.Polygon, RegionCapabilities(map, region), region.Vertices);
            return true;
        }
        if (key.FeatureKind == GeometryFeatureKind.Road &&
            map.Roads.FirstOrDefault(item => item.RoadId.ToString() == key.FeatureId) is { } road)
        {
            adapter = new(key, road.LayerId, GeometryKind.Polyline, RoadCapabilities(map, road), road.Points);
            return true;
        }
        if (key.FeatureKind == GeometryFeatureKind.Marker &&
            map.Markers.FirstOrDefault(item => item.MarkerId.ToString() == key.FeatureId) is { } marker)
        {
            adapter = new(key, marker.LayerId, GeometryKind.Point, MarkerCapabilities(map, marker), [marker.Position]);
            return true;
        }
        adapter = default;
        return false;
    }

    static GeometryCapabilities RegionCapabilities(MapDefinition map, MapRegion region) =>
        EditableLayer(map, region.LayerId)
            ? GeometryCapabilities.Selectable | GeometryCapabilities.VertexEditable |
              GeometryCapabilities.Snappable | GeometryCapabilities.SnapTarget : GeometryCapabilities.None;

    static GeometryCapabilities RoadCapabilities(MapDefinition map, MapRoad road) =>
        EditableLayer(map, road.LayerId)
            ? GeometryCapabilities.Selectable | GeometryCapabilities.VertexEditable |
              GeometryCapabilities.Snappable | GeometryCapabilities.SnapTarget : GeometryCapabilities.None;

    static GeometryCapabilities MarkerCapabilities(MapDefinition map, MapMarker marker) =>
        EditableLayer(map, marker.LayerId)
            ? GeometryCapabilities.Selectable | GeometryCapabilities.VertexEditable |
              GeometryCapabilities.Snappable | GeometryCapabilities.SnapTarget : GeometryCapabilities.None;

    static bool EditableLayer(MapDefinition map, MapLayerId layerId) =>
        MapLayerRules.Find(map.Layers, layerId) is { IsVisible: true, IsLocked: false };
}
