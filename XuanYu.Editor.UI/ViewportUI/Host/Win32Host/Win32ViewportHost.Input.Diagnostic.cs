using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

static partial class Win32ViewportHost
{
    static NativePointerMessage CreateDiagnosticPointerMessage(
        nint hwnd, uint message, nint wParam, nint lParam)
    {
        var before = GetCapture();
        if (message is WM_LBUTTONDOWN or WM_MBUTTONDOWN) SetCapture(hwnd);
        var after = GetCapture();
        var target = message == WM_CAPTURECHANGED ? lParam : 0;
        var point = ReadPoint(hwnd, message, lParam);
        var eventId = ViewportProbe.Enabled ? ViewportProbe.NewNativeInput() : 0;
        var result = new NativePointerMessage(message, (int)wParam,
            point.X, point.Y, hwnd, before, after, target,
            IsAltDown(), 1, IsMetaDown(), eventId);
        if (eventId != 0)
            ViewportProbe.Log("native-input", $"[NATIVE-WNDPROC] EventId={eventId};" +
                $"Message=0x{message:X};Hwnd={hwnd};Physical=({point.X},{point.Y});" +
                $"Buttons=0x{(int)wParam:X};WheelDelta={result.WheelDelta};" +
                $"CaptureBefore={before};CaptureAfter={after};Target={target}");
        return result;
    }
}
