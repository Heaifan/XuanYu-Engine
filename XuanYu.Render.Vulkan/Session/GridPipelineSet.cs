using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Session;

// MAP-A-R1-D5-R1-F2-R2/F3-F1：全屏 Pass 管线组合（参考网格 / 世界轴 / 世界原点 / 导航 Gizmo）。
// 设备不支持时对应 Pass 禁用（返回 null），不阻止编辑器启动。
internal sealed class GridPipelineSet : IDisposable
{
    GridPipelineSet(VulkanGraphicsPipelineOwner? grid, VulkanGraphicsPipelineOwner? axes,
        VulkanGraphicsPipelineOwner? origin, VulkanGraphicsPipelineOwner? navGizmo,
        VulkanGraphicsPipelineOwner? viewPlaneGrid)
    {
        Grid = grid;
        Axes = axes;
        Origin = origin;
        NavGizmo = navGizmo;
        ViewPlaneGrid = viewPlaneGrid;
    }

    public VulkanGraphicsPipelineOwner? Grid { get; }
    public VulkanGraphicsPipelineOwner? Axes { get; }
    public VulkanGraphicsPipelineOwner? Origin { get; }
    public VulkanGraphicsPipelineOwner? NavGizmo { get; }
    public VulkanGraphicsPipelineOwner? ViewPlaneGrid { get; }

    public static GridPipelineSet Create(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clear, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice, Action<string>? log)
    {
        var grid = VulkanGraphicsPipelineOwner.CreateReferenceGrid(vk, deviceOwner, clear, swapchain, physicalDevice, log);
        if (grid is not null) clear.SetReferenceGridPipeline(grid.Pipeline, grid.Layout);
        var axes = VulkanGraphicsPipelineOwner.CreateWorldAxes(vk, deviceOwner, clear, swapchain, physicalDevice, log);
        if (axes is not null) clear.SetWorldAxesPipeline(axes.Pipeline, axes.Layout);
        var origin = VulkanGraphicsPipelineOwner.CreateWorldOrigin(vk, deviceOwner, clear, swapchain, physicalDevice, log);
        if (origin is not null) clear.SetWorldOriginPipeline(origin.Pipeline, origin.Layout);
        var navGizmo = VulkanGraphicsPipelineOwner.CreateNavigationGizmo(vk, deviceOwner, clear, swapchain, physicalDevice, log);
        if (navGizmo is not null) clear.SetNavGizmoPipeline(navGizmo.Pipeline, navGizmo.Layout);
        var viewPlaneGrid = VulkanGraphicsPipelineOwner.CreateViewPlaneGrid(vk, deviceOwner, clear, swapchain, physicalDevice, log);
        if (viewPlaneGrid is not null) clear.SetViewPlaneGridPipeline(viewPlaneGrid.Pipeline, viewPlaneGrid.Layout);
        return new GridPipelineSet(grid, axes, origin, navGizmo, viewPlaneGrid);
    }

    public void Dispose()
    {
        Grid?.Dispose();
        Axes?.Dispose();
        Origin?.Dispose();
        NavGizmo?.Dispose();
        ViewPlaneGrid?.Dispose();
    }
}
