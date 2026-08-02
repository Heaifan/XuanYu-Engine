namespace XuanYu.Core.Map;

// MAP-A-R1-D4：地图边界线 + 四角标识构建（XY 平面细条四边形，48 顶点）。
// Z = 地表采样高度（微抬升防 z-fighting），亮色由 shader 边界分支给出。
public static class MapBoundsMeshBuilder
{
    public static MapTerrainVertex[] BuildBounds(MapRenderSnapshot map)
    {
        const float lift = 0.05f;
        var halfW = map.WidthMeters / 2.0;
        var halfD = map.DepthMeters / 2.0;
        var sample = (double px, double py) => MapSurfaceSampler.SampleHeight(
            map.SurfaceKind, map.BaseHeightMeters, map.AmplitudeMeters,
            map.WavelengthMeters, map.Seed, px, py);
        var corner = new[]
        {
            new Xy(-halfW, -halfD), new Xy(halfW, -halfD),
            new Xy(halfW, halfD), new Xy(-halfW, halfD)
        };
        var vertices = new List<MapTerrainVertex>(48);
        for (var i = 0; i < 4; i++)
        {
            var a = corner[i];
            var b = corner[(i + 1) % 4];
            AddStrip(vertices, a, b, sample, lift);
        }

        for (var i = 0; i < 4; i++)
        {
            var c = corner[i];
            AddStrip(vertices,
                new Xy(c.X - 0.4, c.Y - 0.4), new Xy(c.X + 0.4, c.Y + 0.4),
                sample, lift);
        }

        return vertices.ToArray();
    }

    static void AddStrip(List<MapTerrainVertex> vertices, Xy a, Xy b,
        Func<double, double, double> sample, float lift)
    {
        const float width = 0.3f;
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var len = System.Math.Sqrt(dx * dx + dy * dy);
        if (len <= 0.0001) return;
        var px = -dy / len * width;
        var py = dx / len * width;
        var za = (float)sample(a.X, a.Y) + lift;
        var zb = (float)sample(b.X, b.Y) + lift;
        vertices.Add(NewV(a.X + px, a.Y + py, za));
        vertices.Add(NewV(b.X + px, b.Y + py, zb));
        vertices.Add(NewV(b.X - px, b.Y - py, zb));
        vertices.Add(NewV(a.X - px, a.Y - py, za));
        vertices.Add(NewV(a.X + px, a.Y + py, za));
        vertices.Add(NewV(b.X - px, b.Y - py, zb));
    }

    static MapTerrainVertex NewV(double x, double y, double z) =>
        new((float)x, (float)y, (float)z, 0, 0, 1, 1.0f, 0.0f);

    readonly record struct Xy(double X, double Y);
}
