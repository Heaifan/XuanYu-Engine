using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 创建 Scene3D PipelineLayout 与 MVP PushConstantRange。
/// MVP 为 4x4 float，共 16 × 4 = 64 字节。
/// </summary>
public static unsafe class VulkanScene3dPipelineLayout
{
    public const uint MvpPushConstantBytes = 64;

    /// <summary>
    /// 创建 PipelineLayout。
    /// </summary>
    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        out PipelineLayout pipelineLayout, out string errorMessage)
    {
        pipelineLayout = default;
        errorMessage = string.Empty;

        var pcRange = new PushConstantRange
        {
            StageFlags = ShaderStageFlags.VertexBit,
            Offset = 0,
            Size = MvpPushConstantBytes
        };

        if (pcRange.Size == 0)
        {
            errorMessage = "Scene3D PipelineLayout：PushConstantRange 字节数为 0。";
            return false;
        }

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
