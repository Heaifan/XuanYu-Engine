using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Swapchain;

namespace XuanYu.Render.Vulkan.Pipeline;

// MAP-A-R1-D5-R1-F2-R2：全屏 Pass 管线通用创建（参考网格 / 世界轴 / 世界原点共用）。
// 全屏三角形、DepthTest=On(GreaterOrEqual)、DepthWrite=Off、AlphaBlend=On；
// 创建时校验设备 maxPushConstantsSize 支持 pushSize 独立 PushConstant。
internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    internal static VulkanGraphicsPipelineOwner? CreateFullscreenPass(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanClearFrameOwner clearFrame, VulkanSwapchainOwner swapchain, PhysicalDevice physicalDevice,
        uint[] vertCode, uint[] fragCode, uint pushSize, Action<string>? log, bool depthTest = true,
        PrimitiveTopology topology = PrimitiveTopology.TriangleList)
    {
        var props = new PhysicalDeviceProperties();
        vk.GetPhysicalDeviceProperties(physicalDevice, &props);
        if (props.Limits.MaxPushConstantsSize < pushSize)
        {
            log?.Invoke(VulkanPipelineLogFormatter.Failed($"全屏 Pass：设备 maxPushConstantsSize={props.Limits.MaxPushConstantsSize} < {pushSize}，Pass 禁用"));
            return null;
        }

        var vert = VulkanShaderModuleOwner.Create(vk, deviceOwner, vertCode);
        var frag = VulkanShaderModuleOwner.Create(vk, deviceOwner, fragCode);
        if (vert.Handle == 0 || frag.Handle == 0)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("全屏 Pass ShaderModule 创建失败"));
            return null;
        }
        var range = new PushConstantRange
        {
            StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
            Offset = 0,
            Size = pushSize
        };
        var layoutInfo = new PipelineLayoutCreateInfo { SType = StructureType.PipelineLayoutCreateInfo, SetLayoutCount = 0, PushConstantRangeCount = 1, PPushConstantRanges = &range };
        if (vk.CreatePipelineLayout(deviceOwner.LogicalDevice, &layoutInfo, null, out var layout) != Result.Success)
        {
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            log?.Invoke(VulkanPipelineLogFormatter.Failed("全屏 Pass CreatePipelineLayout 失败"));
            return null;
        }
        if (!TryCreateFullscreenPipeline(vk, deviceOwner, clearFrame, vert, frag, layout, depthTest, topology, log, out var pipeline))
        {
            vk.DestroyPipelineLayout(deviceOwner.LogicalDevice, layout, null);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
            VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
            return null;
        }
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, vert);
        VulkanShaderModuleOwner.Destroy(vk, deviceOwner, frag);
        log?.Invoke(VulkanPipelineLogFormatter.GridCreated());
        return new VulkanGraphicsPipelineOwner(vk, deviceOwner, layout, pipeline, log);
    }
}
