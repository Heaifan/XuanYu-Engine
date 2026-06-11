using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// 创建并上传 Grid / Unit 顶点 Buffer。
/// Host Visible + Host Coherent，不做 staging buffer。
/// </summary>
public static unsafe class VulkanScene3dVertexBuffers
{
    /// <summary>
    /// 创建 Grid 和 Unit 顶点 Buffer。
    /// </summary>
    public static bool Create(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        out Silk.NET.Vulkan.Buffer gridBuffer, out DeviceMemory gridMemory,
        out Silk.NET.Vulkan.Buffer unitBuffer, out DeviceMemory unitMemory,
        out int gridVertexCount, out int unitVertexCount,
        out string errorMessage)
    {
        gridBuffer = default; gridMemory = default;
        unitBuffer = default; unitMemory = default;
        gridVertexCount = gridVertices.Length;
        unitVertexCount = unitVertices.Length;
        errorMessage = string.Empty;

        if (gridVertexCount == 0)
        {
            errorMessage = "Scene3D VertexBuffer：Grid vertex count 为 0。";
            return false;
        }
        if (unitVertexCount == 0)
        {
            errorMessage = "Scene3D VertexBuffer：Unit vertex count 为 0。";
            return false;
        }

        var gridData = VulkanScene3dVertices.ToInterleaved(gridVertices);
        var unitData = VulkanScene3dVertices.ToInterleaved(unitVertices);

        if (!CreateOne(vk, pd, dev, gridData, out gridBuffer, out gridMemory, out var gridErr))
        {
            errorMessage = $"Scene3D Grid VertexBuffer：{gridErr}";
            return false;
        }

        if (!CreateOne(vk, pd, dev, unitData, out unitBuffer, out unitMemory, out var unitErr))
        {
            vk.DestroyBuffer(dev, gridBuffer, null);
            vk.FreeMemory(dev, gridMemory, null);
            gridBuffer = default; gridMemory = default;
            errorMessage = $"Scene3D Unit VertexBuffer：{unitErr}";
            return false;
        }

        return true;
    }

    private static bool CreateOne(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev, float[] data,
        out Silk.NET.Vulkan.Buffer buffer, out DeviceMemory memory,
        out string errorMessage)
    {
        buffer = default; memory = default; errorMessage = string.Empty;
        var size = (nuint)(data.Length * sizeof(float));

        var bCI = new BufferCreateInfo
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = BufferUsageFlags.VertexBufferBit,
            SharingMode = SharingMode.Exclusive
        };
        if (vk.CreateBuffer(dev, &bCI, null, out buffer) != Result.Success)
        {
            errorMessage = "vkCreateBuffer 失败。";
            return false;
        }

        vk.GetBufferMemoryRequirements(dev, buffer, out var memReqs);
        var memTypeIndex = FindMemoryType(vk, pd, memReqs.MemoryTypeBits,
            MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        if (memTypeIndex == uint.MaxValue)
        {
            errorMessage = "无法找到 HostVisible | HostCoherent 内存类型。";
            vk.DestroyBuffer(dev, buffer, null); buffer = default;
            return false;
        }

        var allocCI = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memReqs.Size,
            MemoryTypeIndex = memTypeIndex
        };
        if (vk.AllocateMemory(dev, &allocCI, null, out memory) != Result.Success)
        {
            errorMessage = "vkAllocateMemory 失败。";
            vk.DestroyBuffer(dev, buffer, null); buffer = default;
            return false;
        }

        if (vk.BindBufferMemory(dev, buffer, memory, 0) != Result.Success)
        {
            errorMessage = "vkBindBufferMemory 失败。";
            vk.DestroyBuffer(dev, buffer, null);
            vk.FreeMemory(dev, memory, null); memory = default;
            return false;
        }

        void* mapped;
        if (vk.MapMemory(dev, memory, 0, size, 0, &mapped) != Result.Success)
        {
            errorMessage = "vkMapMemory 失败。";
            vk.DestroyBuffer(dev, buffer, null);
            vk.FreeMemory(dev, memory, null); memory = default;
            return false;
        }

        fixed (float* src = data)
        {
            System.Buffer.MemoryCopy(src, mapped, (long)size, (long)size);
        }
        vk.UnmapMemory(dev, memory);
        return true;
    }

    private static uint FindMemoryType(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        uint typeBits, MemoryPropertyFlags props)
    {
        vk.GetPhysicalDeviceMemoryProperties(pd, out var memProps);
        for (var i = 0u; i < memProps.MemoryTypeCount; i++)
        {
            if ((typeBits & (1u << (int)i)) != 0 &&
                (memProps.MemoryTypes[(int)i].PropertyFlags & props) == props)
                return i;
        }
        return uint.MaxValue;
    }
}
