using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1：区域集合严格校验（领域权威层）。
// 检查：ID 合法且唯一、引用图层存在且可承载区域（非 Base）、名称非空、
// 至少三个不同顶点、无相邻重复点（含首尾）、顶点数上限、有限数值、边界内、非零面积。
// 自交检测明确归 D5（绘制轮），不在 F1 范围。
public static class MapRegionValidator
{
    public const int MaxVerticesPerRegion = 1024;
    public const double MinAreaSquareMeters = 1e-9;

    public static MapValidationResult Validate(
        ImmutableArray<MapRegion> regions,
        ImmutableArray<MapLayer> layers,
        MapSize size)
    {
        if (regions.IsDefault)
            return MapValidationResult.Fail("InvalidRegionList", "区域集合缺失。");

        var layerIds = layers.IsDefault
            ? new HashSet<MapLayerId>()
            : layers.Select(l => l.LayerId).ToHashSet();
        var carrierIds = layers.IsDefault
            ? new HashSet<MapLayerId>()
            : layers.Where(l => l.Kind != MapLayerKind.Base)
                .Select(l => l.LayerId).ToHashSet();
        var ids = new HashSet<MapRegionId>();
        foreach (var region in regions)
        {
            if (!region.RegionId.IsValid)
                return MapValidationResult.Fail("InvalidRegionId", $"区域ID非法：{region.RegionId}。");
            if (!ids.Add(region.RegionId))
                return MapValidationResult.Fail("DuplicateRegionId", $"区域ID重复：{region.RegionId}。");
            if (!layerIds.Contains(region.LayerId))
                return MapValidationResult.Fail("UnknownRegionLayer", $"区域引用的图层不存在：{region.RegionId}。");
            if (!carrierIds.Contains(region.LayerId))
                return MapValidationResult.Fail("RegionOnBaseLayer", $"区域不得挂载到基础地图层：{region.DisplayName}。");
            if (string.IsNullOrWhiteSpace(region.DisplayName))
                return MapValidationResult.Fail("InvalidRegionName", $"区域名称不能为空：{region.RegionId}。");

            var vertices = region.Vertices;
            if (vertices.IsDefault || vertices.Length < 3)
                return MapValidationResult.Fail("TooFewRegionVertices", $"区域顶点数必须至少为 3：{region.DisplayName}。");
            if (vertices.Length > MaxVerticesPerRegion)
                return MapValidationResult.Fail("TooManyRegionVertices", $"区域顶点数超过上限 {MaxVerticesPerRegion}：{region.DisplayName}。");

            var distinct = new HashSet<MapPoint>();
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                if (!Finite(vertex.X) || !Finite(vertex.Y))
                    return MapValidationResult.Fail("NonFiniteRegionVertex", $"区域顶点必须为有限数值：{region.DisplayName}。");
                if (!MapBounds.Contains(size, vertex.X, vertex.Y))
                    return MapValidationResult.Fail("RegionVertexOutOfBounds", $"区域顶点超出地图边界：{region.DisplayName}。");
                var next = vertices[(i + 1) % vertices.Length];
                if (vertex == next)
                    return MapValidationResult.Fail("AdjacentDuplicateVertex", $"区域存在相邻重复顶点（含首尾）：{region.DisplayName}。");
                distinct.Add(vertex);
            }

            if (distinct.Count < 3)
                return MapValidationResult.Fail("TooFewDistinctVertices", $"区域不同顶点数必须至少为 3：{region.DisplayName}。");
            if (!(ShoelaceArea(vertices) > MinAreaSquareMeters))
                return MapValidationResult.Fail("ZeroAreaRegion", $"区域面积接近零：{region.DisplayName}。");
        }

        return MapValidationResult.Ok();
    }

    // 鞋带公式（自动闭合：最后一条边连接尾点→首点）。共线三点面积为零。
    static double ShoelaceArea(ImmutableArray<MapPoint> vertices)
    {
        var sum = 0.0;
        for (var i = 0; i < vertices.Length; i++)
        {
            var a = vertices[i];
            var b = vertices[(i + 1) % vertices.Length];
            sum += a.X * b.Y - b.X * a.Y;
        }

        return Math.Abs(sum) / 2.0;
    }

    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}
