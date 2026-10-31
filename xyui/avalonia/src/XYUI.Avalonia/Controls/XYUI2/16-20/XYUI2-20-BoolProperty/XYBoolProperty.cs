using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYBoolProperty : TemplatedControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYBoolProperty, string>(nameof(Label), "属性");
    public static readonly StyledProperty<bool> ValueProperty = AvaloniaProperty.Register<XYBoolProperty, bool>(nameof(Value));
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<XYBoolProperty, bool>(nameof(IsReadOnly));
    public static readonly StyledProperty<double> LabelColumnWidthProperty = AvaloniaProperty.Register<XYBoolProperty, double>(nameof(LabelColumnWidth), 160);
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public double LabelColumnWidth { get => GetValue(LabelColumnWidthProperty); set => SetValue(LabelColumnWidthProperty, value); }
    public event EventHandler? ValueChanged;
    internal XYSwitch? SwitchPart { get; set; }
    internal Grid? RowPart { get; set; }
    internal TextBlock? LabelPart { get; set; }
    internal bool Syncing { get; set; }

    public XYBoolProperty() { Classes.Add("xyui-bool-property"); Focusable = true; }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Space && e.Source is not XYSwitch && IsEnabled && !IsReadOnly) { ToggleValue(); e.Handled = true; return; } base.OnKeyDown(e); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty) { SyncParts(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == LabelProperty || change.Property == IsReadOnlyProperty || change.Property == IsEnabledProperty || change.Property == LabelColumnWidthProperty) SyncParts();
    }
    internal void ToggleValue() { if (IsEnabled && !IsReadOnly) Value = !Value; }
    internal void OnRowPressed(object? sender, PointerPressedEventArgs e) { if (!IsEnabled || IsReadOnly || e.Source is XYSwitch || e.Source is Visual v && v.GetVisualAncestors().OfType<XYSwitch>().Any()) return; Focus(); ToggleValue(); e.Handled = true; }
    internal void SyncParts() { if (SwitchPart is not null) { Syncing = true; SwitchPart.IsChecked = Value; SwitchPart.IsEnabled = IsEnabled && !IsReadOnly; Syncing = false; } if (LabelPart is not null) LabelPart.Text = Label; if (RowPart is not null) RowPart.ColumnDefinitions[0].Width = new GridLength(LabelColumnWidth); }
}
