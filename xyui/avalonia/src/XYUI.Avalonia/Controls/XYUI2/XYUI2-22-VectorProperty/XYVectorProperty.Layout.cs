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
        var width = Bounds.Width; var compact = width > 0 && width < XYPropertyLayoutMetrics.CompactBreakpoint; var wide = width >= XYPropertyLayoutMetrics.WideBreakpoint;
        Classes.Set("xyui-vector-wide", wide); Classes.Set("xyui-vector-medium", !wide && !compact); Classes.Set("xyui-vector-compact", compact);
        RowPart.ColumnDefinitions.Clear(); RowPart.RowDefinitions.Clear();
        if (wide) { RowPart.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(XYPropertyLayoutMetrics.LabelColumnWidth, GridUnitType.Pixel))); RowPart.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); Grid.SetColumn(LabelPart!, 0); Grid.SetRow(LabelPart!, 0); Grid.SetColumn(AxisPanelPart, 1); Grid.SetRow(AxisPanelPart, 0); AxisPanelPart.Orientation = Orientation.Horizontal; }
        else { RowPart.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); RowPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); RowPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); Grid.SetColumn(LabelPart!, 0); Grid.SetRow(LabelPart!, 0); Grid.SetColumn(AxisPanelPart, 0); Grid.SetRow(AxisPanelPart, 1); AxisPanelPart.Orientation = compact ? Orientation.Vertical : Orientation.Horizontal; }
        foreach (var host in AxisHosts) host.Width = compact ? double.NaN : 140;
    }
}
