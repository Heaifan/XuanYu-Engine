using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateSubMenuLiveExamples()
    {
        var feedback = new TextBlock { Text = "就绪 · 鼠标悬停或点击「导出为...」展开子菜单", Classes = { "xyui-text-caption" } };
        var batch = SubAction("批量资产包 (ZIP)...", feedback, sub: true);
        var child = new XYMenu(
            SubAction("地图数据 (GeoJSON)", feedback),
            SubAction("高度图图像 (PNG)", feedback),
            SubAction("项目配置文件 (JSON)", feedback),
            XYMenu.Separator(),
            batch
        );
        var batchSubMenu = new XYSubMenu { ParentMenu = child, ChildMenu = new XYMenu(SubAction("增量资产清单", feedback)), Trigger = batch };
        batch.SubMenu = batchSubMenu;
        var export = SubAction("导出为...", feedback, sub: true);
        var parent = new XYMenu(
            SubAction("打开工程", feedback),
            SubAction("保存工程", feedback),
            export,
            XYMenu.Separator(),
            SubAction("关闭工程", feedback)
        );

        var subMenu = new XYSubMenu { ParentMenu = parent, ChildMenu = child, Trigger = export };
        export.SubMenu = subMenu;
        var modeToggle = new XYButton { Content = "切换展开方向：当前 OpenRight", Margin = new(0, 0, 0, 6) };
        modeToggle.Click += (_, _) =>
        {
            subMenu.OpenLeft = !subMenu.OpenLeft;
            modeToggle.Content = $"切换展开方向：当前 {(subMenu.OpenLeft ? "OpenLeft (镜像)" : "OpenRight (标准)")}";
        };

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(modeToggle);
        var menuHost = new Grid { ColumnDefinitions = new ColumnDefinitions("270,300") };
        menuHost.Children.Add(parent); menuHost.Children.Add(subMenu); Grid.SetColumn(subMenu, 1);
        panel.Children.Add(menuHost);
        panel.Children.Add(feedback);
        return WrapCard(panel, "层级连接型子菜单 · 父子关联与镜像展开");
    }

    static XYMenuItem SubAction(string label, TextBlock feedback, bool sub = false)
    {
        var item = new XYMenuItem { Label = label, HasSubMenu = sub };
        item.Invoked += (_, _) => feedback.Text = $"已触发子菜单项 · {label}";
        return item;
    }

    static Control CreateSubMenuComposition()
    {
        var hierarchy = new XYSubMenuHierarchyDebugPreview();
        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(hierarchy);
        panel.Children.Add(new TextBlock
        {
            Text = "多层级生命周期规范：展开曾孙级必须同步维持子级与父级处于可见与有效关联，关闭父级级联收拢全部后代。",
            Classes = { "xyui-text-caption" },
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });
        return WrapCard(panel, "三级级联子菜单 · 父 / 子 / 孙 运行时生命周期");
    }
}
