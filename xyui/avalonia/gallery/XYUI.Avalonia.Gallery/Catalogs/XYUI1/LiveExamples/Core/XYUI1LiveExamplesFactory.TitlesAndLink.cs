using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1LiveExamplesFactory
{
    static Control CreateHeadingExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 页面大标题与导言 (PageTitle)" });
        s1.Children.Add(new XYHeading { Text = "地图工程配置与世界树", Variant = XyuiHeadingVariant.PageTitle });
        s1.Children.Add(new XYCaption { Text = "管理当前加载的所有多边形地形网格、视口范围及全局光照缓存。" });
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 面板标准标题 (PanelTitle)" });
        s2.Children.Add(new XYHeading { Text = "光照贴图参数 (Lightmap Settings)", Variant = XyuiHeadingVariant.PanelTitle });
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateSectionTitleExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 变换属性分组 (Transform)" });
        s1.Children.Add(new XYSectionTitle { Text = "基础变换 (Transform)" });
        s1.Children.Add(new XYText { Text = "位置: [120.0, 45.5, 0.0]  旋转: [0, 0, 90]  缩放: [1.0, 1.0, 1.0]" });
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 材质属性分组 (Material)" });
        s2.Children.Add(new XYSectionTitle { Text = "材质与贴图 (Material & Textures)" });
        s2.Children.Add(new XYText { Text = "漫反射通道: albedo_rock_diffuse.png  粗糙度: 0.42" });
        panel.Children.Add(s2);
        return panel;
    }

    static Control CreateLinkExamples()
    {
        var panel = new StackPanel { Spacing = 12 };
        var s1 = new StackPanel { Spacing = 4 };
        s1.Children.Add(new XYCaption { Text = "场景 1 · 交互状态矩阵 (Normal / Hover / Disabled)" });
        var stateRow = new StackPanel { Orientation = global::Avalonia.Layout.Orientation.Horizontal, Spacing = 20 };
        stateRow.Children.Add(new XYLink { Content = "常态链接 (Normal)" });
        var activeLink = new XYLink { Content = "悬停态反馈 (Hover Accent)" };
        XY.SetForeground(activeLink, "XY.Brush.Accent.Strong");
        stateRow.Children.Add(activeLink);
        stateRow.Children.Add(new XYLink { Content = "禁用链接 (Disabled)", IsEnabled = false });
        s1.Children.Add(stateRow);
        panel.Children.Add(s1);

        var s2 = new StackPanel { Spacing = 4 };
        s2.Children.Add(new XYCaption { Text = "场景 2 · 诊断告警与关联文档跳转" });
        s2.Children.Add(new XYText { Text = "网格顶点法线解析失败，可能导致阴影断裂。" });
        s2.Children.Add(new XYLink { Content = "查看玄域引擎模型切线空间导入规范 →" });
        panel.Children.Add(s2);
        return panel;
    }
}
