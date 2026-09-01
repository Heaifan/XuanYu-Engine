using Avalonia.Controls;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Gallery.Views;

public sealed record SpacingMetricsViewModel(
    double CompactToolItem, double ComfortableToolItem, double TouchToolItem,
    double CompactToolGroup, double ComfortableToolGroup, double TouchToolGroup,
    double CompactIconText, double ComfortableIconText, double TouchIconText,
    double CompactField, double ComfortableField, double TouchField,
    double CompactSection, double ComfortableSection, double TouchSection,
    double CompactPadding, double ComfortablePadding, double TouchPadding)
{
    public static SpacingMetricsViewModel Create()
    {
        XyuiDensity.TryGetSemanticMetrics(XyuiDensityMode.Compact, out var c);
        XyuiDensity.TryGetSemanticMetrics(XyuiDensityMode.Comfortable, out var comf);
        XyuiDensity.TryGetSemanticMetrics(XyuiDensityMode.Touch, out var t);
        return new(
            c.ToolItemGap, comf.ToolItemGap, t.ToolItemGap,
            c.ToolGroupGap, comf.ToolGroupGap, t.ToolGroupGap,
            c.IconTextGap, comf.IconTextGap, t.IconTextGap,
            c.FieldGap, comf.FieldGap, t.FieldGap,
            c.SectionGap, comf.SectionGap, t.SectionGap,
            c.PanelPadding, comf.PanelPadding, t.PanelPadding);
    }
}

public partial class SpacingLayoutView : UserControl
{
    public SpacingLayoutView()
    {
        InitializeComponent();
        DataContext = SpacingMetricsViewModel.Create();
    }
}
