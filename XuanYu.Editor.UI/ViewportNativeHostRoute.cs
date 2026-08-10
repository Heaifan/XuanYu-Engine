using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public static class ViewportNativeHostRoute
{
    public static void Report(UiVm? vm, NativeHostHandleSnapshot snapshot) =>
        vm?.LogNativeHostLifecycle(snapshot);

    public static void ReportMerged(UiVm? vm, NativeHostHandleSnapshot snapshot, int mergeCount) =>
        vm?.LogNativeHostResizedMerged(snapshot, mergeCount);

    public static void ReportVulkanBridge(UiVm? vm, string message) =>
        vm?.LogVulkanLifecycle(message, "");

    public static void ReportProbe(UiVm? vm, bool isLogOpen, int logicalW, int logicalH, double dpi, int targetW, int targetH, int clientW, int clientH) =>
        vm?.LogNativeHostProbe(isLogOpen, logicalW, logicalH, dpi, targetW, targetH, clientW, clientH);

    internal static void ReportScaleIndicatorProbe(UiVm? vm, Win32ViewportHost.ScaleIndicatorProbe probe) =>
        vm?.LogScaleIndicatorProbe(probe);
}
