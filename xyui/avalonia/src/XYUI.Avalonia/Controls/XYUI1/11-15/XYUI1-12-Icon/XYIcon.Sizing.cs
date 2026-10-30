using XYUI.Avalonia.Sizing;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYIcon
{
    void ApplyInheritedSize()
    {
        if (IsSet(SizeProperty)) return;
        ApplySize(XyuiSizingMetrics.IconFor(global::XYUI.Avalonia.XY.GetSize(this)));
    }
}
