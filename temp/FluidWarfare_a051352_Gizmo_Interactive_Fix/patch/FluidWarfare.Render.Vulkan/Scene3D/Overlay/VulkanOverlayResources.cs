using Silk.NET.Vulkan;
using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>
/// Overlay 全部 Vulkan 资源。
/// Session 启动创建；Swapchain Resize 时随 RenderPass/Viewport 事务式重建；Session Dispose 释放。
/// </summary>
public sealed unsafe class VulkanOverlayResources : IDisposable
{
    private readonly Vk _vk;
    private readonly Silk.NET.Vulkan.Device _device;
    private readonly uint _vertexCapacity;

    public ShaderModule VertModule { get; private set; }
    public ShaderModule FragModule { get; private set; }
    public PipelineLayout Layout { get; private set; }
    public Pipeline Pipeline { get; private set; }
    public Silk.NET.Vulkan.Buffer VertexBuffer { get; private set; }
    public DeviceMemory VertexMemory { get; private set; }
    public uint VertexCapacity => _vertexCapacity;
    public bool IsValid { get; private set; }

    // ─── 创建 ───────────────────────────────────────────────────

    public static VulkanOverlayResources? TryCreate(
        Vk vk, Silk.NET.Vulkan.Device device,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        RenderPass renderPass,
        uint viewportWidth, uint viewportHeight,
        out string? error)
    {
        error = null;
        ShaderModule vertMod = default, fragMod = default;
        PipelineLayout layout = default;
        Pipeline pipeline = default;
        Silk.NET.Vulkan.Buffer buffer = default;
        DeviceMemory memory = default;

        // Shader modules
        if (!CreateShaderModules(vk, device, out vertMod, out fragMod, out error))
            return null;

        // Pipeline layout
        if (!VulkanOverlayPipelineLayout.Create(vk, device, physicalDevice, out layout, out error))
        { DestroyShaderModules(vk, device, vertMod, fragMod); return null; }

        // Pipeline
        if (!VulkanOverlayPipeline.Create(vk, device, renderPass, layout,
                vertMod, fragMod, viewportWidth, viewportHeight,
                out pipeline, out error))
        { DestroyLayoutAndShaders(vk, device, layout, vertMod, fragMod); return null; }

        // Vertex buffer (fixed capacity)
        const uint capacity = (uint)VulkanNavigationOverlayGeometry.MaxVertexCapacity;
        if (!CreateVertexBuffer(vk, device, physicalDevice, capacity,
                out buffer, out memory, out error))
        { DestroyAll(vk, device, pipeline, layout, vertMod, fragMod, buffer, memory); return null; }

        return new VulkanOverlayResources(vk, device, vertMod, fragMod, layout, pipeline, buffer, memory, capacity);
    }

    private VulkanOverlayResources(Vk vk, Silk.NET.Vulkan.Device device,
        ShaderModule vertMod, ShaderModule fragMod,
        PipelineLayout layout, Pipeline pipeline,
        Silk.NET.Vulkan.Buffer buffer, DeviceMemory memory,
        uint capacity)
    {
        _vk = vk;
        _device = device;
        VertModule = vertMod;
        FragModule = fragMod;
        Layout = layout;
        Pipeline = pipeline;
        VertexBuffer = buffer;
        VertexMemory = memory;
        _vertexCapacity = capacity;
        IsValid = true;
    }

    // ─── 上传顶点数据 ───────────────────────────────────────────

    public bool UploadVertices(VulkanOverlayVertex[] vertices, out string? error)
    {
        error = null;
        if (vertices.Length > _vertexCapacity)
        {
            error = $"Overlay 顶点数 {vertices.Length} 超过容量 {_vertexCapacity}。";
            return false;
        }

        var data = VulkanOverlayVertex.ToInterleaved(vertices);
        var byteSize = (ulong)(data.Length * sizeof(float));
        void* mapped;
        var mapResult = _vk.MapMemory(_device, VertexMemory, 0, byteSize, 0, &mapped);
        if (mapResult != Result.Success)
        {
            error = "Overlay Memory 映射失败。";
            return false;
        }
        var floatPtr = (float*)mapped;
        for (var i = 0; i < data.Length; i++)
            floatPtr[i] = data[i];
        _vk.UnmapMemory(_device, VertexMemory);
        return true;
    }

    // ─── 内部 ───────────────────────────────────────────────────

    private static bool CreateShaderModules(Vk vk, Silk.NET.Vulkan.Device dev,
        out ShaderModule vert, out ShaderModule frag, out string? error)
    {
        vert = default; frag = default; error = null;

        fixed (uint* pVert = CompiledShaders.OverlayVert)
        {
            var ci = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (uint)CompiledShaders.OverlayVert.Length * 4,
                PCode = pVert
            };
            if (vk.CreateShaderModule(dev, &ci, null, out vert) != Result.Success)
            { error = "Overlay Vertex ShaderModule 创建失败。"; return false; }
        }

        fixed (uint* pFrag = CompiledShaders.OverlayFrag)
        {
            var ci = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (uint)CompiledShaders.OverlayFrag.Length * 4,
                PCode = pFrag
            };
            if (vk.CreateShaderModule(dev, &ci, null, out frag) != Result.Success)
            { vk.DestroyShaderModule(dev, vert, null); error = "Overlay Frag ShaderModule 创建失败。"; return false; }
        }
        return true;
    }

    private static bool CreateVertexBuffer(Vk vk, Silk.NET.Vulkan.Device dev,
        Silk.NET.Vulkan.PhysicalDevice physicalDevice,
        uint vertexCapacity, out Silk.NET.Vulkan.Buffer buffer,
        out DeviceMemory memory, out string? error)
    {
        buffer = default; memory = default; error = null;
        var size = (ulong)vertexCapacity * VulkanOverlayPipeline.VertexStride;

        var bufCI = new BufferCreateInfo
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = BufferUsageFlags.VertexBufferBit | BufferUsageFlags.TransferDstBit,
            SharingMode = SharingMode.Exclusive
        };
        if (vk.CreateBuffer(dev, &bufCI, null, out buffer) != Result.Success)
        { error = "Overlay VertexBuffer 创建失败。"; return false; }

        vk.GetBufferMemoryRequirements(dev, buffer, out var memReq);
        PhysicalDeviceMemoryProperties memProps;
        vk.GetPhysicalDeviceMemoryProperties(physicalDevice, &memProps);
        var memType = FindMemoryType(memProps, memReq.MemoryTypeBits,
            MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        var allocInfo = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memReq.Size,
            MemoryTypeIndex = memType
        };
        if (vk.AllocateMemory(dev, &allocInfo, null, out memory) != Result.Success)
        { vk.DestroyBuffer(dev, buffer, null); error = "Overlay Memory 分配失败。"; return false; }

        vk.BindBufferMemory(dev, buffer, memory, 0);
        return true;
    }

    private static uint FindMemoryType(PhysicalDeviceMemoryProperties memProps, uint typeFilter, MemoryPropertyFlags props)
    {
        for (var i = 0; i < (int)memProps.MemoryTypeCount; i++)
        {
            if ((typeFilter & (1u << i)) != 0 &&
                (memProps.MemoryTypes[i].PropertyFlags & props) == props)
                return (uint)i;
        }
        return 0;
    }

    private static void DestroyShaderModules(Vk vk, Silk.NET.Vulkan.Device dev,
        ShaderModule v, ShaderModule f)
    {
        if (v.Handle != 0) vk.DestroyShaderModule(dev, v, null);
        if (f.Handle != 0) vk.DestroyShaderModule(dev, f, null);
    }

    private static void DestroyLayoutAndShaders(Vk vk, Silk.NET.Vulkan.Device dev,
        PipelineLayout l, ShaderModule v, ShaderModule f)
    {
        if (l.Handle != 0) vk.DestroyPipelineLayout(dev, l, null);
        DestroyShaderModules(vk, dev, v, f);
    }

    private static void DestroyAll(Vk vk, Silk.NET.Vulkan.Device dev,
        Pipeline p, PipelineLayout l, ShaderModule v, ShaderModule f,
        Silk.NET.Vulkan.Buffer buf, DeviceMemory mem)
    {
        if (p.Handle != 0) vk.DestroyPipeline(dev, p, null);
        if (l.Handle != 0) vk.DestroyPipelineLayout(dev, l, null);
        DestroyShaderModules(vk, dev, v, f);
        if (buf.Handle != 0) vk.DestroyBuffer(dev, buf, null);
        if (mem.Handle != 0) vk.FreeMemory(dev, mem, null);
    }

    // ─── 释放 ───────────────────────────────────────────────────

    public void Dispose()
    {
        if (!IsValid) return;

        if (Pipeline.Handle != 0) _vk.DestroyPipeline(_device, Pipeline, null);
        if (Layout.Handle != 0) _vk.DestroyPipelineLayout(_device, Layout, null);
        if (FragModule.Handle != 0) _vk.DestroyShaderModule(_device, FragModule, null);
        if (VertModule.Handle != 0) _vk.DestroyShaderModule(_device, VertModule, null);
        if (VertexBuffer.Handle != 0) _vk.DestroyBuffer(_device, VertexBuffer, null);
        if (VertexMemory.Handle != 0) _vk.FreeMemory(_device, VertexMemory, null);

        Pipeline = default;
        Layout = default;
        FragModule = default;
        VertModule = default;
        VertexBuffer = default;
        VertexMemory = default;
        IsValid = false;
    }
}
