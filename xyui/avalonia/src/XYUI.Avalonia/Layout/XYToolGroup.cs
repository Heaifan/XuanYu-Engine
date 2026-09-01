using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public sealed class XYToolGroup : XyuiSemanticStack
{
    public XYToolGroup() => Orientation = global::Avalonia.Layout.Orientation.Horizontal;
    protected override double Gap(XyuiDensitySemanticMetrics metrics) => metrics.ToolItemGap;
}
