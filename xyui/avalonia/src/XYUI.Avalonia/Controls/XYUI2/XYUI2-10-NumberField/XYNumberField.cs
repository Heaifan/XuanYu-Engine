using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberField : XYTextField
{
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(Value));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(Minimum));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<double> StepProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(Step), 1);
    public static readonly StyledProperty<double> LargeStepProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(LargeStep), 10);
    public static readonly StyledProperty<double> SmallStepProperty = AvaloniaProperty.Register<XYNumberField, double>(nameof(SmallStep), .1);
    public static readonly StyledProperty<string?> SuffixProperty = AvaloniaProperty.Register<XYNumberField, string?>(nameof(Suffix));
    public static readonly StyledProperty<int> DecimalPlacesProperty = AvaloniaProperty.Register<XYNumberField, int>(nameof(DecimalPlaces), 2);
    public static readonly StyledProperty<bool> IsScrubEnabledProperty = AvaloniaProperty.Register<XYNumberField, bool>(nameof(IsScrubEnabled), true);
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum)); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public double LargeStep { get => GetValue(LargeStepProperty); set => SetValue(LargeStepProperty, value); }
    public double SmallStep { get => GetValue(SmallStepProperty); set => SetValue(SmallStepProperty, value); }
    public string? Suffix { get => GetValue(SuffixProperty); set => SetValue(SuffixProperty, value); }
    public int DecimalPlaces { get => GetValue(DecimalPlacesProperty); set => SetValue(DecimalPlacesProperty, Math.Max(0, value)); }
    public bool IsScrubEnabled { get => GetValue(IsScrubEnabledProperty); set => SetValue(IsScrubEnabledProperty, value); }
    internal bool IsScrubbing { get; set; }
    internal bool IsScrubArmed { get; set; }
    internal double ScrubStartValue { get; set; }
    double _editStartValue;
    Control? _stepper;
    internal Control? ValueHost { get; set; }
    public XYNumberField()
    {
        Classes.Add("xyui-number-field"); TextChanged += OnTextChanged; KeyDown += OnNumberKeyDown;
        PointerEntered += (_, _) => SetStepperVisibility(true); PointerExited += (_, _) => SetStepperVisibility(IsFocused);
        LostFocus += (_, _) => CommitText();
    }
    internal void Adjust(double amount) { CommitText(); Value += amount; SyncText(); }
    protected override void OnPointerPressed(PointerPressedEventArgs e) { base.OnPointerPressed(e); OnNumberPointerPressed(this, e); }
    protected override void OnPointerMoved(PointerEventArgs e) { base.OnPointerMoved(e); OnNumberPointerMoved(this, e); }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { base.OnPointerReleased(e); OnNumberPointerReleased(this, e); }
    protected override void OnGotFocus(FocusChangedEventArgs e) { _editStartValue = Value; SetStepperVisibility(true); base.OnGotFocus(e); }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); _stepper = e.NameScope.Find<Control>("PART_StepperCell"); ValueHost = e.NameScope.Find<Control>("PART_ValueHost") ?? this.GetVisualDescendants().OfType<Control>().FirstOrDefault(x => x.Name == "PART_ValueHost"); SyncText(); SetStepperVisibility(IsFocused); }
    void SetStepperVisibility(bool visible)
    {
        _stepper ??= this.GetVisualDescendants().OfType<Control>().FirstOrDefault(x => x.Name == "PART_StepperCell");
        if (_stepper is not null) { _stepper.Opacity = IsEnabled && visible ? 1 : 0; _stepper.IsHitTestVisible = IsEnabled && visible; }
    }
    internal double EditStartValue => _editStartValue;
}
