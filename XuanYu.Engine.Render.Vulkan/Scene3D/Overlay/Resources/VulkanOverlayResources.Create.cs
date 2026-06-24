using Silk.NET.Vulkan;
using XuanYu.Engine.Render.Vulkan.Shaders;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay 资源创建静态辅助方法。</summary>
sealed unsafe partial class VulkanOverlayResources
{
    static bool CreateShaderModules(Vk vk, Silk.NET.Vulkan.Device dev, out ShaderModule vert, out ShaderModule frag, out string? error)
    {
        vert = default; frag = default; error = null;
        fixed (uint* p = CompiledShaders.OverlayVert)
        { var ci = new ShaderModuleCreateInfo { SType = StructureType.ShaderModuleCreateInfo, CodeSize = (uint)CompiledShaders.OverlayVert.Length * 4, PCode = p }; if (vk.CreateShaderModule(dev, &ci, null, out vert) != Result.Success) { error = "Overlay Vert 创建失败。"; return false; } }
        fixed (uint* p = CompiledShaders.OverlayFrag)
        { var ci = new ShaderModuleCreateInfo { SType = StructureType.ShaderModuleCreateInfo, CodeSize = (uint)CompiledShaders.OverlayFrag.Length * 4, PCode = p }; if (vk.CreateShaderModule(dev, &ci, null, out frag) != Result.Success) { vk.DestroyShaderModule(dev, vert, null); error = "Overlay Frag 创建失败。"; return false; } }
        return true;
    }

    static bool CreateVertexBuffer(Vk vk, Silk.NET.Vulkan.Device dev, Silk.NET.Vulkan.PhysicalDevice pd, uint cap, out Silk.NET.Vulkan.Buffer buf, out DeviceMemory mem, out string? error)
    {
        buf = default; mem = default; error = null;
        var sz = (ulong)cap * VulkanOverlayPipeline.VertexStride;
        var ci = new BufferCreateInfo { SType = StructureType.BufferCreateInfo, Size = sz, Usage = BufferUsageFlags.VertexBufferBit | BufferUsageFlags.TransferDstBit, SharingMode = SharingMode.Exclusive };
        if (vk.CreateBuffer(dev, &ci, null, out buf) != Result.Success) { error = "Overlay Buffer 创建失败。"; return false; }
        vk.GetBufferMemoryRequirements(dev, buf, out var mr); PhysicalDeviceMemoryProperties mp; vk.GetPhysicalDeviceMemoryProperties(pd, &mp);
        var mt = FindMemoryType(mp, mr.MemoryTypeBits, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        var ai = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = mr.Size, MemoryTypeIndex = mt };
        if (vk.AllocateMemory(dev, &ai, null, out mem) != Result.Success) { vk.DestroyBuffer(dev, buf, null); error = "Overlay Memory 分配失败。"; return false; }
        vk.BindBufferMemory(dev, buf, mem, 0); return true;
    }

    static uint FindMemoryType(PhysicalDeviceMemoryProperties mp, uint tf, MemoryPropertyFlags pf)
    { for (var i = 0; i < (int)mp.MemoryTypeCount; i++) if ((tf & (1u << i)) != 0 && (mp.MemoryTypes[i].PropertyFlags & pf) == pf) return (uint)i; return 0; }

    static void DestroyShaderModules(Vk vk, Silk.NET.Vulkan.Device dev, ShaderModule v, ShaderModule f)
    { if (v.Handle != 0) vk.DestroyShaderModule(dev, v, null); if (f.Handle != 0) vk.DestroyShaderModule(dev, f, null); }

    static void DestroyLayoutAndShaders(Vk vk, Silk.NET.Vulkan.Device dev, PipelineLayout l, ShaderModule v, ShaderModule f)
    { if (l.Handle != 0) vk.DestroyPipelineLayout(dev, l, null); DestroyShaderModules(vk, dev, v, f); }

    static void DestroyAll(Vk vk, Silk.NET.Vulkan.Device dev, Pipeline p, PipelineLayout l, ShaderModule v, ShaderModule f, Silk.NET.Vulkan.Buffer buf, DeviceMemory mem)
    { if (p.Handle != 0) vk.DestroyPipeline(dev, p, null); if (l.Handle != 0) vk.DestroyPipelineLayout(dev, l, null); DestroyShaderModules(vk, dev, v, f); if (buf.Handle != 0) vk.DestroyBuffer(dev, buf, null); if (mem.Handle != 0) vk.FreeMemory(dev, mem, null); }
}
