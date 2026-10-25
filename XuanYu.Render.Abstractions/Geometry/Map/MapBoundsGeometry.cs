using XuanYu.Core.Map;

namespace XuanYu.Render.Abstractions;

// MAP-A-R2-D3：地图边界几何——四条边各一条细条四边形（每边 6 顶点 = 2 三角形），
// 共 24 顶点；禁止用小线段近似矩形（D3 合同），禁止把边界创建为 MapRegion。
// 宽度取世界宽度并设上下限（D3 允许的过渡方案：真机验证远近后决定是否屏幕恒宽）。
// Z = BaseHeight + 渲染抬升（lift 仅渲染侧偏移，绝不修改地图领域高度）。
public static class MapBoundsGeometryBuilder
{
    public const int VertexCount = 24;

    // 宽度公式：地图尺寸的千分之一，钳制 [1, 50] 米（10 km → 10 m，聚焦下约 2 px 可辨认）。
    public static double StripWidthMeters(MapRenderSnapshot map) =>
        System.Math.Clamp(System.Math.Min(map.WidthMeters, map.DepthMeters) * 0.001, 1.0, 50.0);

    public static MapTerrainVertex[] Build(MapRenderSnapshot map)
    {
        const float lift = 0.05f;
        var halfW = map.WidthMeters / 2.0;
        var halfD = map.DepthMeters / 2.0;
        var z = (float)map.BaseHeightMeters + lift;
        var corners = new[]
        {
            new Xy(-halfW, -halfD), new Xy(halfW, -halfD),
            new Xy(halfW, halfD), new Xy(-halfW, halfD)
        };
        var width = (float)StripWidthMeters(map);
        var vertices = new MapTerrainVertex[VertexCount];
        var v = 0;
        for (var i = 0; i < 4; i++)
        {
            var a = corners[i];
            var b = corners[(i + 1) % 4];
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var len = System.Math.Sqrt(dx * dx + dy * dy);
            var px = (float)(-dy / len * width);
            var py = (float)(dx / len * width);
            vertices[v++] = V(a.X + px, a.Y + py, z);
            vertices[v++] = V(b.X + px, b.Y + py, z);
            vertices[v++] = V(b.X - px, b.Y - py, z);
            vertices[v++] = V(a.X - px, a.Y - py, z);
            vertices[v++] = V(a.X + px, a.Y + py, z);
            vertices[v++] = V(b.X - px, b.Y - py, z);
        }

        return vertices;
    }

    static MapTerrainVertex V(double x, double y, float z) =>
        new((float)x, (float)y, z, 0, 0, 1, 1.0f, 0.0f);

    readonly record struct Xy(double X, double Y);
}
