using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 创建 Scene3D PipelineLayout 与 PushConstantRange。
/// MVP (mat4, 64 字节) + Tint (vec4, 16 字节) = 80 字节。
/// </summary>
public static unsafe class VulkanScene3dPipelineLayout
{
    /// <summary>Push Constant 总字节数。</summary>
    public const uint TotalPushConstantBytes = VulkanScene3dPushConstants.ByteSize;

    /// <summary>
    /// 创建 PipelineLayout。
    /// 检查设备最大 PushConstants 大小是否 >= 80。
    /// </summary>
    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        out PipelineLayout pipelineLayout, out string errorMessage)
    {
        pipelineLayout = default;
        errorMessage = string.Empty;

        // 检查设备 Push Constants 支持
        var limits = default(PhysicalDeviceLimits);
        unsafe
        {
            var props = new PhysicalDeviceProperties();
            vk.GetPhysicalDeviceProperties(physicalDevice, &props);
            limits = props.Limits;
        }

        if (limits.MaxPushConstantsSize < TotalPushConstantBytes)
        {
            errorMessage =
                $"Scene3D PipelineLayout：设备 MaxPushConstantsSize ({limits.MaxPushConstantsSize}) " +
                $"小于所需 {TotalPushConstantBytes} 字节。";
            return false;
        }

        var pcRange = new PushConstantRange
        {
            StageFlags = ShaderStageFlags.VertexBit,
            Offset = 0,
            Size = TotalPushConstantBytes
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
            errorMessage = $"Scene3D PipelineLayout：vkCreatePipelineLayout 返回 {result}。";
            return false;
        }

        return true;
    }
}
