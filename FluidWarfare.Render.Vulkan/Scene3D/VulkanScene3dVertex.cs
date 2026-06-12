namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 3D 场景顶点，包含位置和颜色。
/// 用于地面网格和单位占位物的顶点数据。
/// </summary>
public readonly record struct VulkanScene3dVertex(
    float X,
    float Y,
    float Z,
    float R,
    float G,
    float B,
    float A);

/// <summary>
/// 单位 3D 绘制信息：世界坐标和缩放。
/// 由 Editor 从 RenderScene 对象转换而来，Vulkan 层据此计算每对象 MVP。
/// </summary>
public readonly record struct VulkanScene3dUnitDrawInfo(
    string? EntityId,
    float X,
    float Y,
    float Z,
    float Scale);

/// <summary>
/// 3D 场景顶点数据生成工具。
/// </summary>
public static class VulkanScene3dVertices
{
    /// <summary>
    /// 生成地面网格顶点（线段列表）。
    /// 范围：-gridExtent 到 +gridExtent，间隔 gridSpacing。
    /// 每两条线段需要 2 个顶点。
    /// yOffset 用于将网格略微下沉以避免与单位底面的 Z-fighting。
    /// </summary>
    public static VulkanScene3dVertex[] BuildGrid(int gridExtent, int gridSpacing,
        float r = 0.35f, float g = 0.40f, float b = 0.50f, float a = 1.0f,
        float yOffset = -0.01f)
    {
        var vertices = new List<VulkanScene3dVertex>();

        for (var x = -gridExtent; x <= gridExtent; x += gridSpacing)
        {
            // Z 方向线段
            vertices.Add(new VulkanScene3dVertex(x, yOffset, -gridExtent, r, g, b, a));
            vertices.Add(new VulkanScene3dVertex(x, yOffset, gridExtent, r, g, b, a));
        }

        for (var z = -gridExtent; z <= gridExtent; z += gridSpacing)
        {
            // X 方向线段
            vertices.Add(new VulkanScene3dVertex(-gridExtent, yOffset, z, r, g, b, a));
            vertices.Add(new VulkanScene3dVertex(gridExtent, yOffset, z, r, g, b, a));
        }

        return [.. vertices];
    }

    /// <summary>
    /// 生成立方体顶点（三角形列表）。
    /// 中心在 (cx, cy, cz)，尺寸 size x size x size。
    /// 每个面 2 个三角形 = 6 个顶点。
    /// </summary>
    public static VulkanScene3dVertex[] BuildCube(float cx, float cy, float cz, float size,
        float r = 1.0f, float g = 0.82f, float b = 0.20f, float a = 1.0f)
    {
        var h = size / 2;
        var (x0, x1) = (cx - h, cx + h);
        var (y0, y1) = (cy - h, cy + h);
        var (z0, z1) = (cz - h, cz + h);

        // 12 triangles = 36 vertices (每个三角形 3 个顶点)
        // Front, Back, Top, Bottom, Left, Right faces (2 tris each)
        var verts = new[]
        {
            // Front face (z+)
            new VulkanScene3dVertex(x0, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z1, r, g, b, a),
            // Back face (z-)
            new VulkanScene3dVertex(x1, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z0, r, g, b, a),
            // Top face (y+)
            new VulkanScene3dVertex(x0, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z0, r, g, b, a),
            // Bottom face (y-)
            new VulkanScene3dVertex(x0, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z1, r, g, b, a),
            // Left face (x-)
            new VulkanScene3dVertex(x0, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x0, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x0, y1, z0, r, g, b, a),
            // Right face (x+)
            new VulkanScene3dVertex(x1, y0, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z1, r, g, b, a),
            new VulkanScene3dVertex(x1, y0, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z0, r, g, b, a),
            new VulkanScene3dVertex(x1, y1, z1, r, g, b, a),
        };

        return verts;
    }

    /// <summary>
    /// 生成 X/Y/Z 轴线（调试用），LineList。
    /// </summary>
    public static VulkanScene3dVertex[] BuildAxes(float length = 2.0f)
    {
        return
        [
            // X axis (red)
            new VulkanScene3dVertex(-length, 0, 0, 1, 0.2f, 0.2f, 1),
            new VulkanScene3dVertex(length, 0, 0, 1, 0.2f, 0.2f, 1),
            // Y axis (green)
            new VulkanScene3dVertex(0, -length, 0, 0.2f, 1, 0.2f, 1),
            new VulkanScene3dVertex(0, length, 0, 0.2f, 1, 0.2f, 1),
            // Z axis (blue)
            new VulkanScene3dVertex(0, 0, -length, 0.2f, 0.2f, 1, 1),
            new VulkanScene3dVertex(0, 0, length, 0.2f, 0.2f, 1, 1),
        ];
    }

    /// <summary>
    /// 将顶点数组转换为 float[] (x,y,z,r,g,b,a) 交错格式。
    /// 用于 Vulkan Vertex Buffer。
    /// </summary>
    public static float[] ToInterleaved(ReadOnlySpan<VulkanScene3dVertex> vertices)
    {
        var result = new float[vertices.Length * 7];
        for (var i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];
            var offset = i * 7;
            result[offset] = v.X;
            result[offset + 1] = v.Y;
            result[offset + 2] = v.Z;
            result[offset + 3] = v.R;
            result[offset + 4] = v.G;
            result[offset + 5] = v.B;
            result[offset + 6] = v.A;
        }
        return result;
    }
}
