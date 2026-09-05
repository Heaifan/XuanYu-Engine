using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control EnumPropertyExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYEnumProperty { Width = 520, Label = "渲染模式", ItemsSource = new[] { "实体 (Solid)", "线框 (Wireframe)", "点云 (Points)" }, SelectedIndex = 0 });
        col1.Children.Add(new XYEnumProperty { Width = 520, Label = "质量等级", ItemsSource = new[] { "低画质", "中画质", "高画质", "极致画质" }, SelectedIndex = 2 });

        var col2 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYEnumProperty { Width = 520, Label = "只读枚举选项", ItemsSource = new[] { "直射光", "点光源", "聚光灯" }, SelectedIndex = 0, IsReadOnly = true });
        col2.Children.Add(new XYEnumProperty { Width = 520, Label = "禁用枚举选项", ItemsSource = new[] { "开启", "关闭" }, SelectedIndex = 1, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 离散枚举配置 (严格复用 XYSelect，支持键盘与下拉选择)", col1),
            Scene("场景 2 · 只读呈现与禁用保护 (保留选项内容，阻断用户展开与修改)", col2));
    }

    static Control ReferencePropertyExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(CreateRef("实体对象引用", new("Tank_004", "Entity", "E004"), "Entity"));
        col1.Children.Add(CreateRef("地图道路数据", new("Highway_A1", "Dataset", "Road-01"), "Dataset"));

        var col2 = new StackPanel { Spacing = 8, Width = 540, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(CreateRef("空引用状态", null, "Entity", XYReferenceState.Empty));
        col2.Children.Add(CreateRef("资产丢失警示", new("Infantry_031", "Entity", "E031"), "Entity", XYReferenceState.Missing));
        col2.Children.Add(CreateRef("类型不匹配警示", new("Terrain_Albedo", "Texture", "T-104"), "Material", XYReferenceState.TypeMismatch));

        return SceneHost(
            Scene("场景 1 · 正常解析引用 (支持定位反馈、浏览选择与清除置空动作)", col1),
            Scene("场景 2 · 异常状态机 (显式区分未设置、引用丢失与类型不匹配警告)", col2));
    }

    static Control CreateRef(string caption, XYReferenceValue? value, string expected, XYReferenceState state = XYReferenceState.Resolved)
    {
        var matching = expected switch
        {
            "Dataset" => new XYReferenceValue("Roads_A1", "Dataset", "D001"),
            "Material" => new XYReferenceValue("Steel_Mat", "Material", "M001"),
            "Texture" => new XYReferenceValue("Terrain_Albedo", "Texture", "T001"),
            _ => new XYReferenceValue("Tank_004", "Entity", "E004")
        };
        var list = new ListBox { ItemsSource = new[] { matching, new XYReferenceValue("Wrong_Type", "Entity", "E999") }, MinWidth = 220 };
        return new XYReferenceProperty { Width = 520, Label = caption, Reference = value, ExpectedType = expected, ReferenceState = state, ReferencePickerContent = list };
    }
}
