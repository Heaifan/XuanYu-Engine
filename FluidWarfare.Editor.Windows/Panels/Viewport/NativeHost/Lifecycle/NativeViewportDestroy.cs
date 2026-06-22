using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;

/// <summary>NativeHost 子窗口销毁。</summary>
sealed class NativeViewportDestroy
{
    public static void Destroy(nint windowHandle, ref nint storage)
    {
        if (windowHandle != 0)
        {
            Win32ViewportDestroyWindow.Destroy(windowHandle);
            storage = 0;
        }
    }
}
