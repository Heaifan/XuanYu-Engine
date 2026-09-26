using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.Label;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    VulkanMapLabelTextureCache? _mapLabelTextures;
    Silk.NET.Vulkan.Pipeline _mapLabelPipeline;
    PipelineLayout _mapLabelPipelineLayout;

    internal DescriptorSetLayout MapLabelDescriptorSetLayout => _mapLabelTextures?.DescriptorSetLayout ?? default;

    internal void InitializeMapLabelTextures() =>
        _mapLabelTextures ??= new VulkanMapLabelTextureCache(_vk, _deviceOwner, _commandPool, _log);

    public void SetMapLabelPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _mapLabelPipeline = pipeline; _mapLabelPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Map Label 管线注入后 CommandBuffer 重录失败");
    }

    internal void DisposeMapLabels()
    {
        _mapLabelTextures?.Dispose(); _mapLabelTextures = null;
    }
}
