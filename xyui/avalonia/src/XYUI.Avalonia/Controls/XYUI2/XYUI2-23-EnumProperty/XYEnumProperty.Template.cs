using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYEnumProperty
{
    internal static FuncControlTemplate<XYEnumProperty> CreateTemplate() => new((control, scope) =>
    {
        var label = new TextBlock { Name = "PART_Label", Text = control.Label, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var select = new XYSelect { Name = "PART_Select", Width = 180, Height = 30, ItemsSource = control.ItemsSource, SelectedItem = control.SelectedItem, SelectedIndex = control.SelectedIndex };
        var row = new Grid { Name = "PART_Row", Height = XYPropertyLayoutMetrics.RowHeight, ColumnDefinitions = new ColumnDefinitions("*,Auto"), Children = { label, select } }; Grid.SetColumn(select, 1);
        control.LabelPart = label; control.SelectPart = select; select.SelectionChanged += control.OnSelectChanged; scope?.Register("PART_Label", label); scope?.Register("PART_Select", select); scope?.Register("PART_Row", row); return row;
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { if (SelectPart is not null) SelectPart.SelectionChanged -= OnSelectChanged; base.OnApplyTemplate(e); LabelPart = e.NameScope.Find<TextBlock>("PART_Label") ?? LabelPart; SelectPart = e.NameScope.Find<XYSelect>("PART_Select") ?? SelectPart; if (SelectPart is not null) SelectPart.SelectionChanged += OnSelectChanged; SyncParts(); }
}
