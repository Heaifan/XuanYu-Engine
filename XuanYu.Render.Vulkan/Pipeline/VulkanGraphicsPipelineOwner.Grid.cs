using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// GRID-RW-1：Reference Grid 使用世界线 LineList；轴、原点和屏幕 Overlay 仍使用全屏三角形。
// GRID-RW-1-CORR2：管线工厂改为专用 Empty-input Line Pipeline（无 StaticModel binding + 负 Depth Bias），
// 实现见 VulkanGraphicsPipelineOwner.GridLine.cs。
internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateReferenceGrid(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateReferenceGridLinePass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridLineVert.Code, ShaderBytecodeGridLineFrag.Code, log);

    internal static VulkanGraphicsPipelineOwner? CreateWorldAxes(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeWorldAxesFrag.Code, VulkanClearFrameOwner.ReferenceGridPushSize, log);

    internal static VulkanGraphicsPipelineOwner? CreateWorldOrigin(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeWorldOriginFrag.Code, VulkanClearFrameOwner.ReferenceGridPushSize, log,
            depthTest: false);

    // F3-F1：导航 Gizmo Overlay Pass——屏幕空间、深度测试/写入关闭、始终最后绘制（不受原生窗口遮挡）。
    internal static VulkanGraphicsPipelineOwner? CreateNavigationGizmo(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeNavGizmoVert.Code, ShaderBytecodeNavGizmoFrag.Code, VulkanClearFrameOwner.NavGizmoPushSize, log,
            depthTest: false);

    internal static VulkanGraphicsPipelineOwner? CreateScaleIndicator(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeNavGizmoVert.Code, ShaderBytecodeScaleIndicatorFrag.Code,
            VulkanClearFrameOwner.ScaleIndicatorPushSize, log, depthTest: false);

    // F3-F4：正交标准视图的视图平面网格（复用 GridVert；独立 192B PushConstant 含平面法线）。
    internal static VulkanGraphicsPipelineOwner? CreateViewPlaneGrid(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
        => CreateFullscreenPass(vk, deviceOwner, clearFrame, swapchain, physicalDevice,
            ShaderBytecodeGridVert.Code, ShaderBytecodeViewPlaneGridFrag.Code, VulkanClearFrameOwner.ViewPlaneGridPushSize, log);
}
