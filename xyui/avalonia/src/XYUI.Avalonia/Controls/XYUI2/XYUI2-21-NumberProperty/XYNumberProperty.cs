using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberProperty : TemplatedControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYNumberProperty, string>(nameof(Label), "属性");
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(Value));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(Minimum));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<double> StepProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(Step), 1);
    public static readonly StyledProperty<double> LargeStepProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(LargeStep), 10);
    public static readonly StyledProperty<double> SmallStepProperty = AvaloniaProperty.Register<XYNumberProperty, double>(nameof(SmallStep), .1);
    public static readonly StyledProperty<int> DecimalPlacesProperty = AvaloniaProperty.Register<XYNumberProperty, int>(nameof(DecimalPlaces), 2);
    public static readonly StyledProperty<string?> SuffixProperty = AvaloniaProperty.Register<XYNumberProperty, string?>(nameof(Suffix));
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<XYNumberProperty, bool>(nameof(IsReadOnly));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum)); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public double LargeStep { get => GetValue(LargeStepProperty); set => SetValue(LargeStepProperty, value); }
    public double SmallStep { get => GetValue(SmallStepProperty); set => SetValue(SmallStepProperty, value); }
    public int DecimalPlaces { get => GetValue(DecimalPlacesProperty); set => SetValue(DecimalPlacesProperty, Math.Max(0, value)); }
    public string? Suffix { get => GetValue(SuffixProperty); set => SetValue(SuffixProperty, value); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public event EventHandler? ValueChanged;
    internal Border? LabelPart { get; set; }
    internal TextBlock? LabelTextPart { get; set; }
    internal XYNumberField? ValueFieldPart { get; set; }
    internal bool Syncing { get; set; }

    public XYNumberProperty() { Classes.Add("xyui-number-property"); Focusable = true; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty) { SyncParts(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == LabelProperty || change.Property == MinimumProperty || change.Property == MaximumProperty || change.Property == StepProperty || change.Property == LargeStepProperty || change.Property == SmallStepProperty || change.Property == DecimalPlacesProperty || change.Property == SuffixProperty || change.Property == IsReadOnlyProperty || change.Property == IsEnabledProperty) SyncParts();
    }
    internal void OnFieldChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == XYNumberField.ValueProperty && !Syncing && ValueFieldPart is not null) Value = ValueFieldPart.Value; }
    internal void SyncParts()
    {
        if (LabelTextPart is not null) LabelTextPart.Text = Label;
        if (ValueFieldPart is null) return;
        Syncing = true; ValueFieldPart.Minimum = Minimum; ValueFieldPart.Maximum = Maximum; ValueFieldPart.Step = Step; ValueFieldPart.LargeStep = LargeStep; ValueFieldPart.SmallStep = SmallStep; ValueFieldPart.DecimalPlaces = DecimalPlaces; ValueFieldPart.Suffix = Suffix; ValueFieldPart.IsReadOnly = IsReadOnly; ValueFieldPart.IsEnabled = IsEnabled; ValueFieldPart.Value = Value; Syncing = false;
    }
}
