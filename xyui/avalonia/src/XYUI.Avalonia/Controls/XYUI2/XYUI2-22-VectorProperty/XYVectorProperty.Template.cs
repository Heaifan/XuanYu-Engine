using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYVectorProperty
{
    internal static FuncControlTemplate<XYVectorProperty> CreateTemplate() => new((control, scope) =>
    {
        var label = new TextBlock { Name = "PART_Label", Text = control.Label, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var axes = new WrapPanel { Name = "PART_Axes", Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
        var row = new Grid { Name = "PART_Row", ColumnDefinitions = new ColumnDefinitions("160,*"), Children = { label, axes } }; Grid.SetColumn(axes, 1);
        control.LabelPart = label; control.AxisPanelPart = axes; control.RowPart = row;
        foreach (var axis in new[] { "X", "Y", "Z", "W" }) { var axisText = new TextBlock { Text = axis, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; var field = new XYNumberField { Width = 110, Height = 30, Tag = axis, IsScrubEnabled = true }; var axisCell = new Border { Classes = { "xyui-vector-axis-cell" }, Child = axisText }; var host = new Border { Width = 140, Margin = new Thickness(0, 0, 6, 0), Child = new Grid { ColumnDefinitions = new ColumnDefinitions("25,*"), Children = { axisCell, field } } }; Grid.SetColumn(field, 1); axes.Children.Add(host); control.AxisFields.Add(field); control.AxisHosts.Add(host); field.PropertyChanged += control.OnAxisChanged; }
        scope?.Register("PART_Label", label); scope?.Register("PART_Axes", axes); scope?.Register("PART_Row", row); control.UpdateLayoutMode(); return row;
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); LabelPart = e.NameScope.Find<TextBlock>("PART_Label") ?? LabelPart; AxisPanelPart = e.NameScope.Find<WrapPanel>("PART_Axes") ?? AxisPanelPart; RowPart = e.NameScope.Find<Grid>("PART_Row") ?? RowPart; SyncParts(); UpdateLayoutMode(); }
}
