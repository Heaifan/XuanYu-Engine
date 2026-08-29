using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Controls;

public enum XYTimeSegment { Hour, Minute, Second }

public partial class XYTimePicker : TemplatedControl
{
    public static readonly StyledProperty<TimeOnly> TimeProperty = AvaloniaProperty.Register<XYTimePicker, TimeOnly>(nameof(Time), new TimeOnly(14, 30, 25));
    public static readonly StyledProperty<bool> ShowSecondsProperty = AvaloniaProperty.Register<XYTimePicker, bool>(nameof(ShowSeconds), true);
    public TimeOnly Time { get => GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
    public bool ShowSeconds { get => GetValue(ShowSecondsProperty); set => SetValue(ShowSecondsProperty, value); }
    public event EventHandler? TimeChanged;
    internal readonly Dictionary<XYTimeSegment, Button> SegmentButtons = [];
    internal Button? SecondSeparatorPart { get; set; }
    internal XYIcon? ScrubIndicatorPart { get; set; }
    internal XYTimeSegment ActiveSegment { get; private set; } = XYTimeSegment.Minute;
    internal bool IsScrubArmed { get; set; }
    internal bool IsScrubbing { get; set; }
    internal XYTimeSegment ScrubSegment { get; set; }
    public XYTimePicker() { Classes.Add("xyui-time-picker"); Focusable = true; AddHandler(InputElement.PointerMovedEvent, OnTimePointerMoved, RoutingStrategies.Bubble, true); AddHandler(InputElement.PointerReleasedEvent, OnTimePointerReleased, RoutingStrategies.Bubble, true); AddHandler(InputElement.PointerCaptureLostEvent, OnTimePointerCaptureLost, RoutingStrategies.Bubble, true); }
    protected override void OnKeyDown(KeyEventArgs e) { OnTimeKeyDown(e); if (!e.Handled) base.OnKeyDown(e); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TimeProperty) { SyncTimeParts(); TimeChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == ShowSecondsProperty) SyncTimeParts();
        if (change.Property == IsEnabledProperty && !IsEnabled) CancelScrub();
    }
    public void ActivateSegment(XYTimeSegment segment) { if (!IsEnabled || segment == XYTimeSegment.Second && !ShowSeconds) return; ActiveSegment = segment; SyncSegmentClasses(); SegmentButtons.GetValueOrDefault(segment)?.Focus(); }
    internal XYTimeSegment[] SegmentOrder() => ShowSeconds ? [XYTimeSegment.Hour, XYTimeSegment.Minute, XYTimeSegment.Second] : [XYTimeSegment.Hour, XYTimeSegment.Minute];
    internal void SyncSegmentClasses() { foreach (var segment in Enum.GetValues<XYTimeSegment>()) Classes.Set($"xyui-time-{segment.ToString().ToLowerInvariant()}-active", ActiveSegment == segment && (segment != XYTimeSegment.Second || ShowSeconds)); }
    internal string SegmentText(XYTimeSegment segment) => segment switch { XYTimeSegment.Hour => Time.Hour.ToString("00"), XYTimeSegment.Minute => Time.Minute.ToString("00"), _ => Time.Second.ToString("00") };
    internal void SyncTimeParts() { foreach (var pair in SegmentButtons) { pair.Value.Content = SegmentText(pair.Key); pair.Value.IsVisible = pair.Key != XYTimeSegment.Second || ShowSeconds; } if (SecondSeparatorPart is not null) SecondSeparatorPart.IsVisible = ShowSeconds; SyncSegmentClasses(); }
    internal void SetSegment(XYTimeSegment segment, int value) { var hour = Time.Hour; var minute = Time.Minute; var second = Time.Second; if (segment == XYTimeSegment.Hour) hour = Wrap(value, 24); if (segment == XYTimeSegment.Minute) minute = Wrap(value, 60); if (segment == XYTimeSegment.Second) second = Wrap(value, 60); Time = new TimeOnly(hour, minute, second); }
    internal static int Wrap(int value, int length) => (value % length + length) % length;
}
