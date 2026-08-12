using System.Collections.Immutable;

namespace XuanYu.World.Map;

public static class MapRoadValidator
{
    public const int MaxPoints = 1024;
    public static MapValidationResult Validate(ImmutableArray<MapRoad> roads, ImmutableArray<MapLayer> layers, MapSize size)
    {
        if (roads.IsDefault || roads.IsEmpty) return MapValidationResult.Ok();
        var layerIds = layers.IsDefault ? [] : layers.Select(item => item.LayerId).ToHashSet();
        var carriers = layers.IsDefault ? [] : layers.Where(item => item.Kind == MapLayerKind.Region).Select(item => item.LayerId).ToHashSet();
        var ids = new HashSet<MapRoadId>();
        foreach (var road in roads)
        {
            if (!road.RoadId.IsValid || !ids.Add(road.RoadId)) return Fail("InvalidRoadId", "道路 ID 非法或重复。");
            if (!layerIds.Contains(road.LayerId) || !carriers.Contains(road.LayerId)) return Fail("InvalidRoadLayer", "道路必须挂载到用户数据图层。");
            if (string.IsNullOrWhiteSpace(road.DisplayName) || string.IsNullOrWhiteSpace(road.Kind)) return Fail("InvalidRoadProperties", "道路名称和类型不能为空。");
            if (road.Points.IsDefault || road.Points.Length < 2) return Fail("TooFewRoadPoints", "道路至少需要两个节点。");
            if (road.Points.Length > MaxPoints) return Fail("TooManyRoadPoints", $"道路节点数超过上限 {MaxPoints}。");
            for (var i = 0; i < road.Points.Length; i++)
            {
                var point = road.Points[i];
                if (!Finite(point.X) || !Finite(point.Y)) return Fail("NonFiniteRoadPoint", "道路节点必须为有限数值。");
                if (!MapBounds.Contains(size, point.X, point.Y)) return Fail("RoadPointOutOfBounds", "道路节点超出地图边界。");
                if (i > 0 && point == road.Points[i - 1]) return Fail("AdjacentDuplicateRoadPoint", "道路相邻节点不得重复。");
            }
        }
        return MapValidationResult.Ok();
    }
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    static MapValidationResult Fail(string code, string message) => MapValidationResult.Fail(code, message, "ValidateRoads");
}
