using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;
using VkDevice = global::Silk.NET.Vulkan.Device;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanStaticModelBufferMapFailureDeviceTests
{
    [Fact]
    public void Map_failure_frees_actual_buffer_then_bound_memory_once()
    {
        using var vk = Vk.GetApi();
        using var instance = VulkanInstanceOwner.CreateOffscreen(vk);
        var selection = VulkanPhysicalDeviceSelector.SelectOffscreen(vk, instance.Instance);
        Assert.True(selection.Success, selection.Message);
        using var device = VulkanDeviceOwner.CreateOffscreen(vk, selection);
        Assert.NotNull(device);
        var calls = new List<(string Name, ulong Handle)>();
        var mapAttempts = 0;
        var injected = new VulkanStaticModelBuffer.Calls(
            (Vk mapVk, VkDevice logicalDevice, DeviceMemory memory, ulong size, out nint data) =>
                { mapAttempts++; data = 0; return Result.ErrorMemoryMapFailed; },
            (api, logical, buffer) =>
                { calls.Add(("DestroyBuffer", buffer.Handle)); VulkanStaticModelBufferMapFailureCalls.DestroyBuffer(api, logical, buffer); },
            (api, logical, memory) =>
                { calls.Add(("FreeMemory", memory.Handle)); VulkanStaticModelBufferMapFailureCalls.FreeMemory(api, logical, memory); });

        var result = VulkanStaticModelBuffer.Create(vk, device!, new uint[] { 1, 2, 3 },
            BufferUsageFlags.VertexBufferBit, out var error, injected);

        Assert.Null(result);
        Assert.Equal("MapMemory", error);
        Assert.Equal(1, mapAttempts);
        Assert.Equal(new[] { "DestroyBuffer", "FreeMemory" }, calls.Select(x => x.Name));
        Assert.All(calls, call => Assert.NotEqual(0ul, call.Handle));
        Assert.Equal(2, calls.Select(x => x.Handle).Distinct().Count());
    }
}
