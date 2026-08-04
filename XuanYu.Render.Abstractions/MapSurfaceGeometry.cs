using XuanYu.Core.Map;

namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3：有限 Flat 地面常量几何——固定 4 顶点 / 6 索引（两个三角形），
// 地图尺寸只进入顶点坐标，绝不随米数增加顶点（D3 红线：禁止按米细分）。
// 顶点顺序（D3 合同）：左下 → 右下 → 右上 → 左上；索引 0,1,2 / 0,2,3。
// Z-Up：X/Y 水平面，Z = BaseHeightMeters。
public readonly record struct MapSurfaceGeometry(
    MapTerrainVertex[] Vertices,
    uint[] Indices)
{
    public const int VertexCount = 4;

    public const int IndexCount = 6;
}

public static class MapSurfaceGeometryBuilder
{
    public static MapSurfaceGeometry Build(MapRenderSnapshot map)
    {
        var halfW = map.WidthMeters / 2.0;
        var halfD = map.DepthMeters / 2.0;
        var z = (float)map.BaseHeightMeters;
        var vertices = new MapTerrainVertex[MapSurfaceGeometry.VertexCount];
        vertices[0] = V(-halfW, -halfD, z);
        vertices[1] = V(halfW, -halfD, z);
        vertices[2] = V(halfW, halfD, z);
        vertices[3] = V(-halfW, halfD, z);
        return new MapSurfaceGeometry(vertices, [0, 1, 2, 0, 2, 3]);
    }

    // 平面法线 +Z、亮度 1.0（shader 地表分支直接输出基色，不依赖预计算光照）。
    static MapTerrainVertex V(double x, double y, float z) =>
        new((float)x, (float)y, z, 0, 0, 1, 1.0f, 0.0f);
}
