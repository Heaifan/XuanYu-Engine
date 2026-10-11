using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    DiagnosticNativePointerSnapshot? _lastNativePointerProbe;
    bool _nativeDiagnosticInside;
    internal static event Action<VulkanNativeHost, DiagnosticNativeViewportEvent>? NativeViewportDiagnosticChanged;
    internal static event Action<VulkanNativeHost, double, double>? NativePointerMoved;

    void OnNativePointerMessage(NativePointerMessage message)
    {
        if (message.DiagnosticEventId == 0)
        {
            RouteNativePointerMessage(message);
            return;
        }
        var previousNativeInput = XuanYu.Core.Diagnostics.ViewportProbe.SetNativeInput(
            message.DiagnosticEventId);
        try { RouteNativePointerMessage(message); }
        finally { XuanYu.Core.Diagnostics.ViewportProbe.RestoreNativeInput(previousNativeInput); }
    }

    void RouteNativePointerMessage(NativePointerMessage message)
    {
        var dpi = GetDpiScale();
        var x = message.PhysicalX / dpi;
        var y = message.PhysicalY / dpi;
        ObserveDiagnosticPointer(message, x, y);
        if (message.Message == NativePointerMessage.Move) NativePointerMoved?.Invoke(this, x, y);
        if (message.Message == NativePointerMessage.LeftDown) _mapContextMenu?.Close();
        if (message.Message is NativePointerMessage.LeftDown or NativePointerMessage.RightDown or NativePointerMessage.MiddleDown)
            XYContextDropdownBoard.NotifyOwnerPointerDown();
        if (DataContext is UiVm vm)
        {
            if (message.DiagnosticEventId != 0)
                XuanYu.Core.Diagnostics.ViewportProbe.Log("native-input",
                    $"[NATIVE-HWND-DISPATCH] EventId={message.DiagnosticEventId};" +
                    $"Message=0x{message.Message:X};Hwnd={message.Hwnd};" +
                    $"Physical=({message.PhysicalX},{message.PhysicalY});" +
                    $"WheelDelta={message.WheelDelta};Source=native-hwnd;" +
                    $"CameraRevision={vm.RenderSnapshot.CameraState.Revision};" +
                    $"ObservationCenter={vm.ObservationCenter}");
            NativeViewportInputForwarder.Forward(vm.ViewportInput.Sink, message, new("native-hwnd"), dpi);
            if (message.DiagnosticEventId != 0)
                XuanYu.Core.Diagnostics.ViewportProbe.Log("native-input",
                    $"[NATIVE-HWND-ROUTED] EventId={message.DiagnosticEventId};" +
                    $"Message=0x{message.Message:X};CameraRevision={vm.RenderSnapshot.CameraState.Revision};" +
                    $"ObservationCenter={vm.ObservationCenter}");
        }
        if (message.Message is NativePointerMessage.LeftUp or NativePointerMessage.RightUp
            or NativePointerMessage.MiddleUp or NativePointerMessage.CaptureChanged
            or NativePointerMessage.KillFocus or NativePointerMessage.CancelMode)
            Win32ViewportHost.ReleaseMouseCapture(_hwnd);
    }
}
