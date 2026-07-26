using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    bool TryBeginAvaloniaCamera(PointerPressedEventArgs e, PointerPoint point)
    {
        if (!point.Properties.IsMiddleButtonPressed || DataContext is not UiVm vm) return false;
        var begun = vm.BeginCameraNavigation(e.Pointer.Id, point.Position.X, point.Position.Y,
            e.KeyModifiers.HasFlag(KeyModifiers.Shift), (int)Bounds.Width, (int)Bounds.Height);
        if (!begun) return false;
        e.Pointer.Capture(this);
        e.Handled = true;
        return true;
    }

    static void ReleaseAvaloniaCapture(PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);
        e.Handled = true;
    }
}
