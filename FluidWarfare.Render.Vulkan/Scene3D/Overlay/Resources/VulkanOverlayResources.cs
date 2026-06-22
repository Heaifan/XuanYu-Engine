using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay 全部 Vulkan 资源。Session 启动创建；Swapchain Resize 时重建；Dispose 释放。</summary>
public sealed unsafe partial class VulkanOverlayResources : IDisposable
{
    readonly Vk _vk;
    readonly Silk.NET.Vulkan.Device _device;
    readonly uint _vertexCapacity;

    public ShaderModule VertModule { get; private set; }
    public ShaderModule FragModule { get; private set; }
    public PipelineLayout Layout { get; private set; }
    public Pipeline Pipeline { get; private set; }
    public Silk.NET.Vulkan.Buffer VertexBuffer { get; private set; }
    public DeviceMemory VertexMemory { get; private set; }
    public uint VertexCapacity => _vertexCapacity;
    public bool IsValid { get; private set; }

    VulkanOverlayResources(Vk vk, Silk.NET.Vulkan.Device device, ShaderModule vertMod, ShaderModule fragMod,
        PipelineLayout layout, Pipeline pipeline, Silk.NET.Vulkan.Buffer buffer, DeviceMemory memory, uint capacity)
    { _vk = vk; _device = device; VertModule = vertMod; FragModule = fragMod; Layout = layout; Pipeline = pipeline; VertexBuffer = buffer; VertexMemory = memory; _vertexCapacity = capacity; IsValid = true; }

    public static VulkanOverlayResources? TryCreate(Vk vk, Silk.NET.Vulkan.Device device,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice, RenderPass renderPass,
        uint viewportWidth, uint viewportHeight, out string? error)
    {
        error = null;
        ShaderModule v = default, f = default; PipelineLayout l = default; Pipeline p = default;
        Silk.NET.Vulkan.Buffer buf = default; DeviceMemory mem = default;
        if (!CreateShaderModules(vk, device, out v, out f, out error)) return null;
        if (!VulkanOverlayPipelineLayout.Create(vk, device, physicalDevice, out l, out error))
        { DestroyShaderModules(vk, device, v, f); return null; }
        if (!VulkanOverlayPipeline.Create(vk, device, renderPass, l, v, f, viewportWidth, viewportHeight, out p, out error))
        { DestroyLayoutAndShaders(vk, device, l, v, f); return null; }
        const uint cap = (uint)VulkanNavigationOverlayGeometry.MaxVertexCapacity;
        if (!CreateVertexBuffer(vk, device, physicalDevice, cap, out buf, out mem, out error))
        { DestroyAll(vk, device, p, l, v, f, buf, mem); return null; }
        return new VulkanOverlayResources(vk, device, v, f, l, p, buf, mem, cap);
    }

    public bool UploadVertices(VulkanOverlayVertex[] vertices, out string? error)
    {
        error = null;
        if (vertices.Length > _vertexCapacity) { error = $"Overlay 顶点数 {vertices.Length} 超过容量 {_vertexCapacity}。"; return false; }
        var data = VulkanOverlayVertex.ToInterleaved(vertices);
        void* mapped;
        if (_vk.MapMemory(_device, VertexMemory, 0, (ulong)(data.Length * sizeof(float)), 0, &mapped) != Result.Success)
        { error = "Overlay Memory 映射失败。"; return false; }
        var fp = (float*)mapped; for (var i = 0; i < data.Length; i++) fp[i] = data[i];
        _vk.UnmapMemory(_device, VertexMemory); return true;
    }

    public void Dispose()
    {
        if (!IsValid) return;
        if (Pipeline.Handle != 0) _vk.DestroyPipeline(_device, Pipeline, null);
        if (Layout.Handle != 0) _vk.DestroyPipelineLayout(_device, Layout, null);
        if (FragModule.Handle != 0) _vk.DestroyShaderModule(_device, FragModule, null);
        if (VertModule.Handle != 0) _vk.DestroyShaderModule(_device, VertModule, null);
        if (VertexBuffer.Handle != 0) _vk.DestroyBuffer(_device, VertexBuffer, null);
        if (VertexMemory.Handle != 0) _vk.FreeMemory(_device, VertexMemory, null);
        Pipeline = default; Layout = default; FragModule = default; VertModule = default;
        VertexBuffer = default; VertexMemory = default; IsValid = false;
    }
}
