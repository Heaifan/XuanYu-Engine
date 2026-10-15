using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.Readback;
using XuanYu.Render.Vulkan.Render.StaticModels;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    internal VulkanClearFrameOwner(Vk vk, VulkanDeviceOwner deviceOwner,
        VulkanOffscreenColorTarget target, int graphicsFamily, Action<string>? log)
    {
        _vk = vk; _deviceOwner = deviceOwner; _offscreenTarget = target; _log = log;
        _staticModels = new VulkanStaticModelCache(vk, deviceOwner, log);
        _vectorOverlays = new VulkanVectorOverlayCache(vk, deviceOwner, log);
        _proceduralVertexBuffer = VulkanStaticModelBuffer.Create(vk, deviceOwner,
            new VulkanStaticModelVertex[RenderDrawPlan.RotateGizmoVertexCount],
            BufferUsageFlags.VertexBufferBit, out _);
        Initialize(graphicsFamily);
    }
}
