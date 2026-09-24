using System.Diagnostics;

namespace XuanYu.Editor.UI;

internal sealed record DiagnosticNativePointerSnapshot(
    bool Observed, uint Message, nint Hwnd, int X, int Y,
    DiagnosticNativeViewportPhase Phase, string Target)
{
    public string Format() => $"Observed={Observed};Message=0x{Message:X};" +
        $"Hwnd={Hwnd};X={X};Y={Y};Phase={Phase};Target={Target}";
}

internal static class DiagnosticNativePointerProbe
{
    public static DiagnosticNativePointerSnapshot Capture(
        NativePointerMessage message, DiagnosticNativeViewportPhase phase)
    {
        var result = new DiagnosticNativePointerSnapshot(true, message.Message,
            message.Hwnd, message.PhysicalX, message.PhysicalY, phase, "XYE.VIEWPORT");
        Debug.WriteLine($"[DiagNativeHover] {result.Format()}");
        return result;
    }
}
