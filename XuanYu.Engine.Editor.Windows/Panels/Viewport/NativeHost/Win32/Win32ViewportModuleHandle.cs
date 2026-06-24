using System.Runtime.InteropServices;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

internal static class Win32ViewportModuleHandle
{
    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern nint GetModuleHandle(string? moduleName);
}
