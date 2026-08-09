using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    long _viewportRevision = 1;
    double _pickLogicalW;
    double _pickLogicalH;
    double _pickDpi;

    void ReportPointerPicking(UiVm vm, double x, double y)
    {
        var viewport = CaptureViewportState();
        vm.PickViewportPointer(x, y, (int)viewport.LogicalWidth, (int)viewport.LogicalHeight,
            viewport.PhysicalWidth, viewport.PhysicalHeight, viewport.DpiScale,
            viewport.Revision, _hwnd != 0);
    }

    bool ReportRegionDrawing(UiVm vm, double x, double y)
    {
        F1ForensicTrace.Routing(vm, x, y);
        return vm.RegionDrawingPointerPressed(x, y, CaptureViewportState());
    }

    bool PreviewRegionDrawing(UiVm vm, double x, double y) =>
        vm.RegionDrawingPointerMoved(x, y, CaptureViewportState());

    ViewportState CaptureViewportState()
    {
        var width = Math.Max(1, (int)Math.Round(Bounds.Width));
        var height = Math.Max(1, (int)Math.Round(Bounds.Height));
        var dpi = GetDpiScale();
        if (width != (int)_pickLogicalW || height != (int)_pickLogicalH || Math.Abs(dpi - _pickDpi) > 0.000001)
        {
            _viewportRevision++;
            _pickLogicalW = width;
            _pickLogicalH = height;
            _pickDpi = dpi;
        }

        var physical = ToPhysicalSize(width, height, dpi);
        return new ViewportState(
            0, 0, width, height, physical.Width, physical.Height, dpi, _viewportRevision);
    }
}
