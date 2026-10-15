using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Benchmark;

public sealed unsafe class VulkanGpuTimestampQuery : IDisposable
{
    readonly Vk _vk;
    readonly VulkanDeviceOwner _device;
    readonly QueryPool _pool;
    readonly uint _validBits;
    readonly float _periodNs;

    VulkanGpuTimestampQuery(Vk vk, VulkanDeviceOwner device, QueryPool pool, uint bits, float period)
        => (_vk, _device, _pool, _validBits, _periodNs) = (vk, device, pool, bits, period);

    public static VulkanGpuTimestampQuery? TryCreate(Vk vk, VulkanDeviceOwner device, uint family)
    {
        vk.GetPhysicalDeviceProperties(device.PhysicalDevice, out var properties);
        var count = 0u;
        vk.GetPhysicalDeviceQueueFamilyProperties(device.PhysicalDevice, &count, null);
        var queues = stackalloc QueueFamilyProperties[(int)count];
        vk.GetPhysicalDeviceQueueFamilyProperties(device.PhysicalDevice, &count, queues);
        if (family >= count || queues[family].TimestampValidBits == 0 || properties.Limits.TimestampPeriod <= 0)
            return null;
        var info = new QueryPoolCreateInfo { SType = StructureType.QueryPoolCreateInfo,
            QueryType = QueryType.Timestamp, QueryCount = 2 };
        return vk.CreateQueryPool(device.LogicalDevice, &info, null, out var pool) == Result.Success
            ? new VulkanGpuTimestampQuery(vk, device, pool, queues[family].TimestampValidBits,
                properties.Limits.TimestampPeriod) : null;
    }

    public uint ValidBits => _validBits;
    public double PeriodNanoseconds => _periodNs;

    public void RecordStart(CommandBuffer command)
    {
        _vk.CmdResetQueryPool(command, _pool, 0, 2);
        _vk.CmdWriteTimestamp(command, PipelineStageFlags.TopOfPipeBit, _pool, 0);
    }

    public void RecordEnd(CommandBuffer command) =>
        _vk.CmdWriteTimestamp(command, PipelineStageFlags.BottomOfPipeBit, _pool, 1);

    public bool TryReadMilliseconds(out double milliseconds)
    {
        milliseconds = 0;
        ulong* values = stackalloc ulong[4];
        var flags = QueryResultFlags.Result64Bit | QueryResultFlags.ResultWithAvailabilityBit;
        var result = _vk.GetQueryPoolResults(_device.LogicalDevice, _pool, 0, 2,
            (nuint)32, values, 16, flags);
        if (result != Result.Success || values[1] == 0 || values[3] == 0) return false;
        var mask = _validBits == 64 ? ulong.MaxValue : (1UL << (int)_validBits) - 1;
        milliseconds = ((values[2] - values[0]) & mask) * _periodNs / 1_000_000d;
        return double.IsFinite(milliseconds) && milliseconds >= 0;
    }

    public void Dispose() => _vk.DestroyQueryPool(_device.LogicalDevice, _pool, null);
}
