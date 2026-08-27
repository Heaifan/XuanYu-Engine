using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public class XYSlider : Grid
{
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Value));
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Minimum));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<double> StepProperty = AvaloniaProperty.Register<XYSlider, double>(nameof(Step), 1);
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum)); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }
    readonly Slider _slider; readonly XYNumberField _number;
    public XYSlider()
    {
        Classes.Add("xyui-slider"); ColumnDefinitions = new ColumnDefinitions("*,88"); MinHeight = 44;
        _slider = new Slider { Minimum = Minimum, Maximum = Maximum, VerticalAlignment = VerticalAlignment.Center };
        _number = new XYNumberField { Minimum = Minimum, Maximum = Maximum, Step = Step, Height = 30 };
        _slider.PropertyChanged += (_, e) => { if (e.Property == RangeBase.ValueProperty && e.NewValue is double v) Value = v; };
        _number.PropertyChanged += (_, e) => { if (e.Property == XYNumberField.ValueProperty && e.NewValue is double v) Value = v; };
        PropertyChanged += (_, e) => { if (e.Property == ValueProperty) { _slider.Value = Value; _number.Value = Value; } };
        Children.Add(_slider); Grid.SetColumn(_number, 1); Children.Add(_number);
    }
}
