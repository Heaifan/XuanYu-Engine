using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] NumberProperties() =>
    [
        new XYNumberProperty { Width = 520, Label = "最大速度", Value = 8.42, Minimum = 0, Maximum = 100, Step = .1, DecimalPlaces = 2, Suffix = "m/s" },
        new XYNumberProperty { Width = 520, Label = "质量", Value = 12.50, Minimum = 0, Maximum = 1000, Step = .5, DecimalPlaces = 2, Suffix = "kg" },
        new XYNumberProperty { Width = 520, Label = "透明度", Value = 55, Minimum = 0, Maximum = 100, Step = 1, DecimalPlaces = 0, Suffix = "%" },
        new XYNumberProperty { Width = 520, Label = "禁用的数值属性", Value = 12, IsEnabled = false },
        new XYNumberProperty { Width = 520, Label = "只读的数值属性", Value = 12, IsReadOnly = true },
        Hint("交互提示", "点击右侧输入框 → 精确编辑；↑ / ↓ → 按统一步长调整\n拖动左侧属性名称 → 连续微调；普通点击名称不改变值\n禁用 / 只读 → 阻断修改；单位后缀只参与显示"),
    ];

    static Control[] VectorProperties() =>
    [
        Sample("宽布局 · 位置", new XYVectorProperty { Width = 620, Label = "位置", Dimension = XYVectorDimension.Vector3, X = 12.5, Y = 0, Z = -4.8, Step = .1, DecimalPlaces = 2 }),
        Sample("中布局 · 旋转", new XYVectorProperty { Width = 420, Label = "旋转", Dimension = XYVectorDimension.Vector3, X = 0, Y = 90, Z = 0, Step = 1, DecimalPlaces = 0 }),
        Sample("紧凑布局 · 尺寸", new XYVectorProperty { Width = 240, Label = "尺寸", Dimension = XYVectorDimension.Vector2, X = 128, Y = 64, Step = 1, DecimalPlaces = 0 }),
        Sample("四轴向量", new XYVectorProperty { Width = 620, Label = "方向与权重", Dimension = XYVectorDimension.Vector4, X = 1, Y = 0, Z = 0, W = 1 }),
        new XYVectorProperty { Width = 420, Label = "禁用的向量", Dimension = XYVectorDimension.Vector3, IsEnabled = false },
        Hint("响应式与交互", "Wide → horizontal · Medium → label stacked · Compact → axes vertical\nAxes use real XYNumberField"),
    ];

    static Control[] EnumProperties() =>
    [
        new XYEnumProperty { Width = 520, Label = "渲染模式", ItemsSource = new[] { "实体", "线框", "点" }, SelectedIndex = 0 },
        new XYEnumProperty { Width = 520, Label = "道路等级", ItemsSource = new[] { "支路", "次干道", "主干道" }, SelectedIndex = 2 },
        new XYEnumProperty { Width = 520, Label = "质量等级", ItemsSource = new[] { "低", "中", "高" }, SelectedIndex = 1 },
        new XYEnumProperty { Width = 520, Label = "禁用的枚举", ItemsSource = new[] { "启用", "停用" }, SelectedIndex = 0, IsReadOnly = true },
        Hint("交互提示", "右侧直接复用 XYSelect；点击值区或 Chevron → 打开候选 Popup\n键盘 Enter / Space / ↑ / ↓ → 使用 XYSelect 原生选择交互\n点击外部、Esc、禁用或只读 → 按选择框生命周期处理"),
    ];

    static Control[] ReferenceProperties()
    {
        var resolved = ReferenceSample("实体引用", new("Infantry_023", "Entity", "E023"), "Entity");
        var dataset = ReferenceSample("数据集引用", new("道路数据", "Dataset", "Road-01"), "Dataset");
        return [resolved, dataset, ReferenceSample("空引用", null, "Entity"), ReferenceSample("丢失引用", new("Infantry_031", "Entity", "E031"), "Entity", XYReferenceState.Missing), ReferenceSample("类型不匹配", new("道路数据", "Dataset", "Road-01"), "Entity", XYReferenceState.TypeMismatch), ReferenceSample("紧凑引用", new("Tank_004", "Entity", "E004"), "Entity", XYReferenceState.Resolved, 280), Hint("交互提示", "定位 → 触发真实 LocateRequested 并显示反馈；浏览 → 打开真实引用选择 Popup\n单击候选 → 更新引用并关闭；点击外部或 Esc → 关闭；清除 → Empty\nEmpty、引用丢失、类型不匹配使用不同文字语义；拖入不兼容类型时保留原引用")];
    }

    static Control ReferenceSample(string caption, XYReferenceValue? value, string expected, XYReferenceState state = XYReferenceState.Resolved, double width = 520)
    {
        var list = new ListBox { ItemsSource = new[] { new XYReferenceValue("Infantry_023", "Entity", "E023"), new XYReferenceValue("Infantry_031", "Entity", "E031"), new XYReferenceValue("Tank_004", "Entity", "E004") }, MinWidth = 220 };
        var property = new XYReferenceProperty { Width = width, Label = caption, Reference = value, ExpectedType = expected, ReferenceState = state, ReferencePickerContent = list };
        var feedback = new TextBlock { Text = "" }; property.LocateRequested += (_, _) => feedback.Text = "已定位：" + property.ReferenceName; property.BrowseRequested += (_, _) => feedback.Text = "已打开引用选择器"; property.ReferenceChanged += (_, _) => feedback.Text = "引用已更新：" + property.ReferenceName;
        return new StackPanel { Spacing = 3, Children = { property, feedback } };
    }

    static Control Sample(string caption, Control control) => new StackPanel { Spacing = 3, Children = { new XYCaption { Text = caption }, control } };
    static Control Hint(string title, string text) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = title }, new TextBlock { Text = text } } };
}
