namespace XuanYu.Render.Vulkan.Render;

public enum VulkanPresentLoopState
{
    Stopped,
    Starting,
    Running,
    Failed
}

internal static class VulkanPresentLoopStatePolicy
{
    internal static bool CanStart(VulkanPresentLoopState state) => state != VulkanPresentLoopState.Failed;

    internal static VulkanPresentLoopState AfterWorkerExit(bool failed) => failed
        ? VulkanPresentLoopState.Failed
        : VulkanPresentLoopState.Stopped;
}
