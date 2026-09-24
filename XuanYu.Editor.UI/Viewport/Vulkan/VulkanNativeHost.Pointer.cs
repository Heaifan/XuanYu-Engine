namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    const long NativePointerId = 1;
    bool _nativeDragActive;
    bool _mapGeometryDragActive;
    bool _expectedCaptureRelease;
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
        if (DataContext is UiVm vm)
            NativeViewportInputForwarder.Forward(
                vm.ViewportInput.Sink, message, new("native-hwnd"), dpi);
        if (message.Message is NativePointerMessage.LeftUp or NativePointerMessage.RightUp
            or NativePointerMessage.MiddleUp or NativePointerMessage.CaptureChanged
            or NativePointerMessage.KillFocus or NativePointerMessage.CancelMode)
            Win32ViewportHost.ReleaseMouseCapture(_hwnd);
    }
    void PreviewNativePointer(UiVm vm, double x, double y, bool snapSuppressed)
    {
        if (_nativeDragActive) vm.PreviewViewportPointer(NativePointerId, x, y);
        else if (vm.IsRegionDrawingTool) PreviewDrawing(vm, x, y, snapSuppressed);
    }
    void CommitNativePointer(UiVm vm, double x, double y)
    {
        if (!_nativeDragActive) return;
        vm.CommitViewportPointer(NativePointerId, x, y);
        _nativeDragActive = false;
        ReleaseExpectedCapture();
    }
    void ReleaseExpectedCapture()
    {
        _expectedCaptureRelease = true;
        Win32ViewportHost.ReleaseMouseCapture(_hwnd);
        _expectedCaptureRelease = false;
    }
    bool IsInBounds(double x, double y) =>
        x >= 0 && y >= 0 && x <= Bounds.Width && y <= Bounds.Height;
}
