using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    double _lastRegionSnapX;
    double _lastRegionSnapY;

    void RememberRegionSnapPointer(double x, double y)
    {
        _lastRegionSnapX = x;
        _lastRegionSnapY = y;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key is Key.LeftAlt or Key.RightAlt) RefreshRegionSnapModifier(true);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key is Key.LeftAlt or Key.RightAlt) RefreshRegionSnapModifier(false);
    }

    void RefreshRegionSnapModifier(bool suppressed)
    {
        if (DataContext is not UiVm vm || !vm.IsRegionDrawingDraftActive) return;
        vm.RegionDrawingPointerMoved(_lastRegionSnapX, _lastRegionSnapY, CaptureViewportState(), suppressed);
    }
}
