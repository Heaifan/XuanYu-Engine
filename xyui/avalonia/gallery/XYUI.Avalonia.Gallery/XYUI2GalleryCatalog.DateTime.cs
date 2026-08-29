using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] DatePickers() =>
    [
        DateSample("默认日期", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 8, 12) }),
        DateSample("年份分段激活", ActiveDate(XYDateSegment.Year, new DateOnly(2026, 8, 12))),
        DateSample("月份分段激活", ActiveDate(XYDateSegment.Month, new DateOnly(2026, 8, 12))),
        DateSample("日期分段激活", ActiveDate(XYDateSegment.Day, new DateOnly(2026, 8, 12))),
        DateSample("前一天 / 后一天", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 12, 31) }),
        DateSample("日历面板", OpenDate(new DateOnly(2026, 8, 12))),
        DateSample("月份边界", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 12, 31) }),
        DateSample("闰日", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2028, 2, 29) }),
        DateSample("禁用", new XYDatePicker { Width = 300, SelectedDate = new DateOnly(2026, 8, 12), IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "点击年 / 月 / 日 → 打开对应调节面板\n调节面板可用加减按钮修改对应分段；完成保存，取消恢复\n数字键 → 精确替换当前分段\n↑ / ↓ → 调整当前分段\n← / → → 切换分段\n前一天 / 后一天 → 调整一天\n日历图标 → 打开日历面板；点击日期或 Esc → 关闭\n禁用 → 保留日期显示并阻断交互" } } },
    ];

    static Control[] TimePickers() =>
    [
        TimeSample("时分秒", new XYTimePicker { Width = 220, Time = new TimeOnly(14, 30, 25), ShowSeconds = true }),
        TimeSample("时分", new XYTimePicker { Width = 220, Time = new TimeOnly(9, 5), ShowSeconds = false }),
        TimeSample("小时分段激活", ActiveTime(XYTimeSegment.Hour)),
        TimeSample("分钟分段激活", ActiveTime(XYTimeSegment.Minute)),
        TimeSample("秒分段激活", ActiveTime(XYTimeSegment.Second)),
        TimeSample("禁用", new XYTimePicker { Width = 220, Time = new TimeOnly(22, 10, 5), IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "点击时钟图标或时 / 分 / 秒 → 打开调整时间面板\n调整面板可用加减按钮修改当前时间；完成保存，取消恢复\n数字键 → 精确替换当前分段\n↑ / ↓ → 仅调整当前分段并循环\n← / → → 切换可见分段\n按住分段左右拖动 → 连续微调；右增左减\n秒分段隐藏时不占空间、不参与键盘切换\n禁用 → 结束微调并阻断交互" } } },
    ];

    static Control DateSample(string caption, XYDatePicker picker) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, picker } };
    static Control TimeSample(string caption, XYTimePicker picker) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, picker } };
    static XYDatePicker ActiveDate(XYDateSegment segment, DateOnly date) { var picker = new XYDatePicker { Width = 300, SelectedDate = date }; picker.AttachedToVisualTree += (_, _) => picker.ActivateSegment(segment); return picker; }
    static XYDatePicker OpenDate(DateOnly date) { var picker = new XYDatePicker { Width = 300, SelectedDate = date }; picker.AttachedToVisualTree += (_, _) => picker.OpenCalendar(); return picker; }
    static XYTimePicker ActiveTime(XYTimeSegment segment) { var picker = new XYTimePicker { Width = 220, Time = new TimeOnly(14, 30, 25), ShowSeconds = true }; picker.AttachedToVisualTree += (_, _) => picker.ActivateSegment(segment); return picker; }
}
