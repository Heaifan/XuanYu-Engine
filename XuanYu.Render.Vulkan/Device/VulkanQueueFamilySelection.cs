namespace XuanYu.Render.Vulkan.Device;

// VK4-A：纯数据队列族选择结果。索引为 -1 表示未找到对应能力。
// 仅承载 Graphics / Present 队列族索引与可用性，不持有任何 Vulkan 句柄。
public sealed record VulkanQueueFamilySelection(
    int GraphicsFamily,
    int PresentFamily,
    bool HasGraphics,
    bool HasPresent,
    bool SameFamily)
{
    public static readonly VulkanQueueFamilySelection None =
        new(-1, -1, false, false, false);
}
