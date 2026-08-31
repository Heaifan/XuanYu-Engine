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
        var width = Bounds.Width; var dimension = (int)Dimension; var wideCandidate = width >= XYPropertyLayoutMetrics.WideBreakpoint;
        var axisWidth = Math.Max(0, width - (wideCandidate ? XYPropertyLayoutMetrics.LabelColumnWidth + XYPropertyLayoutMetrics.ColumnGap : 0));
        var compact = XYPropertyLayoutMetrics.IsCompact(width) || axisWidth < RequiredAxisWidth(dimension); var wide = wideCandidate && !compact;
        Classes.Set("xyui-vector-wide", wide); Classes.Set("xyui-vector-medium", !wide && !compact); Classes.Set("xyui-vector-compact", compact);
        if (wide) XYPropertyLayoutMetrics.ConfigureRow(RowPart, LabelPart!, AxisPanelPart, width); else ConfigureStackedRow();
        ConfigureAxisGrid(dimension, compact);
    }

    static double RequiredAxisWidth(int dimension) => dimension * 128 + (dimension - 1) * 6;

    void ConfigureAxisGrid(int dimension, bool compact)
    {
        AxisPanelPart!.ColumnDefinitions.Clear(); AxisPanelPart.RowDefinitions.Clear();
        if (compact) AxisPanelPart.RowDefinitions.AddRange(Enumerable.Range(0, dimension).Select(_ => new RowDefinition(GridLength.Auto)));
        else AxisPanelPart.ColumnDefinitions.AddRange(Enumerable.Range(0, dimension).Select(_ => new ColumnDefinition(GridLength.Star)));
        for (var i = 0; i < AxisHosts.Count; i++)
        {
            var visible = i < dimension; AxisHosts[i].IsVisible = visible; AxisHosts[i].Margin = compact ? new Thickness(0, 0, 0, visible && i < dimension - 1 ? 6 : 0) : new Thickness(0, 0, visible && i < dimension - 1 ? 6 : 0, 0);
            Grid.SetColumn(AxisHosts[i], compact ? 0 : i); Grid.SetRow(AxisHosts[i], compact ? i : 0);
        }
    }

    void ConfigureStackedRow()
    {
        RowPart!.ColumnDefinitions.Clear(); RowPart.RowDefinitions.Clear(); RowPart.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        RowPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); RowPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Grid.SetColumn(LabelPart!, 0); Grid.SetRow(LabelPart!, 0); Grid.SetColumn(AxisPanelPart!, 0); Grid.SetRow(AxisPanelPart!, 1);
    }
}
