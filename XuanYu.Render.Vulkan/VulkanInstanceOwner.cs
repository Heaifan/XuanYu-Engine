using System;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan;

// VK3-B1：Vulkan Instance 持有者。仅创建/释放 Instance，启用 VK_KHR_surface 与 VK_KHR_win32_surface。
// 禁止：Surface / PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。
public sealed unsafe class VulkanInstanceOwner : IDisposable
{
    readonly Vk _vk;
    Instance _instance;
    bool _disposed;

    VulkanInstanceOwner(Vk vk, Instance instance) { _vk = vk; _instance = instance; }

    public Instance Instance => _instance;

    public static VulkanInstanceOwner Create()
    {
        var r = CreateWithResult();
        if (!r.Success) throw new InvalidOperationException(
            VulkanInstanceLogFormatter.CreateFailed(r.ErrorType, r.ErrorMessage));
        return r.Owner!;
    }

    public static VulkanInstanceResult CreateWithResult()
    {
        Vk vk;
        try { vk = Vk.GetApi(); }
        catch (Exception ex)
        {
            Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed(ex.GetType().Name, ex.Message));
            return new VulkanInstanceResult(false, null, 0, ex.GetType().Name, ex.Message);
        }
        try
        {
            Instance created = default; Result createResult = Result.Success;
            VulkanInstanceCreateInfoBuilder.BuildAndUse(ci => createResult = vk.CreateInstance(&ci, null, out created));
            if (createResult != Result.Success)
            {
                vk.Dispose();
                Console.WriteLine(VulkanInstanceLogFormatter.CreateFailed("VkResult", createResult.ToString()));
                return new VulkanInstanceResult(false, null, 0, "VkResult", createResult.ToString());
            }
            Console.WriteLine(VulkanInstanceLogFormatter.Created(VulkanInstanceCreateInfoBuilder.ApiVersion));
            return new VulkanInstanceResult(true, new VulkanInstanceOwner(vk, created), VulkanInstanceCreateInfoBuilder.ApiVersion);
        }
        catch (Exception ex)
        {
            vk.Dispose();
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
        _vk.Dispose();
        _instance = default;
        Console.WriteLine(VulkanInstanceLogFormatter.Disposed(handle));
    }
}
