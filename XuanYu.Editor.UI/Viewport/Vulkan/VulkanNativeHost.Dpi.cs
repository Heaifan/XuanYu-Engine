namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    static (int Width, int Height) ToPhysicalSize(int logicalW, int logicalH, double dpi) =>
        (Math.Max(1, (int)Math.Round(logicalW * dpi)),
         Math.Max(1, (int)Math.Round(logicalH * dpi)));
}
