using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    bool _nativeCameraActive;

    bool TryBeginNativeCamera(UiVm vm, long pointerId, double x, double y, bool shift)
    {
        var begun = vm.BeginCameraNavigation(pointerId, x, y, shift, (int)Bounds.Width, (int)Bounds.Height);
        if (begun) _nativeCameraActive = true;
        return begun;
    }

    void PreviewNativeCamera(UiVm vm, double x, double y)
    {
        if (_nativeCameraActive) vm.PreviewCameraNavigation(NativePointerId, x, y);
    }

    void EndNativeCamera(UiVm vm)
    {
        if (!_nativeCameraActive) return;
        _nativeCameraActive = false;
        vm.EndCameraNavigation(NativePointerId);
        ReleaseExpectedCapture();
    }

    void CancelNativeCamera(UiVm vm, string reason)
    {
        if (!_nativeCameraActive) return;
        _nativeCameraActive = false;
        vm.CancelCameraNavigation(reason);
        ReleaseExpectedCapture();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (DataContext is UiVm vm && vm.DollyCamera(e.Delta.Y)) e.Handled = true;
    }
}
