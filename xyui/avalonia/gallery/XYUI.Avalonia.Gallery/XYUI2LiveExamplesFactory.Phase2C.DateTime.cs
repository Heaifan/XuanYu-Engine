using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control DatePickerExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYDatePicker { Width = 320, SelectedDate = new DateOnly(2026, 9, 5) });
        col1.Children.Add(new XYDatePicker { Width = 320, SelectedDate = new DateOnly(2026, 12, 31) });

        var col2 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYDatePicker { Width = 320, SelectedDate = new DateOnly(2028, 2, 29), MinDate = new DateOnly(2028, 1, 1), MaxDate = new DateOnly(2028, 12, 31) });
        col2.Children.Add(new XYDatePicker { Width = 320, SelectedDate = new DateOnly(2026, 9, 5), IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 项目排期与发布 (年月日分段键盘编辑 / 步进换日 / 右侧日历面板)", col1),
            Scene("场景 2 · 闰年与区间边界约束 (Min/Max Clamp 限制 / 禁用态)", col2));
    }

    static Control TimePickerExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYTimePicker { Width = 260, Time = new TimeOnly(14, 30, 25), ShowSeconds = true });
        col1.Children.Add(new XYTimePicker { Width = 260, Time = new TimeOnly(0, 0, 0), ShowSeconds = true });

        var col2 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYTimePicker { Width = 220, Time = new TimeOnly(9, 15), ShowSeconds = false });
        col2.Children.Add(new XYTimePicker { Width = 220, Time = new TimeOnly(18, 0), ShowSeconds = false, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 物理步长与关键帧时间 (时分秒三段 / 左右拖拽微调 Scrub / 调节面板)", col1),
            Scene("场景 2 · 日常日程时间 (紧凑时分模式 ShowSeconds=false / 禁用态)", col2));
    }
}
