using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public enum XYDateSegment { Year, Month, Day }

public partial class XYDatePicker : TemplatedControl
{
    public static readonly StyledProperty<DateOnly> SelectedDateProperty = AvaloniaProperty.Register<XYDatePicker, DateOnly>(nameof(SelectedDate), new DateOnly(2026, 8, 12));
    public static readonly StyledProperty<DateOnly?> MinDateProperty = AvaloniaProperty.Register<XYDatePicker, DateOnly?>(nameof(MinDate));
    public static readonly StyledProperty<DateOnly?> MaxDateProperty = AvaloniaProperty.Register<XYDatePicker, DateOnly?>(nameof(MaxDate));
    public DateOnly SelectedDate { get => GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, Clamp(value)); }
    public DateOnly? MinDate { get => GetValue(MinDateProperty); set => SetValue(MinDateProperty, value); }
    public DateOnly? MaxDate { get => GetValue(MaxDateProperty); set => SetValue(MaxDateProperty, value); }
    public event EventHandler? DateChanged;
    internal readonly Dictionary<XYDateSegment, Button> SegmentButtons = [];
    internal Popup? PopupPart { get; private set; }
    internal Popup? DatePopupPart { get; set; }
    internal Border? DatePopupSurfacePart { get; set; }
    internal ContentControl? CalendarContentPart { get; private set; }
    internal XYIcon? CalendarIconPart { get; private set; }
    internal Button? CalendarButtonPart { get; set; }
    internal Button? PreviousPart { get; set; }
    internal Button? NextPart { get; set; }
    internal XYDateSegment ActiveSegment { get; private set; } = XYDateSegment.Day;
    internal DateOnly CalendarMonth { get; set; }
    internal bool IsCalendarOpen { get; private set; }
    internal bool IsDatePopupOpen { get; private set; }
    internal bool IsSegmentEditing { get; private set; }
    internal string EditBuffer { get; private set; } = "";
    internal bool PointerActionPending { get; set; }
    internal bool PopupPointerActionPending { get; set; }
    internal bool PopupSyntheticClick { get; set; }
    DateOnly _editStartDate;
    public XYDatePicker() { Classes.Add("xyui-date-picker"); Focusable = true; CalendarMonth = new DateOnly(2026, 8, 1); }
    protected override void OnKeyDown(KeyEventArgs e) { OnDateKeyDown(e); if (!e.Handled) base.OnKeyDown(e); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedDateProperty) { var date = Clamp(change.GetNewValue<DateOnly>()); if (date != change.GetNewValue<DateOnly>()) SetValue(SelectedDateProperty, date); SyncDateParts(); DateChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == MinDateProperty || change.Property == MaxDateProperty) SelectedDate = SelectedDate;
        if (change.Property == IsEnabledProperty && !IsEnabled) CloseCalendarForLifecycle();
    }
    public void ActivateSegment(XYDateSegment segment) { if (!IsEnabled) return; CommitSegmentEdit(); ActiveSegment = segment; BeginSegmentEdit(segment); SegmentButtons.GetValueOrDefault(segment)?.Focus(); }
    internal void BeginSegmentEdit(XYDateSegment segment) { ActiveSegment = segment; _editStartDate = SelectedDate; EditBuffer = ""; IsSegmentEditing = true; Classes.Set("xyui-date-editing", true); SyncSegmentClasses(); }
    internal void CommitSegmentEdit() { if (!IsSegmentEditing) return; if (EditBuffer.Length == SegmentWidth() && int.TryParse(EditBuffer, out var value) && TryBuild(value, out var date)) SelectedDate = date; IsSegmentEditing = false; EditBuffer = ""; Classes.Set("xyui-date-editing", false); SyncSegmentClasses(); }
    internal void CancelSegmentEdit() { if (!IsSegmentEditing) return; SelectedDate = _editStartDate; IsSegmentEditing = false; EditBuffer = ""; Classes.Set("xyui-date-editing", false); SyncSegmentClasses(); }
    internal int SegmentWidth() => ActiveSegment == XYDateSegment.Year ? 4 : 2;
    internal void ChangeDays(int days) => SelectedDate = SelectedDate.AddDays(days);
    internal void SyncSegmentClasses() { foreach (var segment in Enum.GetValues<XYDateSegment>()) Classes.Set($"xyui-date-{segment.ToString().ToLowerInvariant()}-active", ActiveSegment == segment); }
    internal DateOnly Clamp(DateOnly date) => MinDate is { } min && date < min ? min : MaxDate is { } max && date > max ? max : date;
    internal void SyncDateParts() { foreach (var pair in SegmentButtons) pair.Value.Content = SegmentText(pair.Key); SyncSegmentClasses(); }
    internal string SegmentText(XYDateSegment segment) => segment switch { XYDateSegment.Year => SelectedDate.Year.ToString("0000"), XYDateSegment.Month => SelectedDate.Month.ToString("00"), _ => SelectedDate.Day.ToString("00") };
}
