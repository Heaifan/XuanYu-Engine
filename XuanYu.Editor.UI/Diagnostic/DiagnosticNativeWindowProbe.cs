using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

internal sealed record DiagnosticNativeWindowSnapshot(
    nint PopupHwnd, nint OwnerHwnd, nint ParentHwnd, long Style, long ExStyle,
    bool TopMost, bool Visible, string WindowRect, nint ForegroundHwnd)
{
    public string Format() => $"PopupHwnd={PopupHwnd};OwnerHwnd={OwnerHwnd};" +
        $"ParentHwnd={ParentHwnd};Style=0x{Style:X};ExStyle=0x{ExStyle:X};" +
        $"TopMost={TopMost};Visible={Visible};WindowRect={WindowRect};" +
        $"ForegroundHwnd={ForegroundHwnd}";
}

internal static class DiagnosticNativeWindowProbe
{
    const int ExStyleIndex = -20, StyleIndex = -16, OwnerIndex = 4;
    const long TopMostStyle = 0x00000008;
    public static DiagnosticNativeWindowSnapshot Capture(Popup popup, Window owner)
    {
        var popupHwnd = HandleOf(popup.Child);
        var ownerHwnd = HandleOf(owner);
        if (!OperatingSystem.IsWindows())
            return Snapshot(popupHwnd, ownerHwnd, 0, 0, 0, false, false, "0,0,0,0", 0);
        var parent = popupHwnd == 0 ? 0 : GetParent(popupHwnd);
        var style = popupHwnd == 0 ? 0 : GetWindowLongPtr(popupHwnd, StyleIndex).ToInt64();
        var exStyle = popupHwnd == 0 ? 0 : GetWindowLongPtr(popupHwnd, ExStyleIndex).ToInt64();
        var rect = RectOf(popupHwnd);
        var visible = popupHwnd != 0 && IsWindowVisible(popupHwnd);
        var foreground = GetForegroundWindow();
        return Snapshot(popupHwnd, GetWindow(popupHwnd, OwnerIndex), parent, style,
            exStyle, (exStyle & TopMostStyle) != 0, visible, rect, foreground);
    }

    static DiagnosticNativeWindowSnapshot Snapshot(nint popup, nint owner, nint parent,
        long style, long exStyle, bool topMost, bool visible, string rect, nint foreground)
    {
        var result = new DiagnosticNativeWindowSnapshot(popup, owner, parent, style,
            exStyle, topMost, visible, rect, foreground);
        Debug.WriteLine($"[POPUP-OWNER-PROBE] {result.Format()}");
        return result;
    }

    static nint HandleOf(Control? control) => control is TopLevel top
        ? top.TryGetPlatformHandle()?.Handle ?? 0 : TopLevel.GetTopLevel(control)?.TryGetPlatformHandle()?.Handle ?? 0;

    static string RectOf(nint hwnd)
    {
        if (hwnd == 0 || !GetWindowRect(hwnd, out var rect)) return "0,0,0,0";
        return $"{rect.Left},{rect.Top},{rect.Right},{rect.Bottom}";
    }

    [DllImport("user32", EntryPoint = "GetWindowLongPtrW")] static extern nint GetWindowLongPtr(nint hWnd, int index);
    [DllImport("user32")] static extern nint GetWindow(nint hWnd, int command);
    [DllImport("user32")] static extern nint GetParent(nint hWnd);
    [DllImport("user32")] static extern nint GetForegroundWindow();
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool IsWindowVisible(nint hWnd);
    [DllImport("user32")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetWindowRect(nint hWnd, out NativeRect rect);

    [StructLayout(LayoutKind.Sequential)] struct NativeRect { public int Left, Top, Right, Bottom; }
}
