namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>Grid / Axis 顶点生成 + 交错格式转换。</summary>
public static partial class VulkanScene3dVertices
{
    /// <summary>生成地面网格顶点（LineList，Z-Up / XY 地面）。</summary>
    public static VulkanScene3dVertex[] BuildGrid(int gridExtent, int gridSpacing,
        float r = 0.35f, float g = 0.40f, float b = 0.50f, float a = 1.0f,
        float groundOffsetZ = -0.01f)
    {
        var list = new List<VulkanScene3dVertex>();
        for (var x = -gridExtent; x <= gridExtent; x += gridSpacing)
        { list.Add(new(x, -gridExtent, groundOffsetZ, r, g, b, a)); list.Add(new(x, gridExtent, groundOffsetZ, r, g, b, a)); }
        for (var y = -gridExtent; y <= gridExtent; y += gridSpacing)
        { list.Add(new(-gridExtent, y, groundOffsetZ, r, g, b, a)); list.Add(new(gridExtent, y, groundOffsetZ, r, g, b, a)); }
        return [.. list];
    }

    /// <summary>生成世界 X/Y/Z 主轴（LineList，Z-Up）。</summary>
    public static VulkanScene3dVertex[] BuildAxes(
        float xyLength = 20f, float zUpLength = 8f, float axisAlpha = 1.0f)
    {
        return
        [
            new(-xyLength, 0, 0, 1, 0, 0, axisAlpha), new(xyLength, 0, 0, 1, 0, 0, axisAlpha),
            new(0, -xyLength, 0, 0, 1, 0, axisAlpha), new(0, xyLength, 0, 0, 1, 0, axisAlpha),
            new(0, 0, 0, 0, 0, 1, axisAlpha), new(0, 0, zUpLength, 0, 0, 1, axisAlpha),
        ];
    }

    /// <summary>将顶点数组转为 float[] (x,y,z,r,g,b,a) 交错格式。</summary>
    public static float[] ToInterleaved(ReadOnlySpan<VulkanScene3dVertex> vertices)
    {
        var result = new float[vertices.Length * 7];
        for (var i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i]; var o = i * 7;
            result[o] = v.X; result[o + 1] = v.Y; result[o + 2] = v.Z;
            result[o + 3] = v.R; result[o + 4] = v.G; result[o + 5] = v.B; result[o + 6] = v.A;
        }
        return result;
    }
}
