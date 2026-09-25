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
        var dpi = GetDpiScale();
        var x = message.PhysicalX / dpi;
        var y = message.PhysicalY / dpi;
        ObserveDiagnosticPointer(message, x, y);
        if (message.Message == NativePointerMessage.Move) NativePointerMoved?.Invoke(this, x, y);
        if (message.Message is NativePointerMessage.LeftDown or NativePointerMessage.RightDown or NativePointerMessage.MiddleDown)
            XYContextDropdownBoard.NotifyOwnerPointerDown();
        if (DataContext is UiVm vm)
            NativeViewportInputForwarder.Forward(vm.ViewportInput.Sink, message, new("native-hwnd"), dpi);
        if (message.Message is NativePointerMessage.LeftUp or NativePointerMessage.RightUp
            or NativePointerMessage.MiddleUp or NativePointerMessage.CaptureChanged
            or NativePointerMessage.KillFocus or NativePointerMessage.CancelMode)
            Win32ViewportHost.ReleaseMouseCapture(_hwnd);
    }
}
