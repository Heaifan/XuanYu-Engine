using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Swapchain;

public static class VulkanSwapchainUsagePolicy
{
    public static ImageUsageFlags Select(ImageUsageFlags supported, bool requestReadback)
    {
        if ((supported & ImageUsageFlags.ColorAttachmentBit) == 0) return 0;
        var selected = ImageUsageFlags.ColorAttachmentBit;
        if (requestReadback && (supported & ImageUsageFlags.TransferSrcBit) != 0)
            selected |= ImageUsageFlags.TransferSrcBit;
        return selected;
    }
}
