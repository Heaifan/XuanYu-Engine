namespace XuanYu.Engine.Render.Vulkan.Scene3D.GroundCursor;

/// <summary>
/// Ground Cursor 几何数据（Z-Up / XY 地面）。
/// 生成固定顶点：中心十字 + 外部小方框，约 12 个线段顶点。
/// 所有顶点 Z=0（地面），由世界平移提升至 Z+0.02 防 Z-fighting。
/// 创建一次即可，不随点击重新生成。
/// </summary>
public static class VulkanGroundCursorGeometry
{
    /// <summary>
    /// 生成 Ground Cursor 顶点。
    /// 青色 (0.0, 1.0, 1.0, 1.0)，Z 偏移由世界平移提供。
    /// 视觉范围约 0.8 × 0.8 米。
    /// </summary>
    public static VulkanScene3dVertex[] Create()
    {
        const float size = 0.4f;          // 半边长
        const float cx = 0f, cy = 0f;     // 局部中心（XY 地面）
        const float r = 0f, g = 1f, b = 1f, a = 1f; // 青色

        return
        [
            // ─── 中心十字（4 条线段 = 8 个顶点） ───────────────────
            // X 方向横线
            new VulkanScene3dVertex(cx - size, cy, 0, r, g, b, a),
            new VulkanScene3dVertex(cx + size, cy, 0, r, g, b, a),
            // Y 方向竖线
            new VulkanScene3dVertex(cx, cy - size, 0, r, g, b, a),
            new VulkanScene3dVertex(cx, cy + size, 0, r, g, b, a),

            // ─── 外部小方框（4 条线段 = 8 个顶点） ─────────────────
            // 上边
            new VulkanScene3dVertex(cx - size * 0.55f, cy - size * 0.55f, 0, r, g, b, a),
            new VulkanScene3dVertex(cx - size * 0.55f, cy + size * 0.55f, 0, r, g, b, a),
            // 右边
            new VulkanScene3dVertex(cx - size * 0.55f, cy + size * 0.55f, 0, r, g, b, a),
            new VulkanScene3dVertex(cx + size * 0.55f, cy + size * 0.55f, 0, r, g, b, a),
            // 下边
            new VulkanScene3dVertex(cx + size * 0.55f, cy + size * 0.55f, 0, r, g, b, a),
            new VulkanScene3dVertex(cx + size * 0.55f, cy - size * 0.55f, 0, r, g, b, a),
            // 左边
            new VulkanScene3dVertex(cx + size * 0.55f, cy - size * 0.55f, 0, r, g, b, a),
            new VulkanScene3dVertex(cx - size * 0.55f, cy - size * 0.55f, 0, r, g, b, a),
        ];
    }

    /// <summary>
    /// Ground Cursor 顶点数量（固定 12 个 = 6 条线段）。
    /// </summary>
    public const int VertexCount = 12;
}

