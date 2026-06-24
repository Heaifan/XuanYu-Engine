namespace XuanYu.Engine.Render.Vulkan.Scene3D.Axes;

/// <summary>
/// 世界 X/Y/Z 主轴几何数据生成（Z-Up）。
/// X 红、Y 绿、Z 蓝。
/// </summary>
public static class VulkanSceneAxisGeometry
{
    /// <summary>
    /// 生成世界主轴顶点（LineList）。
    /// X 和 Y 沿地面分布，Z 从原点垂直向上。
    /// </summary>
    /// <param name="xyLength">X/Y 轴总长度（半幅）。</param>
    /// <param name="zLength">Z 轴长度（向上）。</param>
    public static VulkanScene3dVertex[] Build(float xyLength = 20f, float zLength = 8f)
    {
        return
        [
            // X axis (red): -xyLength 至 +xyLength
            new VulkanScene3dVertex(-xyLength, 0, 0, 1, 0, 0, 1),
            new VulkanScene3dVertex( xyLength, 0, 0, 1, 0, 0, 1),
            // Y axis (green): -xyLength 至 +xyLength
            new VulkanScene3dVertex(0, -xyLength, 0, 0, 1, 0, 1),
            new VulkanScene3dVertex(0,  xyLength, 0, 0, 1, 0, 1),
            // Z axis (blue): 0 至 +zLength (从地面向上)
            new VulkanScene3dVertex(0, 0, 0, 0, 0, 1, 1),
            new VulkanScene3dVertex(0, 0, zLength, 0, 0, 1, 1),
        ];
    }

    /// <summary>主轴顶点数量。</summary>
    public const int VertexCount = 6;
}
