using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public enum XyuiProgressBarSize { Compact, Standard, Emphasis }
public enum XyuiProgressBarVariant { CleanLinear, Labeled, SegmentedStage, InlineCompact }

public sealed partial class XYProgressBar : Control
{
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<XYProgressBar, double>(nameof(Minimum), 0);
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<XYProgressBar, double>(nameof(Maximum), 100);
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<XYProgressBar, double>(nameof(Value));
    public static readonly StyledProperty<bool> IsIndeterminateProperty = AvaloniaProperty.Register<XYProgressBar, bool>(nameof(IsIndeterminate));
    public static readonly StyledProperty<bool> ShowPercentageProperty = AvaloniaProperty.Register<XYProgressBar, bool>(nameof(ShowPercentage), true);
    public static readonly StyledProperty<string?> StatusTextProperty = AvaloniaProperty.Register<XYProgressBar, string?>(nameof(StatusText));
    public static readonly StyledProperty<XyuiProgressBarSize> SizeProperty = AvaloniaProperty.Register<XYProgressBar, XyuiProgressBarSize>(nameof(Size), XyuiProgressBarSize.Standard);
    public static readonly StyledProperty<XyuiProgressBarVariant> VariantProperty = AvaloniaProperty.Register<XYProgressBar, XyuiProgressBarVariant>(nameof(Variant));
    public static readonly StyledProperty<IBrush?> TrackProperty = AvaloniaProperty.Register<XYProgressBar, IBrush?>(nameof(Track));
    public static readonly StyledProperty<IBrush?> FillProperty = AvaloniaProperty.Register<XYProgressBar, IBrush?>(nameof(Fill));
    readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
    bool _attached;
    double _indeterminateOffset;

    static XYProgressBar() => AffectsRender<XYProgressBar>(MinimumProperty, MaximumProperty, ValueProperty, IsIndeterminateProperty, TrackProperty, FillProperty);

    public XYProgressBar()
    {
        Classes.Add("xyui-4-component"); Classes.Add("xyui-progress-bar");
        Focusable = false; IsHitTestVisible = false; _timer.Tick += OnTick;
        AttachedToVisualTree += (_, _) => { _attached = true; UpdateTimer(); };
        DetachedFromVisualTree += (_, _) => { _attached = false; _timer.Stop(); };
        ApplySize(Size);
    }

    public string CanonicalId => "XYUI-4-4.16";
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, Finite(value)); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, Finite(value)); }
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, Clamp(value)); }
    public bool IsIndeterminate { get => GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }
    public bool ShowPercentage { get => GetValue(ShowPercentageProperty); set => SetValue(ShowPercentageProperty, value); }
    public string? StatusText { get => GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }
    public XyuiProgressBarSize Size { get => GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public XyuiProgressBarVariant Variant { get => GetValue(VariantProperty); set => SetValue(VariantProperty, value); }
    public IBrush? Track { get => GetValue(TrackProperty); set => SetValue(TrackProperty, value); }
    public IBrush? Fill { get => GetValue(FillProperty); set => SetValue(FillProperty, value); }
    public double ProgressFraction => Range <= 0 ? 0 : Math.Clamp((Value - Minimum) / Range, 0, 1);
    public int Percentage => (int)Math.Round(ProgressFraction * 100, MidpointRounding.AwayFromZero);
    double Range => Maximum - Minimum;
    static double Finite(double value) => double.IsFinite(value) ? value : 0;
    double Clamp(double value) => Math.Clamp(Finite(value), Math.Min(Minimum, Maximum), Math.Max(Minimum, Maximum));

    protected override Size MeasureOverride(Size available) => new(double.IsFinite(available.Width) ? available.Width : 160, HeightFor(Size));
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeProperty) ApplySize(change.GetNewValue<XyuiProgressBarSize>());
        if (change.Property == IsIndeterminateProperty || change.Property == Visual.IsVisibleProperty) UpdateTimer();
    }
    static double HeightFor(XyuiProgressBarSize size) => size switch { XyuiProgressBarSize.Compact => 4, XyuiProgressBarSize.Emphasis => 9, _ => 7 };
    static string ClassFor(XyuiProgressBarSize size) => $"xyui-progress-bar-{size.ToString().ToLowerInvariant()}";
    void ApplySize(XyuiProgressBarSize size) { foreach (var item in Enum.GetValues<XyuiProgressBarSize>()) Classes.Remove(ClassFor(item)); Classes.Add(ClassFor(size)); InvalidateMeasure(); InvalidateVisual(); }
    void UpdateTimer() { if (_attached && IsVisible && IsIndeterminate) _timer.Start(); else _timer.Stop(); }
    void OnTick(object? sender, EventArgs e) { _indeterminateOffset = (_indeterminateOffset + 0.018) % 1; InvalidateVisual(); }
}
