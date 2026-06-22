using System.Diagnostics;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Dispose 诊断日志。
/// 仅在 DEBUG 下输出，不影响 Release 性能。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    /// <summary>记录 Dispose 步骤（DEBUG 模式）。</summary>
    [Conditional("DEBUG")]
    private void TraceDispose(string step)
    {
        Debug.WriteLine($"[Dispose] {step}");
    }

    /// <summary>记录 DisposeResources 开始。</summary>
    [Conditional("DEBUG")]
    private void TraceDisposeResources() => TraceDispose("DisposeResources 开始");

    /// <summary>记录 DisposeSessionResources 开始。</summary>
    [Conditional("DEBUG")]
    private void TraceDisposeSession() => TraceDispose("DisposeSessionResources 开始");

    /// <summary>记录 Dispose 不变量失败。</summary>
    private static void TraceInvariantFailure(string diag)
    {
        Debug.WriteLine($"[严重]Session Dispose 后 Swapchain 不变量失效。\n{diag}");
    }
}
