using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYDatePicker
{
    DateOnly _datePopupStart;
    internal void OpenDatePopup(XYDateSegment segment)
    {
        if (!IsEnabled || DatePopupPart is null) return;
        if (!IsDatePopupOpen) _datePopupStart = SelectedDate;
        IsDatePopupOpen = true; ActivateSegment(segment); RefreshDatePopup(); DatePopupPart.Height = double.NaN; DatePopupPart.IsVisible = true; DatePopupPart.PlacementTarget = this; DatePopupPart.IsOpen = true;
    }
    internal void CloseDatePopupForLifecycle() { if (!IsDatePopupOpen && DatePopupPart?.IsOpen != true) return; CommitSegmentEdit(); IsDatePopupOpen = false; HideDatePopup(); }
    internal void CancelDatePopup() { if (!IsDatePopupOpen) return; CancelSegmentEdit(); SelectedDate = _datePopupStart; IsDatePopupOpen = false; HideDatePopup(); }
    internal void OnDatePopupClosed() { if (IsDatePopupOpen) CloseDatePopupForLifecycle(); }
    internal void RefreshDatePopup() { if (DatePopupSurfacePart is not null) DatePopupSurfacePart.Child = BuildDatePopup(); }
    void HideDatePopup() { if (DatePopupPart is null) return; DatePopupPart.IsOpen = false; DatePopupPart.IsVisible = false; DatePopupPart.Height = 0; }
    Control BuildDatePopup()
    {
        var panel = new StackPanel { Spacing = 6, Margin = new Thickness(10), MinWidth = 220 };
        panel.Children.Add(new TextBlock { Text = "调整日期", Classes = { "xyui-text-section" } });
        panel.Children.Add(DateRow(XYDateSegment.Year, "年")); panel.Children.Add(DateRow(XYDateSegment.Month, "月")); panel.Children.Add(DateRow(XYDateSegment.Day, "日"));
        panel.Children.Add(new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 6, Children = { PopupButton("取消", CancelDatePopup), PopupButton("完成", CloseDatePopupForLifecycle) } }); return new Border { Child = panel };
    }
    Control DateRow(XYDateSegment segment, string label)
    {
        var down = PopupButton("−", () => AdjustDateSegment(segment, -1)); down.Tag = $"{segment}-减少"; var up = PopupButton("+", () => AdjustDateSegment(segment, 1)); up.Tag = $"{segment}-增加"; var value = new TextBlock { Text = SegmentText(segment), Width = 54, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; return new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { new TextBlock { Text = label, Width = 24, VerticalAlignment = VerticalAlignment.Center }, down, value, up } };
    }
    void AdjustDateSegment(XYDateSegment segment, int amount) { CommitSegmentEdit(); ActiveSegment = segment; SelectedDate = segment switch { XYDateSegment.Year => SafeYear(amount), XYDateSegment.Month => SelectedDate.AddMonths(amount), _ => SelectedDate.AddDays(amount) }; BeginSegmentEdit(segment); RefreshDatePopup(); }
    static Button PopupButton(string text, Action action) { var button = new Button { Content = text, MinWidth = 28, MinHeight = 28, Padding = new Thickness(4, 0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0) }; button.Click += (_, _) => action(); return button; }
}
