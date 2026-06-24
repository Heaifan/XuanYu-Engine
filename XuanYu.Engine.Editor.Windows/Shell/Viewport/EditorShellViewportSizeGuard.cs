using XuanYu.Engine.Editor.Windows.Panels.Viewport;

namespace XuanYu.Engine.Editor.Windows.Shell.Viewport;

/// <summary>视口尺寸校验工具。检查 NativeHost 尺寸是否可用。</summary>
static class EditorShellViewportSizeGuard
{
    public static bool TryGetValidViewportSize(
        VulkanViewportNativeHostInfo nativeHostInfo,
        out uint width,
        out uint height,
        out string message)
    {
        width = 0;
        height = 0;

        if (nativeHostInfo.Width < 1 || nativeHostInfo.Height < 1)
        {
            message = "Vulkan 视口尺寸尚未就绪，跳过本次绘制。";
            return false;
        }

        width = checked((uint)nativeHostInfo.Width);
        height = checked((uint)nativeHostInfo.Height);
        message = string.Empty;
        return true;
    }
}
