using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYDatePicker
{
    internal static FuncControlTemplate<XYDatePicker> CreateTemplate() => new((control, scope) =>
    {
        var segments = new StackPanel { Name = "PART_Segments", Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        foreach (var segment in control.SegmentOrder()) { if (segments.Children.Count > 0) segments.Children.Add(new TextBlock { Text = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(3, 0) }); segments.Children.Add(control.CreateSegment(segment, scope)); }
        var previous = StepButton(control, "PART_Previous", XyuiVectorIcon.ChevronLeft, -1); var next = StepButton(control, "PART_Next", XyuiVectorIcon.ChevronRight, 1); control.PreviousPart = previous; control.NextPart = next;
        var calendar = new Button { Name = "PART_CalendarButton", Width = 32, Padding = new Thickness(0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Content = new XYIcon { Icon = XyuiVectorIcon.Calendar, Size = XyuiIconSize.Small } };
        calendar.Click += (_, _) => { if (!control.ConsumePointerAction()) control.ToggleCalendar(); }; control.CalendarButtonPart = calendar; control.CalendarIconPart = calendar.Content as XYIcon; scope?.Register("PART_CalendarButton", calendar);
        var surface = new Grid { ColumnDefinitions = new ColumnDefinitions { new(32, GridUnitType.Pixel), new(1, GridUnitType.Star), new(32, GridUnitType.Pixel), new(32, GridUnitType.Pixel) }, Children = { previous, segments, next, calendar } };
        Grid.SetColumn(segments, 1); Grid.SetColumn(next, 2); Grid.SetColumn(calendar, 3);
        var chrome = new Border { Name = "PART_Chrome", Child = surface }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false }; var content = new ContentControl { Name = "PART_CalendarContent" }; var popupSurface = new Border { Name = "PART_CalendarSurface", Child = content }; var popup = new Popup { Name = "PART_Popup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = popupSurface };
        control.CalendarContentPart = content; control.PopupPart = popup; control.CalendarSurfacePart = popupSurface; popupSurface.AddHandler(InputElement.PointerPressedEvent, control.OnCalendarSurfacePointerPressed, RoutingStrategies.Bubble, true); scope?.Register("PART_Popup", popup); scope?.Register("PART_CalendarContent", content); scope?.Register("PART_CalendarSurface", popupSurface);
        popup.Closed += (_, _) => control.CloseCalendarForLifecycle(); return new Grid { Children = { chrome, edge, popup } };
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); PopupPart = e.NameScope.Find<Popup>("PART_Popup"); CalendarContentPart = e.NameScope.Find<ContentControl>("PART_CalendarContent"); RefreshCalendar(); SyncDateParts(); if (IsCalendarOpen) OpenCalendar(); }
    Button CreateSegment(XYDateSegment segment, INameScope? scope) { var button = new Button { Name = $"PART_{segment}", Content = SegmentText(segment), Padding = new Thickness(2, 0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Focusable = true, MinWidth = segment == XYDateSegment.Year ? 38 : 26 }; button.Click += (_, _) => ActivateSegment(segment); SegmentButtons[segment] = button; scope?.Register(button.Name!, button); return button; }
    static Button StepButton(XYDatePicker control, string name, XyuiVectorIcon icon, int days) { var button = new Button { Name = name, Width = 32, Padding = new Thickness(0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small } }; button.Click += (_, _) => { if (!control.ConsumePointerAction()) control.ChangeDays(days); }; return button; }
    internal IEnumerable<XYDateSegment> SegmentOrder() { var pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern; return pattern.IndexOf('y') < pattern.IndexOf('M') && pattern.IndexOf('M') < pattern.IndexOf('d') ? [XYDateSegment.Year, XYDateSegment.Month, XYDateSegment.Day] : pattern.IndexOf('d') < pattern.IndexOf('M') ? [XYDateSegment.Day, XYDateSegment.Month, XYDateSegment.Year] : [XYDateSegment.Month, XYDateSegment.Day, XYDateSegment.Year]; }
}
