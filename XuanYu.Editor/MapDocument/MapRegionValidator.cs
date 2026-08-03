using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：区域集合严格校验。返回结构化结果，不抛出来源不明的异常。
// 检查：ID 唯一、引用图层存在、名称非空、至少三个顶点、已闭合、
// 顶点数上限、顶点有限数值、顶点位于地图边界内。
public static class MapRegionValidator
{
    public const int MaxVerticesPerRegion = 1024;

    public static MapDocumentResult<bool> Validate(
        ImmutableArray<MapRegion> regions,
        ImmutableArray<MapLayer> layers,
        MapSize size)
    {
        if (regions.IsDefault)
            return Fail("InvalidRegionList", "区域集合缺失。");

        var layerIds = layers.IsDefault ? new HashSet<MapLayerId>() : layers.Select(l => l.LayerId).ToHashSet();
        var ids = new HashSet<MapRegionId>();
        foreach (var region in regions)
        {
            if (!ids.Add(region.RegionId))
                return Fail("DuplicateRegionId", $"区域ID重复：{region.RegionId}。");
            if (!layerIds.Contains(region.LayerId))
                return Fail("UnknownRegionLayer", $"区域引用的图层不存在：{region.RegionId}。");
            if (string.IsNullOrWhiteSpace(region.DisplayName))
                return Fail("InvalidRegionName", $"区域名称不能为空：{region.RegionId}。");
            if (region.Vertices.IsDefault || region.Vertices.Length < 3)
                return Fail("TooFewRegionVertices", $"区域顶点数必须至少为 3：{region.DisplayName}。");
            if (region.Vertices.Length > MaxVerticesPerRegion)
                return Fail("TooManyRegionVertices", $"区域顶点数超过上限 {MaxVerticesPerRegion}：{region.DisplayName}。");
            if (!region.IsClosed)
                return Fail("OpenRegion", $"区域未闭合：{region.DisplayName}。");
            foreach (var vertex in region.Vertices)
            {
                if (!Finite(vertex.X) || !Finite(vertex.Y))
                    return Fail("NonFiniteRegionVertex", $"区域顶点必须为有限数值：{region.DisplayName}。");
                if (!MapBounds.Contains(size, vertex.X, vertex.Y))
                    return Fail("RegionVertexOutOfBounds", $"区域顶点超出地图边界：{region.DisplayName}。");
            }
        }

        return MapDocumentResult<bool>.Ok(true);
    }

    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    static MapDocumentResult<bool> Fail(string code, string message) =>
        MapDocumentResult<bool>.Fail(code, message, "ValidateRegions");
}
