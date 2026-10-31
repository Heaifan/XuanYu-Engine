using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateDockTabsLiveExamples()
    {
        var dockTab1 = new XYDockTab(new XYTab { Label = "场景层级", IsSelected = true });
        var dockTab2 = new XYDockTab(new XYTab { Label = "属性检查器" });
        var dockTab3 = new XYDockTab(new XYTab { Label = "资源视口" });
        var dockTab4 = new XYDockTab(new XYTab { Label = "控制台" });

        var dockTabs = new XYDockTabs(dockTab1, dockTab2, dockTab3, dockTab4);

        var statusText = new TextBlock { Text = "当前活动面板：场景层级", Classes = { "xyui-text-caption" } };
        var eventLog = new TextBlock { Text = "拖拽操作指南：按住左侧 ⠿ Drag Grip 横向拖动可在栏内重排停靠项。", Classes = { "xyui-text-caption" } };

        dockTab1.Tab.SelectionRequested += (_, _) => statusText.Text = "当前活动面板：场景层级";
        dockTab2.Tab.SelectionRequested += (_, _) => statusText.Text = "当前活动面板：属性检查器";
        dockTab3.Tab.SelectionRequested += (_, _) => statusText.Text = "当前活动面板：资源视口";
        dockTab4.Tab.SelectionRequested += (_, _) => statusText.Text = "当前活动面板：控制台";

        var container = new StackPanel { Spacing = 12, Children = { dockTabs, statusText, eventLog } };
        return WrapCard(container, "停靠标签栏 · 真实 Drag Grip 同栏拖拽重排与 Raised 选中态");
    }

    static Control CreateDockTabsComposition()
    {
        var panel = new StackPanel { Spacing = 10 };
        var dock = new Border
        {
            Classes = { "xyui-surface-panel" },
            Width = 360,
            Child = new StackPanel
            {
                Children =
                {
                    new XYDockTabs(
                        new XYDockTab(new XYTab { Label = "属性检查器", IsSelected = true }),
                        new XYDockTab(new XYTab { Label = "项目设置" })),
                    new Border
                    {
                        Classes = { "xyui-surface-panel" },
                        Height = 110,
                        Padding = new(12),
                        Child = new TextBlock
                        {
                            Text = "[ 检查器面板属性树 ]\n· Transform: (0, 0, 0)\n· Mesh: Infantry_Mesh\n· Material: Standard_PBR",
                            Classes = { "xyui-text-code" }
                        }
                    }
                }
            }
        };

        panel.Children.Add(dock);
        panel.Children.Add(new TextBlock
        {
            Text = "停靠集成规范：DockTabs 作为面板顶栏使用；Raised 表面与下层内容自然过渡；不实现虚假跨窗口 Dock 冒充全局引擎。",
            Classes = { "xyui-text-caption" },
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });
        return WrapCard(panel, "面板停靠系统形态 · 面板级页签与属性检查器视口");
    }
}
