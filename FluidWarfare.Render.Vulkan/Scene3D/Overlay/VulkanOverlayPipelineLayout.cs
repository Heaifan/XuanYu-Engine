using Silk.NET.Vulkan;
using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>
/// Overlay PipelineLayout：Push Constant = viewportWidth, viewportHeight (8 字节)。
/// </summary>
public static unsafe class VulkanOverlayPipelineLayout
{
    public const uint PushConstantByteSize = 8;

    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        out PipelineLayout pipelineLayout, out string errorMessage)
    {
        pipelineLayout = default;
        errorMessage = string.Empty;

        var limits = default(PhysicalDeviceLimits);
        var props = new PhysicalDeviceProperties();
        vk.GetPhysicalDeviceProperties(physicalDevice, &props);
        limits = props.Limits;

        if (limits.MaxPushConstantsSize < PushConstantByteSize)
        {
            errorMessage = $"Overlay PipelineLayout：MaxPushConstantsSize ({limits.MaxPushConstantsSize}) < {PushConstantByteSize}。";
            return false;
        }

        var pcRange = new PushConstantRange
        {
            StageFlags = ShaderStageFlags.VertexBit,
            Offset = 0,
            Size = PushConstantByteSize
        };

        var ci = new PipelineLayoutCreateInfo
        {
            SType = StructureType.PipelineLayoutCreateInfo,
            PushConstantRangeCount = 1,
            PPushConstantRanges = &pcRange,
            SetLayoutCount = 0,
            PSetLayouts = null
        };

        var result = vk.CreatePipelineLayout(dev, &ci, null, out pipelineLayout);
        if (result != Result.Success)
        {
            errorMessage = $"Overlay PipelineLayout：vkCreatePipelineLayout 返回 {result}。";
            return false;
        }
        return true;
    }
}
