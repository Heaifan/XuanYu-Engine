using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe class VulkanGraphicsPipelineOwner : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _deviceOwner;
    readonly Action<string>? _log;
    PipelineLayout _layout;
    Silk.NET.Vulkan.Pipeline _pipeline;
    public Silk.NET.Vulkan.Pipeline Pipeline => _pipeline;
    public PipelineLayout Layout => _layout;
    internal VulkanGraphicsPipelineOwner(Vk vk, VulkanDeviceOwner deviceOwner, PipelineLayout layout, Silk.NET.Vulkan.Pipeline pipeline, Action<string>? log)
    {
        _vk = vk; _deviceOwner = deviceOwner; _layout = layout; _pipeline = pipeline; _log = log;
    }
    internal static ShaderStageFlags PushConstantStages(bool analyticStroke) => analyticStroke ? ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit : ShaderStageFlags.VertexBit;
    public void Dispose()
    {
        if (_pipeline.Handle == 0 && _layout.Handle == 0) return;
        if (_pipeline.Handle != 0) _vk.DestroyPipeline(_deviceOwner.LogicalDevice, _pipeline, null);
        if (_layout.Handle != 0) _vk.DestroyPipelineLayout(_deviceOwner.LogicalDevice, _layout, null);
        _pipeline = default; _layout = default; _log?.Invoke(PipelineLogFormatter.Disposed());
    }
}
