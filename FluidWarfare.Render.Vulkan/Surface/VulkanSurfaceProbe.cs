using System.Diagnostics;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Surface;

/// <summary>
/// 使用外部传入的 Windows 原生句柄创建并立即释放 Vulkan Surface。
/// 内部使用 VulkanSurfaceInstanceScope 管理临时 VkInstance 生命周期。
/// </summary>
public static unsafe class VulkanSurfaceProbe
{
    const string PlatformText = "Windows";

    public static VulkanSurfaceInfo ProbeWindows(nint hinstance, nint hwnd)
    {
        var sw = Stopwatch.StartNew();

        if (!OperatingSystem.IsWindows())
        {
            sw.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.UnsupportedPlatform,
                "当前平台不支持 Windows Vulkan Surface 创建。",
                PlatformText, false, sw.Elapsed.TotalMilliseconds);
        }

        if (hinstance == 0 || hwnd == 0)
        {
            sw.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Failed,
                "Windows 原生窗口句柄不可用，无法创建 Vulkan Surface。",
                PlatformText, false, sw.Elapsed.TotalMilliseconds);
        }

        try
        {
            using var scope = new VulkanSurfaceInstanceScope();

            var r = scope.CreateInstance();
            if (r != Result.Success)
                return Fail($"Vulkan Surface 创建失败：Instance 创建失败：{r}。", sw);

            if (!scope.HasSurfaceFunctions)
                return Fail("Vulkan Surface 创建失败：无法加载 Windows Surface 扩展函数。", sw);

            r = scope.CreateWin32Surface(hinstance, hwnd);
            if (r != Result.Success)
                return Fail($"Vulkan Surface 创建失败：{r}。", sw);

            sw.Stop();
            return new VulkanSurfaceInfo(
                VulkanSurfaceStatus.Created,
                "Vulkan Surface 创建成功，并已立即释放。",
                PlatformText, true, sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            sw.Stop();
            return Fail($"Vulkan Surface 创建失败：{ex.Message}", sw);
        }
    }

    static VulkanSurfaceInfo Fail(string message, Stopwatch sw) =>
        new(VulkanSurfaceStatus.Failed, message, PlatformText, true, sw.Elapsed.TotalMilliseconds);
}
