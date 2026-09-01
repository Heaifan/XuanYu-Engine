using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public sealed class XYToolbar : XyuiSemanticStack
{
    public XYToolbar() => Orientation = global::Avalonia.Layout.Orientation.Horizontal;
    protected override double Gap(XyuiDensitySemanticMetrics metrics) => metrics.ToolGroupGap;
}
