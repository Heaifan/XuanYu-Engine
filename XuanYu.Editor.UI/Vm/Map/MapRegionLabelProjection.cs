using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

static class MapRegionLabelProjection
{
    public static bool TryCreate(MapRegion region, IReadOnlyList<MapPoint> points,
        double height, double dpiScale, out RenderVectorOverlayLabel label)
    {
        label = default;
        if (!PolygonVisualCenter.TryFind(points, out var anchor)) return false;
        var world = MapCoordinateContract.MapToWorld(anchor, height);
        label = new(region.DisplayName, MapLabelCacheKey.Region(region.DisplayName, dpiScale),
            world, 13, dpiScale, new(.88, .93, .98, .96));
        return true;
    }
}
