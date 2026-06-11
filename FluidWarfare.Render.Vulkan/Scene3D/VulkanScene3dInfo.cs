namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Vulkan 3D 场景渲染结果模型。
/// </summary>
public sealed record VulkanScene3dInfo(
    VulkanScene3dStatus Status,
    string Message,
    int GridVertexCount,
    int GridLineCount,
    int UnitVertexCount,
    int UnitTriangleCount,
    int DrawCallCount,
    int ViewportWidth,
    int ViewportHeight,
    string CameraSummary,
    double ElapsedMilliseconds)
{
    public bool IsSucceeded => Status == VulkanScene3dStatus.Succeeded;

    public static VulkanScene3dInfo NotChecked { get; } =
        new(VulkanScene3dStatus.NotChecked, "Vulkan 3D 场景尚未绘制。",
            0, 0, 0, 0, 0, 0, 0, "未知", 0);
}
