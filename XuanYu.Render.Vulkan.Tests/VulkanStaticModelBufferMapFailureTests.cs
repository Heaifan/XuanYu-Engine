using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;
using Xunit;
using VkDevice = global::Silk.NET.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanStaticModelBufferMapFailureTests
{
    [Fact]
    public void Real_buffer_and_memory_are_released_once_in_dependency_order()
    {
        using var vk = Vk.GetApi();
        using var instance = VulkanInstanceOwner.CreateOffscreen(vk);
        var selection = VulkanPhysicalDeviceSelector.SelectOffscreen(vk, instance.Instance);
        Assert.True(selection.Success, selection.Message);
        using var device = VulkanDeviceOwner.CreateOffscreen(vk, selection);
        Assert.NotNull(device);

        var released = new List<(string Name, ulong Handle)>();
        var mapCalls = 0;
        var calls = new VulkanStaticModelBuffer.Calls(
            (Vk api, VkDevice logical, DeviceMemory memory, ulong size, out nint data) =>
            {
                mapCalls++;
                data = 0;
                return Result.ErrorMemoryMapFailed;
            },
            (api, logical, buffer) =>
            {
                released.Add(("DestroyBuffer", buffer.Handle));
                VulkanStaticModelBufferMapFailureCalls.DestroyBuffer(api, logical, buffer);
            },
            (api, logical, memory) =>
            {
                released.Add(("FreeMemory", memory.Handle));
                VulkanStaticModelBufferMapFailureCalls.FreeMemory(api, logical, memory);
            });

        var result = VulkanStaticModelBuffer.Create(vk, device!, new uint[] { 1, 2, 3 },
            BufferUsageFlags.VertexBufferBit, out var error, calls);

        Assert.Null(result);
        Assert.Equal("MapMemory", error);
        Assert.Equal(1, mapCalls);
        Assert.Equal(new[] { "DestroyBuffer", "FreeMemory" },
            released.Select(item => item.Name));
        Assert.All(released, item => Assert.NotEqual(0ul, item.Handle));
        Assert.Equal(2, released.Select(item => item.Handle).Distinct().Count());
    }
}
