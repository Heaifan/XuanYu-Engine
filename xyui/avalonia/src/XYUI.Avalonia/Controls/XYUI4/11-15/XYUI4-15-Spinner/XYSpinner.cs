using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public enum XyuiSpinnerSize { Compact, Standard, Large }

public sealed partial class XYSpinner : Control
{
    public static readonly StyledProperty<XyuiSpinnerSize> SizeProperty =
        AvaloniaProperty.Register<XYSpinner, XyuiSpinnerSize>(nameof(Size), XyuiSpinnerSize.Standard);
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<XYSpinner, bool>(nameof(IsActive), true);
    public static readonly StyledProperty<bool> IsReducedMotionProperty =
        AvaloniaProperty.Register<XYSpinner, bool>(nameof(IsReducedMotion));
    public static readonly StyledProperty<IBrush?> TrackProperty =
        AvaloniaProperty.Register<XYSpinner, IBrush?>(nameof(Track));
    public static readonly StyledProperty<IBrush?> ArcProperty =
        AvaloniaProperty.Register<XYSpinner, IBrush?>(nameof(Arc));

    readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
    bool _attached;
    double _angle;

    static XYSpinner()
    {
        AffectsRender<XYSpinner>(TrackProperty, ArcProperty);
    }

    public XYSpinner()
    {
        Classes.Add("xyui-4-component"); Classes.Add("xyui-spinner");
        Focusable = false; IsHitTestVisible = false;
        _timer.Tick += OnTick;
        AttachedToVisualTree += (_, _) => { _attached = true; UpdateTimer(); };
        DetachedFromVisualTree += (_, _) => { _attached = false; _timer.Stop(); };
        ApplySize(Size);
    }

    public string CanonicalId => "XYUI-4-4.15";
    public XyuiSpinnerSize Size { get => GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool IsReducedMotion { get => GetValue(IsReducedMotionProperty); set => SetValue(IsReducedMotionProperty, value); }
    public IBrush? Track { get => GetValue(TrackProperty); set => SetValue(TrackProperty, value); }
    public IBrush? Arc { get => GetValue(ArcProperty); set => SetValue(ArcProperty, value); }
    public double CurrentAngleDegrees => _angle;

    protected override Size MeasureOverride(Size available) => new(SizeFor(Size), SizeFor(Size));

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeProperty) ApplySize(change.GetNewValue<XyuiSpinnerSize>());
        if (change.Property == IsActiveProperty || change.Property == IsReducedMotionProperty ||
            change.Property == Visual.IsVisibleProperty) UpdateTimer();
    }

    void ApplySize(XyuiSpinnerSize value)
    {
        foreach (var name in Enum.GetNames<XyuiSpinnerSize>()) Classes.Remove($"xyui-spinner-{name.ToLowerInvariant()}");
        Classes.Add($"xyui-spinner-{value.ToString().ToLowerInvariant()}"); InvalidateMeasure(); InvalidateVisual();
    }

    void UpdateTimer()
    {
        if (_attached && IsVisible && IsActive && !IsReducedMotion) _timer.Start(); else _timer.Stop();
    }

    void OnTick(object? sender, EventArgs e) { _angle = (_angle + 11.88) % 360; InvalidateVisual(); }
}
