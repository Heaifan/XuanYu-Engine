using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Pipeline.Contracts;

internal static unsafe class VulkanStaticModelVertexInput
{
    internal static VertexInputBindingDescription Binding() =>
        new()
        {
            Binding = 0,
            Stride = VulkanStaticModelVertex.Stride,
            InputRate = VertexInputRate.Vertex
        };

    internal static void FillAttributes(VertexInputAttributeDescription* attrs)
    {
        attrs[0] = Attribute(0, Format.R32G32B32Sfloat, 0);
        attrs[1] = Attribute(1, Format.R32G32B32Sfloat, 12);
        attrs[2] = Attribute(2, Format.R32G32Sfloat, 24);
    }

    static VertexInputAttributeDescription Attribute(uint location, Format format, uint offset) =>
        new() { Binding = 0, Location = location, Format = format, Offset = offset };

    internal static PipelineVertexInputStateCreateInfo Create(
        VertexInputBindingDescription* binding,
        VertexInputAttributeDescription* attrs) =>
        new()
        {
            SType = StructureType.PipelineVertexInputStateCreateInfo,
            VertexBindingDescriptionCount = 1,
            PVertexBindingDescriptions = binding,
            VertexAttributeDescriptionCount = 3,
            PVertexAttributeDescriptions = attrs
        };
}
