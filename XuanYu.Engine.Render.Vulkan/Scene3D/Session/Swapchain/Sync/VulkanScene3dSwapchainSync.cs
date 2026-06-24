using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain.Sync;

/// <summary>CommandPool / CommandBuffer / Semaphore / Fence 创建。</summary>
internal static unsafe class VulkanScene3dSwapchainSync
{
    public static CommandPool CreateCommandPool(Vk vk, Silk.NET.Vulkan.Device device, uint queueFamilyIndex)
    {
        var ci = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo, QueueFamilyIndex = queueFamilyIndex };
        CommandPool pool = default;
        vk.CreateCommandPool(device, &ci, null, out pool);
        return pool;
    }

    public static CommandBuffer AllocateCommandBuffer(Vk vk, Silk.NET.Vulkan.Device device, CommandPool pool)
    {
        var ci = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo, CommandPool = pool,
            Level = CommandBufferLevel.Primary, CommandBufferCount = 1
        };
        CommandBuffer cb = default;
        vk.AllocateCommandBuffers(device, &ci, out cb);
        return cb;
    }

    public static bool CreateSemaphore(Vk vk, Silk.NET.Vulkan.Device device,
        out Silk.NET.Vulkan.Semaphore semaphore, out string? error)
    {
        var ci = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        Silk.NET.Vulkan.Semaphore sem = default;
        var r = vk.CreateSemaphore(device, &ci, null, out sem);
        semaphore = sem;
        error = r == Result.Success ? null : $"Semaphore 创建失败：{r}。";
        return r == Result.Success;
    }

    public static bool CreateFence(Vk vk, Silk.NET.Vulkan.Device device,
        out Fence fence, out string? error)
    {
        var ci = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit };
        Fence f = default;
        var r = vk.CreateFence(device, &ci, null, out f);
        fence = f;
        error = r == Result.Success ? null : $"Fence 创建失败：{r}。";
        return r == Result.Success;
    }
}
