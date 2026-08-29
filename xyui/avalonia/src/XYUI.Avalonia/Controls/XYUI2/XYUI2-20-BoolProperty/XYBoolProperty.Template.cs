using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Controls;

public partial class XYBoolProperty
{
    internal static FuncControlTemplate<XYBoolProperty> CreateTemplate() => new((control, scope) =>
    {
        var label = new TextBlock { Name = "PART_Label", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis, Text = control.Label };
        var toggle = new XYSwitch { Name = "PART_Switch", IsChecked = control.Value, IsEnabled = control.IsEnabled && !control.IsReadOnly, HorizontalAlignment = HorizontalAlignment.Left };
        control.LabelPart = label; control.SwitchPart = toggle;
        var row = new Grid { Name = "PART_Row", Height = XyuiComponentTokens.BoolPropertyRowHeight, ColumnDefinitions = new ColumnDefinitions { new(control.LabelColumnWidth, GridUnitType.Pixel), new(1, GridUnitType.Star) }, Children = { label, toggle } }; Grid.SetColumn(toggle, 1); control.RowPart = row;
        row.AddHandler(InputElement.PointerPressedEvent, control.OnRowPressed, RoutingStrategies.Bubble, true); scope?.Register("PART_Label", label); scope?.Register("PART_Switch", toggle); scope?.Register("PART_Row", row); return row;
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); LabelPart = e.NameScope.Find<TextBlock>("PART_Label") ?? LabelPart; SwitchPart = e.NameScope.Find<XYSwitch>("PART_Switch") ?? SwitchPart; RowPart = e.NameScope.Find<Grid>("PART_Row") ?? RowPart; if (SwitchPart is not null) { SwitchPart.PropertyChanged -= OnSwitchChanged; SwitchPart.PropertyChanged += OnSwitchChanged; } SyncParts(); }
    void OnSwitchChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == ToggleButton.IsCheckedProperty && !Syncing && SwitchPart is not null) Value = SwitchPart.IsChecked == true; }
}
