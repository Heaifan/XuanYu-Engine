using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static partial class MapGeometryHitTester
{
    static bool TryMarker(MapDefinition map, MapMarker marker, ViewProjectionState p, double x, double y, double height, out MapGeometryHit hit)
    {
        hit = new(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()), -1, double.MaxValue);
        if (!IsEditableMarker(map, new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()))) return false;
        if (!p.TryProjectWorldPoint(new(marker.Position.X, marker.Position.Y, height), out var screen)) return false;
        var distance = Math.Sqrt(Math.Pow(screen.X - x, 2) + Math.Pow(screen.Y - y, 2));
        hit = hit with { DistanceDip = distance }; return distance <= 10.0;
    }

    static bool IsEditableMarker(MapDefinition map, MapGeometrySelection selection) =>
        map.Markers.FirstOrDefault(marker => marker.MarkerId.ToString() == selection.FeatureId) is { } marker &&
        marker.IsVisible && !marker.IsLocked && MapLayerRules.Find(map.Layers, marker.LayerId) is { IsVisible: true, IsLocked: false };
}
