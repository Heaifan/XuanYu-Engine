using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Controls;

public enum XYVectorDimension { Vector2 = 2, Vector3 = 3, Vector4 = 4 }

public partial class XYVectorProperty : TemplatedControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYVectorProperty, string>(nameof(Label), "向量");
    public static readonly StyledProperty<XYVectorDimension> DimensionProperty = AvaloniaProperty.Register<XYVectorProperty, XYVectorDimension>(nameof(Dimension), XYVectorDimension.Vector3);
    public static readonly StyledProperty<double> XProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(X));
    public static readonly StyledProperty<double> YProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(Y));
    public static readonly StyledProperty<double> ZProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(Z));
    public static readonly StyledProperty<double> WProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(W));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(Minimum), -100000);
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(Maximum), 100000);
    public static readonly StyledProperty<double> StepProperty = AvaloniaProperty.Register<XYVectorProperty, double>(nameof(Step), 0.1);
    public static readonly StyledProperty<int> DecimalPlacesProperty = AvaloniaProperty.Register<XYVectorProperty, int>(nameof(DecimalPlaces), 2);
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<XYVectorProperty, bool>(nameof(IsReadOnly));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XYVectorDimension Dimension { get => GetValue(DimensionProperty); set => SetValue(DimensionProperty, value); }
    public double X { get => GetValue(XProperty); set => SetValue(XProperty, value); }
    public double Y { get => GetValue(YProperty); set => SetValue(YProperty, value); }
    public double Z { get => GetValue(ZProperty); set => SetValue(ZProperty, value); }
    public double W { get => GetValue(WProperty); set => SetValue(WProperty, value); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public int DecimalPlaces { get => GetValue(DecimalPlacesProperty); set => SetValue(DecimalPlacesProperty, Math.Max(0, value)); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public event EventHandler? ValueChanged;
    internal TextBlock? LabelPart { get; set; }
    internal Grid? AxisPanelPart { get; set; }
    internal List<XYNumberField> AxisFields { get; } = [];
    internal List<Border> AxisHosts { get; } = [];
    internal Grid? RowPart { get; set; }
    internal bool Syncing { get; set; }

    public XYVectorProperty() { Classes.Add("xyui-vector-property"); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == XProperty || change.Property == YProperty || change.Property == ZProperty || change.Property == WProperty) { SyncParts(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == LabelProperty || change.Property == DimensionProperty || change.Property == MinimumProperty || change.Property == MaximumProperty || change.Property == StepProperty || change.Property == DecimalPlacesProperty || change.Property == IsReadOnlyProperty || change.Property == IsEnabledProperty) SyncParts();
    }
    internal void OnAxisChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != XYNumberField.ValueProperty || Syncing || sender is not XYNumberField field) return;
        switch (field.Tag as string) { case "X": X = field.Value; break; case "Y": Y = field.Value; break; case "Z": Z = field.Value; break; case "W": W = field.Value; break; }
    }
    internal double AxisValue(string axis) => axis switch { "X" => X, "Y" => Y, "Z" => Z, _ => W };
    internal void SetAxisValue(string axis, double value) { switch (axis) { case "X": X = value; break; case "Y": Y = value; break; case "Z": Z = value; break; default: W = value; break; } }
    internal void SyncParts()
    {
        if (LabelPart is not null) LabelPart.Text = Label;
        for (var i = 0; i < AxisFields.Count; i++) { var field = AxisFields[i]; var visible = i < (int)Dimension; field.IsVisible = visible; field.IsEnabled = IsEnabled; field.IsReadOnly = IsReadOnly; field.Minimum = Minimum; field.Maximum = Maximum; field.Step = Step; field.DecimalPlaces = DecimalPlaces; Syncing = true; field.Value = AxisValue(field.Tag as string ?? "W"); Syncing = false; AxisHosts[i].IsVisible = visible; }
    }
}
