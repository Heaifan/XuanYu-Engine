using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYVectorProperty
{
    protected override void OnSizeChanged(SizeChangedEventArgs e) { base.OnSizeChanged(e); UpdateLayoutMode(); }
    internal void UpdateLayoutMode()
    {
        if (RowPart is null || AxisPanelPart is null) return;
        var width = Bounds.Width; var dimension = (int)Dimension; var compact = XYPropertyLayoutMetrics.IsCompact(width); var wide = width >= XYPropertyLayoutMetrics.WideBreakpoint && width >= WideMinimum(dimension);
        Classes.Set("xyui-vector-wide", wide); Classes.Set("xyui-vector-medium", !wide && !compact); Classes.Set("xyui-vector-compact", compact);
        XYPropertyLayoutMetrics.ConfigureRow(RowPart, LabelPart!, AxisPanelPart, width); AxisPanelPart.Orientation = compact ? Orientation.Vertical : Orientation.Horizontal;
        var available = AxisPanelPart.Bounds.Width > 0 ? AxisPanelPart.Bounds.Width : Math.Max(0, width - (wide ? XYPropertyLayoutMetrics.LabelColumnWidth + XYPropertyLayoutMetrics.ColumnGap : 0));
        var gap = compact ? 0 : 6; var hostWidth = compact ? double.NaN : Math.Max(96, (available - gap * (dimension - 1)) / dimension);
        for (var i = 0; i < AxisHosts.Count; i++) { AxisHosts[i].Width = hostWidth; AxisHosts[i].Margin = compact ? new Thickness(0, 0, 0, i == dimension - 1 ? 0 : 6) : new Thickness(0, 0, i == dimension - 1 ? 0 : 6, 0); }
    }

    static double WideMinimum(int dimension) => XYPropertyLayoutMetrics.LabelColumnWidth + XYPropertyLayoutMetrics.ColumnGap + dimension * 96 + (dimension - 1) * 6;
}
