namespace XuanYu.Core.Map;

// MAP-A-R1-D4：CPU 地形网格构建器（唯一采样源的渲染侧消费方）。
// 顶点高度来自 MapSurfaceSampler；法线为数值差分；亮度为 CPU 预计算的基础
// 方向光（Lambert）+ 半球环境光，shader 地形分支直接输出，不再注入光照参数。
// 地图尺寸与画面一致：X ∈ [-W/2, W/2]、Y ∈ [-D/2, D/2]，闭区间。
public static class MapTerrainMeshBuilder
{
    public const int DefaultSegments = 64;

    public static MapTerrainMesh Build(MapRenderSnapshot map, int segments = DefaultSegments)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(segments, 2);
        var stepX = map.WidthMeters / segments;
        var stepY = map.DepthMeters / segments;
        var vertexCount = (segments + 1) * (segments + 1);
        var vertices = new MapTerrainVertex[vertexCount];
        var halfW = map.WidthMeters / 2.0;
        var halfD = map.DepthMeters / 2.0;

        var index = 0;
        for (var gy = 0; gy <= segments; gy++)
        {
            for (var gx = 0; gx <= segments; gx++)
            {
                var x = -halfW + gx * stepX;
                var y = -halfD + gy * stepY;
                var z = MapSurfaceSampler.SampleHeight(
                    map.SurfaceKind, map.BaseHeightMeters, map.AmplitudeMeters,
                    map.WavelengthMeters, map.Seed, x, y);
                var (nx, ny, nz) = Normal(map, x, y, stepX, stepY);
                var brightness = Brightness(map, nx, ny, nz);
                vertices[index++] = new MapTerrainVertex(
                    (float)x, (float)y, (float)z,
                    (float)nx, (float)ny, (float)nz,
                    (float)brightness, 0.0f);
            }
        }

        return new MapTerrainMesh(vertices, BuildIndices(segments));
    }

    static (double Nx, double Ny, double Nz) Normal(
        MapRenderSnapshot map, double x, double y, double stepX, double stepY)
    {
        var sample = (double px, double py) => MapSurfaceSampler.SampleHeight(
            map.SurfaceKind, map.BaseHeightMeters, map.AmplitudeMeters,
            map.WavelengthMeters, map.Seed, px, py);
        var dzDx = (sample(x + stepX, y) - sample(x - stepX, y)) / (2.0 * stepX);
        var dzDy = (sample(x, y + stepY) - sample(x, y - stepY)) / (2.0 * stepY);
        var len = System.Math.Sqrt(dzDx * dzDx + dzDy * dzDy + 1.0);
        return (-dzDx / len, -dzDy / len, 1.0 / len);
    }

    // sunDirection = 指向光源方向（光射来方向，D1 合同冻结，Z>0 朝上）；
    // Lambert 直接使用 sunDirection 单位化后的方向点积法线。
    // F4：降低环境光与方向光的满额叠加，避免全部顶点被 shader clamp 成同色；
    // 目标区间约 [0.5, 0.85]，保留受光/背光可见明暗差。
    static double Brightness(MapRenderSnapshot map, double nx, double ny, double nz)
    {
        var sunLen = System.Math.Sqrt(
            map.SunDirectionX * map.SunDirectionX +
            map.SunDirectionY * map.SunDirectionY +
            map.SunDirectionZ * map.SunDirectionZ);
        if (sunLen <= 0.0) return map.AmbientIntensity * 0.3;
        var toLightX = map.SunDirectionX / sunLen;
        var toLightY = map.SunDirectionY / sunLen;
        var toLightZ = map.SunDirectionZ / sunLen;
        var ndl = System.Math.Max(nx * toLightX + ny * toLightY + nz * toLightZ, 0.0);
        var hemi = System.Math.Clamp(nz * 0.5 + 0.5, 0.0, 1.0);
        var combined = map.AmbientIntensity * 0.3 * hemi + map.SunIntensity * 0.85 * ndl;
        return System.Math.Clamp(combined, 0.0, 1.0);
    }

    static uint[] BuildIndices(int segments)
    {
        var indices = new uint[segments * segments * 6];
        var i = 0;
        var stride = segments + 1;
        for (var gy = 0; gy < segments; gy++)
        {
            for (var gx = 0; gx < segments; gx++)
            {
                var a = (uint)(gy * stride + gx);
                var b = (uint)(gy * stride + gx + 1);
                var c = (uint)((gy + 1) * stride + gx);
                var d = (uint)((gy + 1) * stride + gx + 1);
                indices[i++] = a; indices[i++] = c; indices[i++] = b;
                indices[i++] = b; indices[i++] = c; indices[i++] = d;
            }
        }

        return indices;
    }
}

public readonly record struct MapTerrainMesh(
    MapTerrainVertex[] Vertices,
    uint[] Indices);
