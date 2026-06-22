using System.Runtime.InteropServices;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.Win32;

internal static class Win32ViewportDefaultProc
{
    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    public static extern nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam);
}
