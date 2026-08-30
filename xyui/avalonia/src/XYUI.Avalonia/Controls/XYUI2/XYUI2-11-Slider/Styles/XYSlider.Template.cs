using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYSlider
{
    internal static FuncControlTemplate<XYSlider> CreateTemplate() => new((control, scope) =>
    {
        var track = new XYSliderTrack { Name = "PART_Track", VerticalAlignment = VerticalAlignment.Center };
        var slider = new Slider { Name = "PART_Slider", Background = null, Opacity = 0, VerticalAlignment = VerticalAlignment.Stretch };
        var number = new XYNumberField { Name = "PART_NumberField", Height = 30, VerticalAlignment = VerticalAlignment.Center };
        var gap = new Border { Name = "PART_Gap" };
        scope?.Register("PART_Track", track); scope?.Register("PART_Slider", slider); scope?.Register("PART_NumberField", number); scope?.Register("PART_Gap", gap);
        var area = new Grid { MinHeight = 44, Children = { track, slider } };
        var root = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,104"), Children = { area, gap, number } };
        Grid.SetColumn(gap, 1); Grid.SetColumn(number, 2); return root;
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (SliderPart is not null) SliderPart.PropertyChanged -= OnSliderChanged;
        if (NumberFieldPart is not null) NumberFieldPart.PropertyChanged -= OnNumberChanged;
        base.OnApplyTemplate(e); TrackPart = e.NameScope.Find<XYSliderTrack>("PART_Track"); SliderPart = e.NameScope.Find<Slider>("PART_Slider"); NumberFieldPart = e.NameScope.Find<XYNumberField>("PART_NumberField");
        if (SliderPart is null || NumberFieldPart is null) return;
        SliderPart.PropertyChanged += OnSliderChanged; NumberFieldPart.PropertyChanged += OnNumberChanged;
        SliderPart.PointerPressed += (_, _) => SetDragging(true); SliderPart.PointerReleased += (_, _) => SetDragging(false); SliderPart.PointerCaptureLost += (_, _) => SetDragging(false); SyncParts();
    }

    void OnSliderChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == RangeBase.ValueProperty) Value = SliderPart!.Value; }
    void OnNumberChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == XYNumberField.ValueProperty) Value = NumberFieldPart!.Value; }
    void SetDragging(bool value) { if (TrackPart is not null) TrackPart.IsDragging = value; }
    void SyncParts()
    {
        SliderPart!.Minimum = Minimum; SliderPart.Maximum = Maximum; SliderPart.Value = Value; SliderPart.SmallChange = Step; SliderPart.LargeChange = LargeStep;
        NumberFieldPart!.Minimum = Minimum; NumberFieldPart.Maximum = Maximum; NumberFieldPart.Value = Value; NumberFieldPart.Step = Step; NumberFieldPart.LargeStep = LargeStep; NumberFieldPart.SmallStep = SmallStep; NumberFieldPart.DecimalPlaces = DecimalPlaces; NumberFieldPart.Suffix = Suffix; NumberFieldPart.IsVisible = IsNumberFieldVisible;
        TrackPart!.Minimum = Minimum; TrackPart.Maximum = Maximum; TrackPart.Value = Value;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty || change.Property == MinimumProperty || change.Property == MaximumProperty || change.Property == StepProperty || change.Property == LargeStepProperty || change.Property == SmallStepProperty || change.Property == DecimalPlacesProperty || change.Property == SuffixProperty || change.Property == IsNumberFieldVisibleProperty)
            if (SliderPart is not null) SyncParts();
    }
}
