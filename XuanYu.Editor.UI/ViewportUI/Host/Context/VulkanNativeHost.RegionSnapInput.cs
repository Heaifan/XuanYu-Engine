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

}
