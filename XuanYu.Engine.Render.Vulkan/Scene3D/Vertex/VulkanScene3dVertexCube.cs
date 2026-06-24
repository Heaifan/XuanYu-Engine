namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>立方体顶点生成（TriangleList）。</summary>
public static partial class VulkanScene3dVertices
{
    /// <summary>生成立方体顶点（6 个面，每面 2 个三角形 = 36 顶点）。</summary>
    public static VulkanScene3dVertex[] BuildCube(float cx, float cy, float cz, float size,
        float r = 1.0f, float g = 0.82f, float b = 0.20f, float a = 1.0f)
    {
        var h = size / 2; var (x0, x1) = (cx - h, cx + h);
        var (y0, y1) = (cy - h, cy + h); var (z0, z1) = (cz - h, cz + h);
        return
        [
            // Front (z+)
            new(x0, y0, z1, r, g, b, a), new(x1, y0, z1, r, g, b, a), new(x0, y1, z1, r, g, b, a),
            new(x1, y0, z1, r, g, b, a), new(x1, y1, z1, r, g, b, a), new(x0, y1, z1, r, g, b, a),
            // Back (z-)
            new(x1, y0, z0, r, g, b, a), new(x0, y0, z0, r, g, b, a), new(x1, y1, z0, r, g, b, a),
            new(x0, y0, z0, r, g, b, a), new(x0, y1, z0, r, g, b, a), new(x1, y1, z0, r, g, b, a),
            // Top (y+)
            new(x0, y1, z1, r, g, b, a), new(x1, y1, z1, r, g, b, a), new(x0, y1, z0, r, g, b, a),
            new(x1, y1, z1, r, g, b, a), new(x1, y1, z0, r, g, b, a), new(x0, y1, z0, r, g, b, a),
            // Bottom (y-)
            new(x0, y0, z0, r, g, b, a), new(x1, y0, z0, r, g, b, a), new(x0, y0, z1, r, g, b, a),
            new(x1, y0, z0, r, g, b, a), new(x1, y0, z1, r, g, b, a), new(x0, y0, z1, r, g, b, a),
            // Left (x-)
            new(x0, y0, z0, r, g, b, a), new(x0, y0, z1, r, g, b, a), new(x0, y1, z0, r, g, b, a),
            new(x0, y0, z1, r, g, b, a), new(x0, y1, z1, r, g, b, a), new(x0, y1, z0, r, g, b, a),
            // Right (x+)
            new(x1, y0, z1, r, g, b, a), new(x1, y0, z0, r, g, b, a), new(x1, y1, z1, r, g, b, a),
            new(x1, y0, z0, r, g, b, a), new(x1, y1, z0, r, g, b, a), new(x1, y1, z1, r, g, b, a),
        ];
    }
}
