using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Controls;

public partial class XYSlider : TemplatedControl
{
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Value));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Minimum));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<double> StepProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Step), 1);
    public static readonly StyledProperty<double> LargeStepProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(LargeStep), 10);
    public static readonly StyledProperty<double> SmallStepProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(SmallStep), .1);
    public static readonly StyledProperty<int> DecimalPlacesProperty = AvaloniaProperty.Register<XYSlider, int>(nameof(DecimalPlaces), 2);
    public static readonly StyledProperty<string?> SuffixProperty = AvaloniaProperty.Register<XYSlider, string?>(nameof(Suffix));
    public static readonly StyledProperty<bool> IsNumberFieldVisibleProperty = AvaloniaProperty.Register<XYSlider, bool>(nameof(IsNumberFieldVisible), true);
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum)); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public double LargeStep { get => GetValue(LargeStepProperty); set => SetValue(LargeStepProperty, value); }
    public double SmallStep { get => GetValue(SmallStepProperty); set => SetValue(SmallStepProperty, value); }
    public int DecimalPlaces { get => GetValue(DecimalPlacesProperty); set => SetValue(DecimalPlacesProperty, Math.Max(0, value)); }
    public string? Suffix { get => GetValue(SuffixProperty); set => SetValue(SuffixProperty, value); }
    public bool IsNumberFieldVisible { get => GetValue(IsNumberFieldVisibleProperty); set => SetValue(IsNumberFieldVisibleProperty, value); }
    internal Slider? SliderPart { get; private set; }
    internal XYNumberField? NumberFieldPart { get; private set; }
    internal XYSliderTrack? TrackPart { get; private set; }
    public XYSlider() => Classes.Add("xyui-slider");

}
