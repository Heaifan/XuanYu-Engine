using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Layout;

public sealed class XYFieldGroup : XyuiSemanticStack
{
    protected override double Gap(XyuiDensitySemanticMetrics metrics) => metrics.FieldGap;
}
