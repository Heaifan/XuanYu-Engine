using System.Diagnostics;

namespace XuanYu.Editor.UI;

internal sealed record DiagnosticNativePointerSnapshot(
    bool Observed, uint Message, nint Hwnd, int X, int Y, string Target)
{
    public string Format() => $"Observed={Observed};Message=0x{Message:X};" +
        $"Hwnd={Hwnd};X={X};Y={Y};Target={Target}";
}

internal static class DiagnosticNativePointerProbe
{
    public static DiagnosticNativePointerSnapshot? Capture(NativePointerMessage message)
    {
        if (message.Message != NativePointerMessage.Move) return null;
        var result = new DiagnosticNativePointerSnapshot(true, message.Message,
            message.Hwnd, message.PhysicalX, message.PhysicalY, "XYE.VIEWPORT");
        Debug.WriteLine($"[DiagNativeHover] {result.Format()}");
        return result;
    }
}
