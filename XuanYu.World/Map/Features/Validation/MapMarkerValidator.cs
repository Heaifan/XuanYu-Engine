using System.Collections.Immutable;

namespace XuanYu.World.Map;

public static class MapMarkerValidator
{
    public static MapValidationResult Validate(ImmutableArray<MapMarker> markers,
        ImmutableArray<MapLayer> layers, MapSize size)
    {
        var ids = new HashSet<MapMarkerId>();
        foreach (var marker in markers)
        {
            if (!marker.MarkerId.IsValid || !ids.Add(marker.MarkerId))
                return MapValidationResult.Fail("DuplicateMarkerId", "Marker Feature ID 必须唯一且合法。");
            if (string.IsNullOrWhiteSpace(marker.DisplayName) || !Finite(marker.Position.X) || !Finite(marker.Position.Y))
                return MapValidationResult.Fail("InvalidMarker", "Marker 名称和坐标必须有效。");
            if (marker.Position.X < -size.Width / 2 || marker.Position.X > size.Width / 2 ||
                marker.Position.Y < -size.Depth / 2 || marker.Position.Y > size.Depth / 2)
                return MapValidationResult.Fail("MarkerOutOfBounds", "Marker 不得超出地图边界。");
            if (MapLayerRules.Find(layers, marker.LayerId) is not { Kind: MapLayerKind.Region })
                return MapValidationResult.Fail("InvalidMarkerLayer", "Marker 必须属于区域编辑图层。");
        }
        return MapValidationResult.Ok();
    }
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}
