using System.Runtime.InteropServices;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

internal static class Win32ViewportDestroyWindow
{
    [DllImport("user32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Destroy(nint hwnd);
}
