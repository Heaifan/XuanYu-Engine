namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    long _viewportRevision = 1;
    double _pickLogicalW;
    double _pickLogicalH;
    double _pickDpi;

    void ReportPointerPicking(UiVm vm, double x, double y)
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
        vm.PickViewportPointer(x, y, width, height, physical.Width, physical.Height, dpi, _viewportRevision, _hwnd != 0);
    }
}
