namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    void ObserveDiagnosticPointer(NativePointerMessage message, double x, double y)
    {
        if (message.Message == NativePointerMessage.Move)
        {
            var phase = _nativeDiagnosticInside
                ? DiagnosticNativeViewportPhase.Moved
                : DiagnosticNativeViewportPhase.Entered;
            _nativeDiagnosticInside = true;
            PublishDiagnosticPointer(message, phase, x, y);
        }
        else if (message.Message == NativePointerMessage.MouseLeave)
        {
            _nativeDiagnosticInside = false;
            PublishDiagnosticPointer(message, DiagnosticNativeViewportPhase.Exited, x, y);
        }
    }

    void PublishDiagnosticPointer(NativePointerMessage message,
        DiagnosticNativeViewportPhase phase, double x, double y)
    {
        _lastNativePointerProbe = DiagnosticNativePointerProbe.Capture(message, phase);
        NativeViewportDiagnosticChanged?.Invoke(this,
            new DiagnosticNativeViewportEvent(this, phase, x, y));
    }

    void ExitDiagnosticPointer()
    {
        if (!_nativeDiagnosticInside) return;
        _nativeDiagnosticInside = false;
        var message = new NativePointerMessage(NativePointerMessage.MouseLeave,
            0, 0, 0, _hwnd, 0, 0, 0);
        PublishDiagnosticPointer(message, DiagnosticNativeViewportPhase.Exited, 0, 0);
    }
}
