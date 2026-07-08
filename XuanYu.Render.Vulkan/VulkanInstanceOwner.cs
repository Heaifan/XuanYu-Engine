using System;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

// VK3-B1 / C1-R2：Vulkan Instance 持有者。仅创建/释放 Instance，启用 VK_KHR_surface 与 VK_KHR_win32_surface。
// Vk 由调用方（Bridge）统一持有与释放，本类只使用传入的 Vk，不在 Dispose 中释放 Vk。
// 禁止：Surface / PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed unsafe class VulkanInstanceOwner : IDisposable
{
    readonly Vk _vk;
    Instance _instance;
    bool _disposed;

    VulkanInstanceOwner(Vk vk, Instance instance) { _vk = vk; _instance = instance; }

    public Instance Instance => _instance;

    public static VulkanInstanceOwner Create(Vk vk)
    {
        var r = CreateWithResult(vk);
        if (!r.Success) throw new InvalidOperationException(
            VulkanInstanceLogFormatter.CreateFailed(r.ErrorType, r.ErrorMessage));
        return r.Owner!;
    }

    public static VulkanInstanceResult CreateWithResult(Vk vk)
    {
        try
        {
            Instance created = default; Result createResult = Result.Success;
            VulkanInstanceCreateInfoBuilder.BuildAndUse(ci => createResult = vk.CreateInstance(&ci, null, out created));
            if (createResult != Result.Success)
            {
                Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed("VkResult", createResult.ToString()));
                return new VulkanInstanceResult(false, null, 0, "VkResult", createResult.ToString());
            }
            Console.WriteLine(VulkanInstanceLogFormatter.Created(VulkanInstanceCreateInfoBuilder.ApiVersion));
            return new VulkanInstanceResult(true, new VulkanInstanceOwner(vk, created), VulkanInstanceCreateInfoBuilder.ApiVersion);
        }
        catch (Exception ex)
        {
            Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed(ex.GetType().Name, ex.Message));
            return new VulkanInstanceResult(false, null, 0, ex.GetType().Name, ex.Message);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        var handle = _instance.Handle;
        if (handle != 0) _vk.DestroyInstance(_instance, null);
        _instance = default;
        Console.WriteLine(VulkanInstanceLogFormatter.Disposed(handle));
    }
}
