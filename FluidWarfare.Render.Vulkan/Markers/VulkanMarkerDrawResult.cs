namespace FluidWarfare.Render.Vulkan.Markers;

/// <summary>
/// Vulkan 点位绘制结果模型。
/// </summary>
public sealed record VulkanMarkerDrawResult(
    VulkanMarkerDrawStatus Status,
    string Message,
    int DrawnMarkerCount,
    double ElapsedMilliseconds)
{
    public bool IsSucceeded => Status == VulkanMarkerDrawStatus.Succeeded;

    public static VulkanMarkerDrawResult NotChecked { get; } =
        new(VulkanMarkerDrawStatus.NotChecked, "Vulkan 点位绘制尚未执行。", 0, 0);
}
