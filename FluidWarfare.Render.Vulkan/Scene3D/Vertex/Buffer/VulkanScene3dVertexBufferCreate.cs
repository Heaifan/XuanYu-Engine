using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 单个 Vertex Buffer 创建：CreateBuffer → AllocateMemory → Bind → Map → Copy → Unmap。
/// </summary>
public static unsafe partial class VulkanScene3dVertexBuffers
{
    internal static bool CreateOneBuffer(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev, float[] data,
        out Silk.NET.Vulkan.Buffer buffer, out DeviceMemory memory,
        out string errorMessage)
    {
        buffer = default; memory = default; errorMessage = string.Empty;
        var size = (nuint)(data.Length * sizeof(float));

        var bCI = new BufferCreateInfo { SType = StructureType.BufferCreateInfo, Size = size, Usage = BufferUsageFlags.VertexBufferBit, SharingMode = SharingMode.Exclusive };
        if (vk.CreateBuffer(dev, &bCI, null, out buffer) != Result.Success)
        { errorMessage = "vkCreateBuffer 失败。"; return false; }

        vk.GetBufferMemoryRequirements(dev, buffer, out var memReqs);
        var memTypeIndex = FindMemoryType(vk, pd, memReqs.MemoryTypeBits,
            MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        if (memTypeIndex == uint.MaxValue)
        { errorMessage = "无法找到 HostVisible | HostCoherent 内存类型。"; vk.DestroyBuffer(dev, buffer, null); buffer = default; return false; }

        var allocCI = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = memReqs.Size, MemoryTypeIndex = memTypeIndex };
        if (vk.AllocateMemory(dev, &allocCI, null, out memory) != Result.Success)
        { errorMessage = "vkAllocateMemory 失败。"; vk.DestroyBuffer(dev, buffer, null); buffer = default; return false; }

        if (vk.BindBufferMemory(dev, buffer, memory, 0) != Result.Success)
        { errorMessage = "vkBindBufferMemory 失败。"; vk.DestroyBuffer(dev, buffer, null); vk.FreeMemory(dev, memory, null); memory = default; return false; }

        void* mapped;
        if (vk.MapMemory(dev, memory, 0, size, 0, &mapped) != Result.Success)
        { errorMessage = "vkMapMemory 失败。"; vk.DestroyBuffer(dev, buffer, null); vk.FreeMemory(dev, memory, null); memory = default; return false; }

        fixed (float* src = data) { System.Buffer.MemoryCopy(src, mapped, (long)size, (long)size); }
        vk.UnmapMemory(dev, memory);
        return true;
    }

    private static uint FindMemoryType(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        uint typeBits, MemoryPropertyFlags props)
    {
        vk.GetPhysicalDeviceMemoryProperties(pd, out var memProps);
        for (var i = 0u; i < memProps.MemoryTypeCount; i++)
            if ((typeBits & (1u << (int)i)) != 0 && (memProps.MemoryTypes[(int)i].PropertyFlags & props) == props)
                return i;
        return uint.MaxValue;
    }
}
