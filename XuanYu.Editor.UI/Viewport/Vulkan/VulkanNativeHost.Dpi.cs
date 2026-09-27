namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    static (int Width, int Height) ToPhysicalSize(int logicalW, int logicalH, double dpi) =>
        NativeHostSurfaceContract.ToPhysical(logicalW, logicalH, dpi);
}
