using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Pipeline;

internal static unsafe class VulkanFullscreenPipelineState
{
    internal static PipelineShaderStageCreateInfo[] ShaderStages(ShaderModule vert,
        ShaderModule frag, byte* entry)
        =>
        [
            new() { SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit, Module = vert, PName = entry },
            new() { SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit, Module = frag, PName = entry }
        ];
}
