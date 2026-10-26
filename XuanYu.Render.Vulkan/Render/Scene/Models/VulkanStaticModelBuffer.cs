using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;
using VkBuffer = Silk.NET.Vulkan.Buffer;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

sealed unsafe class VulkanStaticModelBuffer : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device;
    VkBuffer _buffer; DeviceMemory _memory; readonly ulong _capacityBytes;
    VulkanStaticModelBuffer(Vk vk, VulkanDeviceOwner device, VkBuffer b, DeviceMemory m, ulong capacity) =>
        (_vk, _device, _buffer, _memory, _capacityBytes) = (vk, device, b, m, capacity);
    public VkBuffer Buffer => _buffer;
    public ulong CapacityBytes => _capacityBytes;

    public static VulkanStaticModelBuffer? Create<T>(Vk vk, VulkanDeviceOwner device,
        T[] data, BufferUsageFlags usage, out string error) where T : unmanaged
    {
        error = "";
        if (data.Length == 0) { error = "empty data"; return null; }
        var bytes = MemoryMarshal.AsBytes(data.AsSpan());
        if (!CreateRaw(vk, device, (ulong)bytes.Length, usage, out var buffer, out error)) return null;
        if (!Allocate(vk, device, buffer, out var memory, out error))
        { vk.DestroyBuffer(device.LogicalDevice, buffer, null); return null; }
        if (vk.BindBufferMemory(device.LogicalDevice, buffer, memory, 0) != Result.Success)
        { vk.FreeMemory(device.LogicalDevice, memory, null); vk.DestroyBuffer(device.LogicalDevice, buffer, null); error = "BindBufferMemory"; return null; }
        void* dst;
        if (vk.MapMemory(device.LogicalDevice, memory, 0, (ulong)bytes.Length, 0, &dst) != Result.Success)
        { vk.FreeMemory(device.LogicalDevice, memory, null); vk.DestroyBuffer(device.LogicalDevice, buffer, null); error = "MapMemory"; return null; }
        bytes.CopyTo(new Span<byte>(dst, bytes.Length));
        vk.UnmapMemory(device.LogicalDevice, memory);
        return new VulkanStaticModelBuffer(vk, device, buffer, memory, (ulong)bytes.Length);
    }

    public bool TryUpdate<T>(T[] data) where T : unmanaged
    {
        var bytes = MemoryMarshal.AsBytes(data.AsSpan());
        if (bytes.Length == 0 || (ulong)bytes.Length > _capacityBytes) return false;
        void* dst;
        if (_vk.MapMemory(_device.LogicalDevice, _memory, 0, (ulong)bytes.Length, 0, &dst) != Result.Success)
            return false;
        bytes.CopyTo(new Span<byte>(dst, bytes.Length));
        _vk.UnmapMemory(_device.LogicalDevice, _memory);
        return true;
    }

    static bool CreateRaw(Vk vk, VulkanDeviceOwner d, ulong size, BufferUsageFlags usage,
        out VkBuffer buffer, out string error)
    {
        error = ""; buffer = default;
        var info = new BufferCreateInfo { SType = StructureType.BufferCreateInfo, Size = size, Usage = usage, SharingMode = SharingMode.Exclusive };
        var result = vk.CreateBuffer(d.LogicalDevice, &info, null, out buffer);
        if (result == Result.Success) return true;
        error = $"CreateBuffer {result}"; return false;
    }

    static bool Allocate(Vk vk, VulkanDeviceOwner d, VkBuffer buffer, out DeviceMemory mem, out string error)
    {
        mem = default; error = "";
        vk.GetBufferMemoryRequirements(d.LogicalDevice, buffer, out var req);
        var type = FindMemoryType(vk, d, req.MemoryTypeBits,
            MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        if (type < 0) { error = "memory type"; return false; }
        var info = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = req.Size, MemoryTypeIndex = (uint)type };
        var result = vk.AllocateMemory(d.LogicalDevice, &info, null, out mem);
        if (result == Result.Success) return true;
        error = $"AllocateMemory {result}"; return false;
    }

    static int FindMemoryType(Vk vk, VulkanDeviceOwner d, uint bits, MemoryPropertyFlags flags)
    {
        vk.GetPhysicalDeviceMemoryProperties(d.PhysicalDevice, out var props);
        for (var i = 0; i < props.MemoryTypeCount; i++)
            if ((bits & (1u << i)) != 0 && (props.MemoryTypes[i].PropertyFlags & flags) == flags) return i;
        return -1;
    }

    public void Dispose()
    {
        if (_buffer.Handle != 0) _vk.DestroyBuffer(_device.LogicalDevice, _buffer, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device.LogicalDevice, _memory, null);
        _buffer = default; _memory = default;
    }
}
