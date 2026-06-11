namespace FluidWarfare.Render.Vulkan.Markers;

/// <summary>
/// 点位绘制信息，描述一个需要在 Vulkan 视口中绘制的点位。
/// 坐标映射规则（当前初版）：
///   pixelX = viewportWidth / 2 + worldX * 10
///   pixelY = viewportHeight / 2 - worldZ * 10
/// 下一阶段 8.1 会加入完整相机系统，不再硬编码此规则。
/// </summary>
public sealed record VulkanMarkerDrawInfo(
    string DisplayName,
    int PixelX,
    int PixelY,
    int PixelSize,
    string ColorText)
{
    /// <summary>
    /// 返回一个在视口中心绘制点位的默认信息。
    /// </summary>
    public static VulkanMarkerDrawInfo CreateAtCenter(
        string displayName,
        int viewportWidth,
        int viewportHeight,
        int pixelSize = 12)
    {
        return new VulkanMarkerDrawInfo(
            displayName,
            viewportWidth / 2,
            viewportHeight / 2,
            pixelSize,
            "rgba(1.00, 0.82, 0.20, 1.00)");
    }

    /// <summary>
    /// 从世界坐标映射到视口像素坐标。
    /// 初版使用固定比例映射，无相机系统。
    /// </summary>
    public static VulkanMarkerDrawInfo FromWorldPosition(
        string displayName,
        float worldX,
        float worldZ,
        int viewportWidth,
        int viewportHeight,
        int pixelSize = 12)
    {
        var pixelX = viewportWidth / 2 + (int)(worldX * 10.0f);
        var pixelY = viewportHeight / 2 - (int)(worldZ * 10.0f);

        return new VulkanMarkerDrawInfo(
            displayName,
            pixelX,
            pixelY,
            pixelSize,
            "rgba(1.00, 0.82, 0.20, 1.00)");
    }
}
