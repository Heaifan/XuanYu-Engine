using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] TextFields() => [
        new XYTextField { Width = 200, Text = "Northern Region" },
        new XYTextField { Width = 200, Placeholder = "输入名称" },
        new XYTextField { Width = 200, Text = "Northern Region" },
        new XYTextField { Width = 200, Text = "只读属性", IsReadOnly = true },
        new XYTextField { Width = 200, Text = "禁用输入", IsEnabled = false },
        new XYTextField { Width = 200, Text = "Invalid Path", IsError = true }];
    static Control[] NumberFields() => [
        NumberSample("Default", new XYNumberField { Width = 220, Value = 125, Maximum = 200 }),
        NumberSample("Hover / Stepper", new XYNumberField { Width = 220, Value = 50, Step = 5 }),
        NumberSample("Focus", new XYNumberField { Width = 220, Value = 25 }),
        NumberSample("Disabled", new XYNumberField { Width = 220, Value = 50, IsEnabled = false }),
        NumberSample("Min / Max", new XYNumberField { Width = 220, Value = 0, Minimum = 0, Maximum = 100 }),
        NumberSample("Suffix", new XYNumberField { Width = 220, Value = 72, Suffix = "%" }),
        NumberSample("Scrub · 按住数值左右拖动", new XYNumberField { Width = 220, Value = 50, IsScrubEnabled = true })];
    static Control NumberSample(string caption, XYNumberField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
    static Control[] Sliders() => [
        SliderSample("透明度", new XYSlider { Width = 360, Value = 62, Suffix = "%" }),
        SliderSample("光照强度", new XYSlider { Width = 360, Value = 35, DecimalPlaces = 0 }),
        SliderSample("相机速度", new XYSlider { Width = 360, Value = 4.5, Maximum = 10, Step = .5 }),
        SliderSample("时间倍率", new XYSlider { Width = 360, Value = 1, Minimum = 0, Maximum = 4, Step = .1, Suffix = "×" }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互 / Interaction" }, new TextBlock { Text = "Slider Drag → 连续粗调\nRail Click → 跳转值\nNumberField First Focus → Select All\nNumberField Typing → 精确输入\n↑ / ↓ → Step\nHorizontal Scrub → 连续精调\nShift / Ctrl → Large / Small Step" } } }];
    static Control SliderSample(string caption, XYSlider slider) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, slider } };
    static Control[] ComboBoxes() => [
        ComboSample("地区 · 可输入搜索", new XYComboBox { Width = 260, Text = "Northern Region", ItemsSource = new[] { "Northern Region", "Northern Coast", "Northern Highlands" } }),
        ComboSample("搜索演示 · North|", new XYComboBox { Width = 260, Text = "North", ItemsSource = new[] { "Northern Region", "Northern Coast", "Northern Highlands" } }),
        ComboSample("Placeholder", new XYComboBox { Width = 260, Placeholder = "选择地区", ItemsSource = new[] { "Northern Region", "Northern Coast", "Northern Highlands" } }),
        ComboSample("材质 · 可编辑候选", new XYComboBox { Width = 260, Text = "Steel", ItemsSource = new[] { "Steel", "Stainless Steel", "Tool Steel" } }),
        ComboSample("资源", new XYComboBox { Width = 260, Placeholder = "Texture", ItemsSource = new[] { "Texture", "Normal Texture", "Render Texture" } }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互 / Interaction" }, new TextBlock { Text = "First Focus → Select All\nTyping → Filter Candidates\nChevron → Open All\n↑ / ↓ → Navigate\nEnter → Select / Commit\nEsc → Close\nMouse Select → Select Item\nCustom Value → 按 IsCustomValueAllowed" } } }];
    static Control ComboSample(string caption, XYComboBox combo) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, combo } };
    static Control[] Selects() => [
        SelectSample("Default · Language", new XYSelect { Width = 260, SelectedIndex = 0, ItemsSource = new[] { "English", "简体中文", "日本語" } }),
        SelectSample("Open · Quality", new XYSelect { Width = 260, SelectedIndex = 2, IsDropDownOpen = true, ItemsSource = new[] { "Low", "Medium", "High", "Ultra" } }),
        SelectSample("Placeholder", new XYSelect { Width = 260, Placeholder = "Select status", ItemsSource = new[] { "Active", "Paused", "Archived" } }),
        SelectSample("Disabled", new XYSelect { Width = 260, SelectedIndex = 1, IsEnabled = false, ItemsSource = new[] { "Performance", "Balanced", "Quality" } }),
        SelectSample("Long Value", new XYSelect { Width = 260, SelectedIndex = 0, ItemsSource = new[] { "Performance profile · Vulkan desktop quality" } }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "Theme / 主题" }, new TextBlock { Text = "Light / Dark follows the Gallery theme\nValue surface + Chevron surface · no divider\nClick either surface → Toggle Popup\nEnter / Space → Open · ↑ / ↓ → Navigate · Enter → Commit · Esc → Close" } } }];
    static Control SelectSample(string caption, XYSelect select) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, select } };
    static Control[] TextAreas() => [
        TextAreaSection("标准 · 自动增长", TextAreaSample("一至两行", new XYTextArea { Width = 340, Text = "部署已完成。" }), TextAreaSample("三至四行", new XYTextArea { Width = 340, Text = "第一行\n第二行\n第三行\n第四行" }), TextAreaSample("长文本 · 最大高度 + 内部滚动", new XYTextArea { Width = 700, MaxHeight = 112, Text = "连接在 5000 毫秒后超时。\n请重试操作，或检查目标服务。\n其余诊断输出将在受限编辑区域内继续显示。\n重试前请检查服务日志。" })),
        TextAreaSection("标准 · 状态", TextAreaSample("占位提示", new XYTextArea { Width = 340, Placeholder = "请描述问题……" }), TextAreaSample("只读", new XYTextArea { Width = 340, IsReadOnly = true, Text = "只读部署说明。\n仍可选择和复制。" }), TextAreaSample("禁用", new XYTextArea { Width = 340, IsEnabled = false, Text = "禁用的配置详情。" }), TextAreaSample("错误", new XYTextArea { Width = 340, IsError = true, Text = "配置内容无效。" })),
        TextAreaSection("编辑区域 · 标题栏 / 正文 / 焦点底边", TextAreaSample("文本 · 地图数据描述", new XYTextArea { Width = 700, Mode = XYTextAreaMode.Editor, EditorType = "文本", Text = "地图数据描述：\n用于记录当前区域的说明。\n编辑后保存到当前地图文档。" }), TextAreaSample("JSON / 提示词", new XYTextArea { Width = 700, Mode = XYTextAreaMode.Editor, EditorType = "JSON", MaxHeight = 150, Text = "{\n  \"模式\": \"平衡\",\n  \"迭代次数\": 120\n}" })),
        new StackPanel { Width = 700, Spacing = 4, Children = { new XYCaption { Text = "主题" }, new TextBlock { Text = "浅色 / 深色 · 标题栏与正文保持层级差\n首次聚焦 → 全选 · 再次点击 → 正常光标\n编辑标题栏：类型（左）· 行数 · 字符数（右）" } } }];
    static Control TextAreaSection(string title, params Control[] samples) { var wrap = new WrapPanel { Orientation = Orientation.Horizontal }; foreach (var sample in samples) { sample.Margin = new Thickness(0, 0, 12, 10); wrap.Children.Add(sample); } return new StackPanel { Width = 720, Spacing = 6, Children = { new XYCaption { Text = title }, wrap } }; }
    static Control TextAreaSample(string caption, XYTextArea area) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, area } };
}
