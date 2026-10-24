namespace XuanYu.Core.Map;

// MAP-A-R1-D4：地形网格顶点。布局与 Vulkan 侧 StaticModelVertex 一致：
// pos(3f) + normal(3f) + uv(2f)，stride 32；uv.x 承载 CPU 预计算 Lambert 亮度。
public readonly record struct MapTerrainVertex(
    float X, float Y, float Z,
    float Nx, float Ny, float Nz,
    float Brightness, float Pad)
{
    public const uint Stride = 32;
}
