using XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Handles;

/// <summary>帧级资源句柄（Shader / Pipeline / Buffer / Overlay）。</summary>
sealed record VulkanScene3dFrameHandles(
    ShaderModule VertModule, ShaderModule FragModule,
    PipelineLayout PipelineLayout,
    Pipeline GridPipeline, Pipeline UnitPipeline,
    Silk.NET.Vulkan.Buffer GridBuf, DeviceMemory GridMemory, int GridVertexCount,
    Silk.NET.Vulkan.Buffer UnitBuf, DeviceMemory UnitMemory, int UnitVertexCount,
    Silk.NET.Vulkan.Buffer CursorBuf, DeviceMemory CursorMemory, int CursorVertexCount,
    VulkanOverlayResources? OverlayResources)
{
    public bool IsValid => VertModule.Handle != 0;

    public static readonly VulkanScene3dFrameHandles Empty = new(
        default, default, default, default, default,
        default, default, 0,
        default, default, 0,
        default, default, 0, null);
}
