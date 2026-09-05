using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] DatePickers() =>
    [
        DateSample("Default", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 9, 5) }),
        DateSample("Calendar Open", OpenDate(new DateOnly(2026, 9, 5))),
    ];

    static Control[] TimePickers() =>
    [
        TimeSample("Standard (HMS)", new XYTimePicker { Width = 260, Time = new TimeOnly(14, 30, 25), ShowSeconds = true }),
        TimeSample("Compact (HM)", new XYTimePicker { Width = 220, Time = new TimeOnly(9, 5), ShowSeconds = false }),
    ];

    static Control DateSample(string caption, XYDatePicker picker) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, picker } };
    static Control TimeSample(string caption, XYTimePicker picker) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, picker } };
    static XYDatePicker OpenDate(DateOnly date) { var picker = new XYDatePicker { Width = 300, SelectedDate = date }; picker.AttachedToVisualTree += (_, _) => picker.OpenCalendar(); return picker; }
}
