using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using VkBuffer = Silk.NET.Vulkan.Buffer;

namespace XuanYu.Render.Vulkan.Render.Readback;

sealed unsafe class VulkanReadbackBuffer : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _device;
    readonly ulong _size;
    VkBuffer _buffer;
    DeviceMemory _memory;

    VulkanReadbackBuffer(Vk vk, VulkanDeviceOwner device, ulong size, VkBuffer buffer, DeviceMemory memory)
        => (_vk, _device, _size, _buffer, _memory) = (vk, device, size, buffer, memory);

    public static VulkanReadbackBuffer? Create(Vk vk, VulkanDeviceOwner device, ulong size)
    {
        var info = new BufferCreateInfo { SType = StructureType.BufferCreateInfo,
            Size = size, Usage = BufferUsageFlags.TransferDstBit, SharingMode = SharingMode.Exclusive };
        if (vk.CreateBuffer(device.LogicalDevice, &info, null, out var buffer) != Result.Success) return null;
        vk.GetBufferMemoryRequirements(device.LogicalDevice, buffer, out var req);
        vk.GetPhysicalDeviceMemoryProperties(device.PhysicalDevice, out var props);
        var type = FindHostCoherentType(props, req.MemoryTypeBits);
        if (type < 0) { vk.DestroyBuffer(device.LogicalDevice, buffer, null); return null; }
        var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
            AllocationSize = req.Size, MemoryTypeIndex = (uint)type };
        if (vk.AllocateMemory(device.LogicalDevice, &allocate, null, out var memory) != Result.Success)
        { vk.DestroyBuffer(device.LogicalDevice, buffer, null); return null; }
        if (vk.BindBufferMemory(device.LogicalDevice, buffer, memory, 0) != Result.Success)
        { vk.FreeMemory(device.LogicalDevice, memory, null); vk.DestroyBuffer(device.LogicalDevice, buffer, null); return null; }
        return new VulkanReadbackBuffer(vk, device, size, buffer, memory);
    }

    static int FindHostCoherentType(PhysicalDeviceMemoryProperties props, uint bits)
    {
        for (var i = 0; i < props.MemoryTypeCount; i++)
            if ((bits & (1u << i)) != 0 &&
                (props.MemoryTypes[i].PropertyFlags &
                    (MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit)) ==
                (MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit)) return i;
        return -1;
    }

    public byte[] Read()
    {
        if (_size > int.MaxValue) throw new InvalidOperationException("Vulkan readback exceeds managed array size");
        void* data;
        if (_vk.MapMemory(_device.LogicalDevice, _memory, 0, _size, 0, &data) != Result.Success)
            throw new InvalidOperationException("Vulkan readback MapMemory failed");
        try { return new ReadOnlySpan<byte>(data, (int)_size).ToArray(); }
        finally { _vk.UnmapMemory(_device.LogicalDevice, _memory); }
    }

    public VkBuffer Buffer => _buffer;

    public void Dispose()
    {
        if (_buffer.Handle != 0) _vk.DestroyBuffer(_device.LogicalDevice, _buffer, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
        _buffer = default; _memory = default;
    }
}
