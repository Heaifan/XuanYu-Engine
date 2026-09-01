using XYUI.Avalonia.Spatial;
using XYUI.Avalonia;

namespace XYUI.Avalonia.Density;

public enum XyuiDensity
{
    Compact,
    Default,
    Comfortable,
}

public readonly record struct XyuiDensityMetrics(
    double RowGap,
    double SectionGap,
    double PanelPadding)
{
    public static XyuiDensityMetrics For(XYDensity density) => For((XyuiDensity)density);

    public static XyuiDensityMetrics For(XyuiDensity density) => density switch
    {
        XyuiDensity.Compact => new(XyuiSpatialTokens.FieldRowGap,
            XyuiSpatialTokens.SectionGap, XyuiSpatialTokens.PanelPadding),
        XyuiDensity.Comfortable => new(XyuiSpatialTokens.Space2,
            XyuiSpatialTokens.Space3, XyuiSpatialTokens.Space3),
        _ => new(XyuiSpatialTokens.FieldRowGap,
            XyuiSpatialTokens.SectionGap, XyuiSpatialTokens.PanelPadding),
    };
}
