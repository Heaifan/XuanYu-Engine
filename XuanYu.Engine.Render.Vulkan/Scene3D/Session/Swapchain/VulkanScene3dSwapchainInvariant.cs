namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Swapchain;

/// <summary>
/// Swapchain 生命周期不变量检查。
/// 用于在关键操作前后断言创建/销毁计数的对称性。
/// 不持有状态，仅通过 VulkanScene3dSwapchainResources 的静态计数器进行判定。
/// </summary>
public static class VulkanScene3dSwapchainInvariant
{
    /// <summary>
    /// 活跃会话应有且仅有一个 Live Swapchain。
    /// LiveCount == 1 && TotalCreate == TotalDestroy + 1
    /// </summary>
    public static bool IsActiveValid()
    {
        var live = VulkanScene3dSwapchainResources.LiveCount;
        return live == 1 &&
               VulkanScene3dSwapchainResources.TotalCreateCount ==
               VulkanScene3dSwapchainResources.TotalDestroyCount + 1;
    }

    /// <summary>
    /// 已销毁会话应有 0 个 Live Swapchain。
    /// LiveCount == 0 && TotalCreate == TotalDestroy
    /// </summary>
    public static bool IsDisposedValid()
    {
        var live = VulkanScene3dSwapchainResources.LiveCount;
        return live == 0 &&
               VulkanScene3dSwapchainResources.TotalCreateCount ==
               VulkanScene3dSwapchainResources.TotalDestroyCount;
    }

    /// <summary>
    /// 获取诊断信息字符串，用于不变量失败时输出。
    /// </summary>
    public static string GetDiagnosticReport()
    {
        return
            $"Swapchain 生命周期诊断：\n" +
            $"Create：{VulkanScene3dSwapchainResources.TotalCreateCount}\n" +
            $"Destroy：{VulkanScene3dSwapchainResources.TotalDestroyCount}\n" +
            $"Live：{VulkanScene3dSwapchainResources.LiveCount}\n" +
            $"预期 Active：Create = Destroy + 1, Live = 1\n" +
            $"预期 Disposed：Create = Destroy, Live = 0";
    }
}
