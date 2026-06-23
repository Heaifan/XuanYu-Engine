using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Context;

/// <summary>
/// 死代码仓。TryCreateDeviceResources 当前硬编码 return false。
/// 不修改不复活，仅用于保持原始行为不变。
/// </summary>
sealed unsafe class VulkanRenderContextLegacy
{
    public string LastErrorMessage { get; private set; } = "";

    public bool TryCreateDeviceResources(Vk vk, Silk.NET.Vulkan.Device device, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Instance inst, SurfaceKHR surf, uint qi,
        nint fnCreateSwapchain, nint fnDestroySwapchain, nint fnGetImages,
        nint fnAcquire, nint fnQueuePresent, nint fnGetCaps, nint fnGetFormats, nint fnGetModes)
    {
        LastErrorMessage = "Swapchain 创建在当前环境不可用（函数指针兼容性问题），跳过清屏资源。Instance/Device/Surface 已就绪。";
        return false;
        // ===== 下方死代码永不执行 =====
        // CreateSwapchain(vk, device, pd, inst, surf, fnCreateSwapchain, fnDestroySwapchain, fnGetImages,
        //     fnGetCaps, fnGetFormats, fnGetModes, qi);
        // CreateImageViews(vk, device, fnGetFormats, pd, inst, surf);
        // CreateRenderPass(vk, device, fnGetFormats, pd, inst, surf);
        // CreateFramebuffers(vk, device, pd, inst, surf, fnGetCaps);
        // CreateCommandPool(vk, device, qi);
        // CreateSyncObjects(vk, device);
    }
}
