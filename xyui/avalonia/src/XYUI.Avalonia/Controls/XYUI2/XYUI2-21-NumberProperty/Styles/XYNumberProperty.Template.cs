using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberProperty
{
    internal static FuncControlTemplate<XYNumberProperty> CreateTemplate() => new((control, scope) =>
    {
        var text = new TextBlock { Name = "PART_LabelText", Text = control.Label, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var label = new Border { Name = "PART_Label", Padding = new Thickness(8, 0), Child = text };
        var field = new XYNumberField { Name = "PART_ValueField", Height = 30, HorizontalAlignment = HorizontalAlignment.Stretch, IsScrubEnabled = true };
        var row = new Grid { Name = "PART_Row", MinHeight = XYPropertyLayoutMetrics.RowHeight, Children = { label, field } };
        XYPropertyLayoutMetrics.ConfigureRow(row, label, field, 0); control.LabelPart = label; control.LabelTextPart = text; control.ValueFieldPart = field; control.RowPart = row;
        label.PointerPressed += control.OnLabelPressed; label.PointerMoved += control.OnLabelMoved; label.PointerReleased += control.OnLabelReleased; label.PointerCaptureLost += control.OnLabelCaptureLost; field.PropertyChanged += control.OnFieldChanged;
        scope?.Register("PART_Label", label); scope?.Register("PART_LabelText", text); scope?.Register("PART_ValueField", field); scope?.Register("PART_Row", row); return row;
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (ValueFieldPart is not null) ValueFieldPart.PropertyChanged -= OnFieldChanged;
        base.OnApplyTemplate(e); LabelPart = e.NameScope.Find<Border>("PART_Label") ?? LabelPart; LabelTextPart = e.NameScope.Find<TextBlock>("PART_LabelText") ?? LabelTextPart; ValueFieldPart = e.NameScope.Find<XYNumberField>("PART_ValueField") ?? ValueFieldPart; RowPart = e.NameScope.Find<Grid>("PART_Row") ?? RowPart;
        if (ValueFieldPart is not null) ValueFieldPart.PropertyChanged += OnFieldChanged; SyncParts();
    }
}
