using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateCommandBarLiveExamples()
    {
        var feedback = new TextBlock { Text = "执行结果: 等待触发命令 · 支持 Standard、Contextual 与 More 溢出菜单", Classes = { "xyui-text-caption" } };

        var itemNew = new XYCommandItem("新建场景", "cmd-new", XYCommandRole.Primary, XyuiVectorIcon.Add);
        var itemImport = new XYCommandItem("导入资源", "cmd-import");
        var itemSave = new XYCommandItem("保存", "cmd-save");
        var itemValidate = new XYCommandItem("校验数据", "cmd-validate");
        var itemDelete = new XYCommandItem("删除场景", "cmd-delete", XYCommandRole.Danger);

        var standardBar = new XYCommandBar(itemNew, itemImport, itemSave, itemValidate, itemDelete);
        standardBar.CommandExecuted += (_, item) => feedback.Text = $"标准命令已触发: [{item.CommandId}] (角色: {item.Role})";

        var moreExport = new XYMenuItem { Label = "导出为 Package" };
        moreExport.SelectionRequested += (_, _) => feedback.Text = "溢出菜单命令已触发: [导出为 Package]";
        var moreBackup = new XYMenuItem { Label = "创建快照备份" };
        moreBackup.SelectionRequested += (_, _) => feedback.Text = "溢出菜单命令已触发: [创建快照备份]";
        standardBar.MoreMenu.Items = [moreExport, XYMenu.Separator(), moreBackup];
        standardBar.RefreshMore();

        var contextualBar = new XYCommandBar(XYCommandBarVariant.Contextual, "核心要塞道路_01",
            new XYCommandItem("编辑拓扑", "road-edit"),
            new XYCommandItem("对齐高程", "road-align"),
            new XYCommandItem("拆分路段", "road-split", XYCommandRole.Danger));
        contextualBar.CommandExecuted += (_, item) => feedback.Text = $"上下文命令已触发: [{item.CommandId}] (目标: {contextualBar.ContextIdentity})";

        var switchContextBtn = new XYButton { Content = "切换上下文目标为 [哨塔网格_03]", Variant = XyuiButtonVariant.Secondary };
        switchContextBtn.Click += (_, _) =>
        {
            contextualBar.UpdateContext("哨塔网格_03",
                new XYCommandItem("重新计算法线", "tower-normals"),
                new XYCommandItem("烘焙遮挡", "tower-bake"),
                new XYCommandItem("解组实体", "tower-ungroup", XYCommandRole.Danger));
            feedback.Text = "已更新上下文命令栏目标为: [哨塔网格_03]";
        };

        var root = new StackPanel
        {
            Spacing = 14,
            Children =
            {
                new TextBlock { Text = "标准命令栏 (含 Primary、Danger 隔离与 More 溢出菜单):", Classes = { "xyui-text-label" } },
                standardBar,
                feedback,
                new TextBlock { Text = "上下文命令栏 (带目标标识，支持动态切换上下文):", Classes = { "xyui-text-label" } },
                contextualBar,
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { switchContextBtn } }
            }
        };
        return WrapCard(root, "命令栏 · 角色分级、实体上下文关联与 More 溢出");
    }

    static Control CreateCommandBarComposition()
    {
        var bar = new XYCommandBar(XYCommandBarVariant.Contextual, "选中要素 · 3 个对象",
            new XYCommandItem("组合", "group"),
            new XYCommandItem("水平居中", "align-h"),
            new XYCommandItem("等距分布", "distribute"),
            new XYCommandItem("批量删除", "delete-all", XYCommandRole.Danger));

        var inspectHint = new Border
        {
            Classes = { "xyui-surface-panel-alt" },
            Padding = new(12),
            Child = new TextBlock { Text = "[ 属性检查器头部 · 针对多选实体提供快捷一次性批量命令 ]", Classes = { "xyui-text-caption" } }
        };

        var panel = new StackPanel { Spacing = 8, Width = 640, Children = { bar, inspectHint } };
        return WrapCard(panel, "属性面板上下文命令集成 · 实体选中驱动快捷指令");
    }
}
