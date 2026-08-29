using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYDatePicker
{
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    internal Border? CalendarSurfacePart { get; set; }
    internal void OnSurfacePointerPressed(object? sender, PointerEventArgs e) { var point = e.GetPosition(this); if (In(point, PreviousPart)) ChangeDays(-1); else if (In(point, NextPart)) ChangeDays(1); else if (In(point, CalendarButtonPart)) ToggleCalendar(); else foreach (var pair in SegmentButtons) if (In(point, pair.Value)) { ActivateSegment(pair.Key); break; } }
    bool In(Point point, Control? control) { if (control is null) return false; var origin = control.TranslatePoint(new Point(0, 0), this); return origin is not null && new Rect(origin.Value, control.Bounds.Size).Contains(point); }
    internal void OnCalendarSurfacePointerPressed(object? sender, PointerEventArgs e) { if (CalendarSurfacePart is null) return; foreach (var button in CalendarSurfacePart.GetVisualDescendants().OfType<Button>()) { var origin = button.TranslatePoint(new Point(0, 0), CalendarSurfacePart); if (origin is not null && new Rect(origin.Value, button.Bounds.Size).Contains(e.GetPosition(CalendarSurfacePart))) { button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); e.Handled = true; return; } } }
    internal void ToggleCalendar() { if (IsCalendarOpen) CloseCalendarForLifecycle(); else OpenCalendar(); }
    public void OpenCalendar()
    {
        if (!IsEnabled || PopupPart is null || CalendarContentPart is null) return; IsCalendarOpen = true; CalendarMonth = new DateOnly(SelectedDate.Year, SelectedDate.Month, 1); RefreshCalendar(); PopupPart.Height = double.NaN; PopupPart.IsVisible = true; PopupPart.PlacementTarget = this; PopupPart.IsOpen = true;
    }
    internal void CloseCalendarForLifecycle() { IsCalendarOpen = false; if (PopupPart is not null) { PopupPart.IsOpen = false; PopupPart.IsVisible = false; PopupPart.Height = 0; } }
    internal void RefreshCalendar() { if (CalendarContentPart is not null) CalendarContentPart.Content = BuildCalendar(); }
    Control BuildCalendar()
    {
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions { new ColumnDefinition(new GridLength(32)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(32)) } }; var previous = CalendarButton(XyuiVectorIcon.ChevronLeft, () => { CalendarMonth = CalendarMonth.AddMonths(-1); RefreshCalendar(); }); var next = CalendarButton(XyuiVectorIcon.ChevronRight, () => { CalendarMonth = CalendarMonth.AddMonths(1); RefreshCalendar(); }); header.Children.Add(previous); var title = new TextBlock { Text = $"{CalendarMonth.Year}年{CalendarMonth.Month}月", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; header.Children.Add(title); Grid.SetColumn(title, 1); header.Children.Add(next); Grid.SetColumn(next, 2);
        var days = new Grid { ColumnDefinitions = new ColumnDefinitions { new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)), new ColumnDefinition(new GridLength(1, GridUnitType.Star)) }, RowDefinitions = new RowDefinitions { new RowDefinition(new GridLength(24)), new RowDefinition(new GridLength(32)), new RowDefinition(new GridLength(32)), new RowDefinition(new GridLength(32)), new RowDefinition(new GridLength(32)), new RowDefinition(new GridLength(32)), new RowDefinition(new GridLength(32)) } }; var firstDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek; for (var i = 0; i < 7; i++) { var day = (DayOfWeek)(((int)firstDay + i) % 7); var label = new TextBlock { Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(day), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; days.Children.Add(label); Grid.SetColumn(label, i); }
        var offset = ((int)new DateTime(CalendarMonth.Year, CalendarMonth.Month, 1).DayOfWeek - (int)firstDay + 7) % 7; var total = DateTime.DaysInMonth(CalendarMonth.Year, CalendarMonth.Month); for (var i = 0; i < 42; i++) { var day = CalendarMonth.AddDays(i - offset); var cell = new Button { Content = day.Day.ToString(), Padding = new Thickness(0), MinWidth = 32, MinHeight = 32, Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), IsVisible = i >= offset && i < offset + total }; if (day == SelectedDate) cell.Classes.Add("xyui-date-selected"); cell.Click += (_, _) => { SelectedDate = day; CloseCalendarForLifecycle(); }; days.Children.Add(cell); Grid.SetColumn(cell, i % 7); Grid.SetRow(cell, i / 7 + 1); }
        return new StackPanel { Spacing = 4, Margin = new Thickness(8), Children = { header, days } };
    }
    static Button CalendarButton(XyuiVectorIcon icon, Action action) { var button = new Button { Width = 32, Height = 28, Padding = new Thickness(0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small } }; button.Click += (_, _) => action(); return button; }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnWindowDeactivated; _hostWindow.Closed += OnWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { CloseCalendarForLifecycle(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnWindowDeactivated; _hostWindow.Closed -= OnWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnDeactivated(object? sender, ActivatedEventArgs e) => CloseCalendarForLifecycle(); void OnWindowDeactivated(object? sender, EventArgs e) => CloseCalendarForLifecycle(); void OnWindowClosed(object? sender, EventArgs e) => CloseCalendarForLifecycle();
}
