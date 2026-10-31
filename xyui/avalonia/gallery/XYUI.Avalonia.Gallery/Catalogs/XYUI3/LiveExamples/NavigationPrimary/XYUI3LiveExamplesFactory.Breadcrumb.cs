using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateBreadcrumbLiveExamples()
    {
        var item1 = new XYBreadcrumbItem { Label = "玄域工程" };
        var item2 = new XYBreadcrumbItem { Label = "世界地图" };
        var item3 = new XYBreadcrumbItem { Label = "华南大区", IsCollapsed = true, HiddenPathOptions = ["华南大区", "粤港澳大湾区"] };
        var item4 = new XYBreadcrumbItem { Label = "佛山片区" };
        var item5 = new XYBreadcrumbItem { Label = "核心要塞", IsCurrent = true };

        var breadcrumb = new XYBreadcrumb(item1, item2, item3, item4, item5);

        var statusText = new TextBlock { Text = "当前所在位置：玄域工程 / 世界地图 / ... / 佛山片区 / 核心要塞", Classes = { "xyui-text-caption" } };
        var logText = new TextBlock { Text = "交互指南：点击祖先节点可回退；点击中间省略号可在下拉浮层中挑选隐藏的层级。", Classes = { "xyui-text-caption" } };

        item1.PointerPressed += (_, _) => statusText.Text = "路径跳转：回退至 [玄域工程] 根层级";
        item2.PointerPressed += (_, _) => statusText.Text = "路径跳转：回退至 [世界地图] 层级";
        item4.PointerPressed += (_, _) => statusText.Text = "路径跳转：定位至 [佛山片区] 层级";

        var container = new StackPanel { Spacing = 12, Children = { breadcrumb, statusText, logText } };
        return WrapCard(container, "面包屑路径导航 · 真实深层路径、中间省略号折叠与祖先跳转");
    }

    static Control CreateBreadcrumbComposition()
    {
        var col1 = new StackPanel
        {
            Spacing = 6, Width = 260,
            Children =
            {
                new TextBlock { Text = "1. Breadcrumb (层级路径定位)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "表达在空间/资源树中的纵向归属，支持随时回跳祖先节点。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new XYBreadcrumb(
                    new XYBreadcrumbItem { Label = "工程" },
                    new XYBreadcrumbItem { Label = "地图" },
                    new XYBreadcrumbItem { Label = "区域", IsCurrent = true })
            }
        };

        var col2 = new StackPanel
        {
            Spacing = 6, Width = 260,
            Children =
            {
                new TextBlock { Text = "2. Steps (线性连续向导)", Classes = { "xyui-text-label" } },
                new TextBlock { Text = "表达一步步顺序执行的任务流程，强调已完成/进行中/待处理。", Classes = { "xyui-text-caption" }, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                new Border
                {
                    Classes = { "xyui-surface-panel" },
                    Padding = new(8),
                    Child = new TextBlock { Text = "① 基础设置 ➔ ② 导入资源 ➔ ③ 完成构建", Classes = { "xyui-text-code" } }
                }
            }
        };

        var grid = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 20, Children = { col1, col2 } };
        return WrapCard(grid, "概念对比 · Breadcrumb (层级回溯) ≠ Steps (流程向导)");
    }
}
