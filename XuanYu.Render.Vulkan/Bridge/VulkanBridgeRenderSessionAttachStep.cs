using System;
using Silk.NET.Vulkan;
using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Swapchain;
using XuanYu.Render.Vulkan.Session;

namespace XuanYu.Render.Vulkan.Bridge;

// VK4-D：把 RenderSession 创建从 Bridge 抽离，Bridge 只委托，不内联 VK4-D 细节。
public sealed class VulkanBridgeRenderSessionAttachStep
{
    public static VulkanRenderSession? Run(Vk vk, VulkanDeviceOwner? deviceOwner,
        VulkanSwapchainOwner? swapchainOwner, VulkanPhysicalDeviceSelection? selection,
        Action<string>? log, NativeHostSurfaceHandle? handle = null, SceneRenderSnapshot? scene = null)
        => VulkanRenderSession.Create(vk, deviceOwner, swapchainOwner, selection, log, handle, scene);
}
