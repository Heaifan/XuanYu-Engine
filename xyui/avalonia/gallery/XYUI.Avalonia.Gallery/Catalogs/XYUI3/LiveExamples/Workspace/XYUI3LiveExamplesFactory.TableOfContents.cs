using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateTableOfContentsLiveExamples()
    {
        var sections = new[]
        {
            new XYTocSection("intro", "01 架构概览", 1),
            new XYTocSection("setup", "02 环境配置", 1),
            new XYTocSection("map-edit", "地图基础设置", 2, "setup"),
            new XYTocSection("data-sync", "数据集同步", 2, "setup"),
            new XYTocSection("render", "03 渲染管线", 1),
            new XYTocSection("api", "04 API 索引", 1)
        };
        var state = new XYTableOfContentsState(sections, "map-edit");
        var hierarchical = new XYTableOfContents(state, XYTableOfContentsVariant.Hierarchical) { Width = 260 };
        var compact = new XYTableOfContents(state, XYTableOfContentsVariant.Compact) { Width = 260 };

        hierarchical.SectionRequested += (_, req) => req.Accept();
        compact.SectionRequested += (_, req) => req.Accept();

        var feedback = new TextBlock { Classes = { "xyui-text-caption" } };
        void SyncFeedback()
        {
            var cur = state.CurrentSectionId;
            var isChild = cur is "map-edit" or "data-sync";
            feedback.Text = $"当前聚焦章节: [{cur}] · 层级: {(isChild ? "Level 2 (子章节)" : "Level 1 (根章节)")} · 父级激活状态: {(isChild ? "setup (02 环境配置) [微高亮]" : "无父级")}";
        }
        state.Changed += (_, _) => SyncFeedback();
        SyncFeedback();

        var btn1 = new XYButton { Content = "跳转: 01 架构概览", Variant = XyuiButtonVariant.Secondary };
        btn1.Click += (_, _) => hierarchical.SelectSection("intro");
        var btn2 = new XYButton { Content = "跳转: 数据集同步 (二级)", Variant = XyuiButtonVariant.Secondary };
        btn2.Click += (_, _) => hierarchical.SelectSection("data-sync");
        var btn3 = new XYButton { Content = "跳转: 03 渲染管线", Variant = XyuiButtonVariant.Secondary };
        btn3.Click += (_, _) => hierarchical.SelectSection("render");

        var col = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = "双级层级目录 (带贯穿左导引线、当前章节 Accent 与父章节激活指示):", Classes = { "xyui-text-label" } },
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 24, Children = { hierarchical, compact } },
                feedback,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { btn1, btn2, btn3 } }
            }
        };
        return WrapCard(col, "长页面章节目录导航 · 两级限深、连续导引线与父子状态联动");
    }

    static Control CreateTableOfContentsComposition()
    {
        var sections = new[]
        {
            new XYTocSection("intro", "01 概述", 1),
            new XYTocSection("config", "02 配置", 1),
            new XYTocSection("terrain", "地形设置", 2, "config")
        };
        var state = new XYTableOfContentsState(sections, "terrain");
        var toc = new XYTableOfContents(state) { Width = 200 };
        toc.SectionRequested += (_, req) => req.Accept();

        var docContent = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new Border { Classes = { "xyui-surface-panel-alt" }, Padding = new(10, 6), Child = new TextBlock { Text = "# 01 概述\n玄域引擎地图与视口系统基础规范。", Classes = { "xyui-text-caption" } } },
                new Border { Classes = { "xyui-surface-panel-alt" }, Padding = new(10, 6), Child = new TextBlock { Text = "## 02 配置 / 地形设置\n当前正在阅读章节：地形网格分辨率与高程数据绑定。", Classes = { "xyui-text-caption" } } }
            }
        };

        var layout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children = { docContent, toc }
        };
        Grid.SetColumn(toc, 1);

        return WrapCard(layout, "文档与长页面锚点定位组合 · 右侧常驻目录与正文章节联动");
    }
}
