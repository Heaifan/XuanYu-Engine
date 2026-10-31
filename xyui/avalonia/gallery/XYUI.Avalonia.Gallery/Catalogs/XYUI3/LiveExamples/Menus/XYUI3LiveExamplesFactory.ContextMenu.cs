using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateContextMenuLiveExamples()
    {
        var feedback = new TextBlock { Text = "就绪 · 在下方任一目标卡片上右键点击触发就地操作", Classes = { "xyui-text-caption" } };
        Border TargetCard(string type, string name, string desc)
        {
            var card = new Border
            {
                Classes = { "xyui-surface-panel" },
                Padding = new(14, 10),
                CornerRadius = new(6),
                Width = 220,
                Child = new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = $"{type} · {name}", Classes = { "xyui-text-label" } },
                        new TextBlock { Text = desc, Classes = { "xyui-text-caption" } }
                    }
                }
            };
            var ctx = new XYContextMenu
            {
                ContextType = type, ContextName = name,
                Menu = new XYMenu(
                    ActionItem("定位到视口", XyuiVectorIcon.Locate),
                    ActionItem("编辑属性", XyuiVectorIcon.Section),
                    ActionItem("重命名"),
                    XYMenu.Separator(),
                    ActionItem("复制对象", sc: "Ctrl+C"),
                    ActionItem("移动到图层...", sub: true),
                    XYMenu.Separator(),
                    ActionItem("删除对象", danger: true)
                )
            };
            ctx.AttachTo(card);
            return card;
        }

        XYMenuItem ActionItem(string label, XyuiVectorIcon? icon = null, string sc = "", bool danger = false, bool sub = false)
        {
            var item = new XYMenuItem { Label = label, Icon = icon, Shortcut = sc, IsDestructive = danger, HasSubMenu = sub };
            item.Invoked += (_, _) => feedback.Text = $"右键执行 · {label}";
            return item;
        }

        var targets = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 14 };
        targets.Children.Add(TargetCard("ENTITY", "Infantry_023", "视口实体 · 右键定位/编辑/删除"));
        targets.Children.Add(TargetCard("REGION", "Guangdong_Zone", "地图区域 · 右键就地管理"));

        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(targets);
        panel.Children.Add(feedback);
        return WrapCard(panel, "就地右键菜单 · AttachTo 触发与对象标题头");
    }

    static Control CreateContextMenuComposition()
    {
        var menu = new XYContextMenu
        {
            ContextType = "DATASET",
            ContextName = "Administrative_Division",
            Menu = new XYMenu(
                new XYMenuItem { Label = "导出数据集", Icon = XyuiVectorIcon.Browse },
                new XYMenuItem { Label = "刷新缓存", Shortcut = "F5" },
                XYMenu.Separator(),
                new XYMenuItem { Label = "重建空间索引" },
                new XYMenuItem { Label = "校验拓扑结构" },
                XYMenu.Separator(),
                new XYMenuItem { Label = "清空数据集", IsDestructive = true }
            )
        };
        var panel = new StackPanel { Spacing = 8, Width = 280 };
        panel.Children.Add(menu);
        panel.Children.Add(new TextBlock
        {
            Text = "对象标题头规范：第一行辅助信息（ContextType），第二行主要对象名（ContextName），底部危险操作独立分组。",
            Classes = { "xyui-text-caption" },
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });
        return WrapCard(panel, "数据集对象上下文菜单静态解剖展示");
    }
}
