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
        SelectSample("Variant · Quality", new XYSelect { Width = 260, SelectedIndex = 2, ItemsSource = new[] { "Low", "Medium", "High", "Ultra" } })];
    static Control SelectSample(string caption, XYSelect select) => new StackPanel { Spacing = 8, Margin = new Thickness(0, 0, 0, 12), Children = { new XYCaption { Text = caption }, select } };
    static Control[] TextAreas() => [
        TextAreaSample("Standard", new XYTextArea { Width = 300, Height = 104, Text = "第一行任务说明\n第二行补充说明\n第三行备注" }),
        TextAreaSample("Editor · JSON", new XYTextArea { Width = 324, Height = 148, Mode = XYTextAreaMode.Editor, EditorType = "JSON", Text = "{\n  \"engine\": \"XuanYu\",\n  \"mode\": \"balanced\"\n}" })];
    static Control TextAreaSample(string caption, XYTextArea area) => new StackPanel { Spacing = 8, Margin = new Thickness(0, 0, 0, 12), Children = { new XYCaption { Text = caption }, area } };
}
