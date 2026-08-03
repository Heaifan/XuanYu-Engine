using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// MAP-A-R1-D5-R1-F2-R2：全屏 Pass 工厂（网格 / 世界轴 / 世界原点）。
// 三个 Pass 共用 CreateFullscreenPass 通用创建（全屏三角形、深度测试开、深度写关、混合开）。
// 网格 PushConstant 176B（含 gridScale）；轴/原点使用同布局（gridScale 未用）。
internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateReferenceGrid(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeGridFrag.Code, VulkanClearFrameOwner.ReferenceGridPushSize, log);

    internal static VulkanGraphicsPipelineOwner? CreateWorldAxes(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeWorldAxesFrag.Code, VulkanClearFrameOwner.ReferenceGridPushSize, log);

    internal static VulkanGraphicsPipelineOwner? CreateWorldOrigin(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeWorldOriginFrag.Code, VulkanClearFrameOwner.ReferenceGridPushSize, log);

    // F3-F1：导航 Gizmo Overlay Pass——屏幕空间、深度测试/写入关闭、始终最后绘制（不受原生窗口遮挡）。
    internal static VulkanGraphicsPipelineOwner? CreateNavigationGizmo(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeNavGizmoVert.Code, ShaderBytecodeNavGizmoFrag.Code, VulkanClearFrameOwner.NavGizmoPushSize, log,
            depthTest: false);
}
