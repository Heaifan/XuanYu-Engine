using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public sealed class XYSectionGroup : XyuiSemanticStack
{
    protected override double Gap(XyuiDensitySemanticMetrics metrics) => metrics.SectionGap;
}
