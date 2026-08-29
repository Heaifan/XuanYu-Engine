using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;
namespace XYUI.Avalonia.Gallery;
public static partial class XYUI2GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-2-01" => Host("XY.Button · Primary / Secondary / Danger / Disabled", Buttons()),
        "XYUI-2-02" => Host("XY.IconButton · Default Ghost / Hover(交互) / Selected / Disabled", IconButtons()),
        "XYUI-2-03" => Host("XY.ToggleButton · OFF / ON(Persistent Edge) / Disabled", Toggles()),
        "XYUI-2-04" => Host("XY.SplitButton · Compact Icon Well / Default / Hover / Pressed / Disabled", Splits()),
        "XYUI-2-05" => Host("XY.DropDownButton · Chevron Track / Default / Hover / Pressed / Disabled", DropDowns()),
        "XYUI-2-06" => Host("XY.Checkbox · Unchecked / Checked / Mixed / Disabled", Checkboxes()),
        "XYUI-2-07" => Host("XY.RadioButton · 渲染模式 / 坐标空间", RadioButtons()),
        "XYUI-2-08" => Host("XY.Switch · Compact Track / ON / OFF / Disabled", Switches()),
        "XYUI-2-09" => Host("XY.TextField · Default / Placeholder / Focus / ReadOnly / Disabled / Error", TextFields()),
        "XYUI-2-10" => Host("XY.NumberField · Stepper / Keyboard / Scrub", NumberFields()),
        "XYUI-2-11" => Host("XY.Slider · 透明度 / 强度 / 时间倍率", Sliders()),
        "XYUI-2-12" => Host("XY.ComboBox · 可编辑 / 可搜索候选", ComboBoxes()),
        "XYUI-2-13" => Host("XY.Select · 固定候选 / 不可输入", Selects()),
        "XYUI-2-14" => Host("XY.TextArea · 标准 / 编辑 / 自动增长", TextAreas()),
        "XYUI-2-15" => Host("搜索框 · 清除 / 筛选", SearchFields()),
        "XYUI-2-16" => Host("密码输入框 · 按住显示", PasswordFields()),
        "XYUI-2-17" => Host("日期选择器 · 分段编辑 / 日历面板", DatePickers()),
        "XYUI-2-18" => Host("时间选择器 · 分段编辑 / 横向微调", TimePickers()),
        "XYUI-2-19" => Host("颜色选择器 · RGB / RGBA / 透明度", ColorPickers()),
        "XYUI-2-20" => Host("布尔属性 · 属性行 / 开关复用", BoolProperties()),
        "XYUI-2-21" => Host("数值属性行 · 标签微调 / 数值输入", NumberProperties()),
        "XYUI-2-22" => HostVertical("向量属性控件 · 自适应轴布局", VectorProperties()),
        "XYUI-2-23" => Host("枚举属性控件 · 选择框复用", EnumProperties()),
        "XYUI-2-24" => Host("引用属性控件 · 身份 / 定位 / 浏览", ReferenceProperties()),
        _ => new TextBlock { Text = "未实装组件（Batch 02+）" }
    };
    static StackPanel Host(string title, Control[] samples)
    {
        var panel = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var sample in samples)
        {
            sample.Margin = new Thickness(0, 0, 8, 0);
            panel.Children.Add(sample);
        }
        return new StackPanel { Spacing = 8, Children = { new XYCaption { Text = title }, panel } };
    }
    static StackPanel HostVertical(string title, Control[] samples)
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };
        foreach (var sample in samples) { sample.Margin = new Thickness(0, 0, 0, 8); panel.Children.Add(sample); }
        return new StackPanel { Spacing = 8, Children = { new XYCaption { Text = title }, panel } };
    }
}
