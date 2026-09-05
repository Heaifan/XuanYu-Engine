using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control ButtonExamples()
    {
        var row1 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        row1.Children.Add(new XYButton { Content = "保存工程", Variant = XyuiButtonVariant.Primary });
        row1.Children.Add(new XYButton { Content = "还原修改", Variant = XyuiButtonVariant.Secondary });
        row1.Children.Add(new XYButton { Content = "删除资源", Variant = XyuiButtonVariant.Danger });
        row1.Children.Add(new XYButton { Content = "提交审查", IsEnabled = false });

        var row2 = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        row2.Children.Add(new XYButton { Content = "导入 FBX 网格", Icon = XyuiVectorIcon.Code, Variant = XyuiButtonVariant.Secondary });
        row2.Children.Add(new XYButton { Content = "同步至服务器", Icon = XyuiVectorIcon.Search, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 检视面板主次操作排版 (Primary / Secondary / Danger / Disabled)", row1),
            Scene("场景 2 · 图标文字混排命令 (Icon + Content 水平对齐与禁用流)", row2));
    }

    static Control IconButtonExamples()
    {
        var tb = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
        var btnSelect = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Code, Size = XyuiIconSize.Medium }, IsSelected = true };
        AutomationProperties.SetName(btnSelect, "代码检查模式");
        ToolTip.SetTip(btnSelect, "代码检查模式 (激活)");
        var btnSearch = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Medium } };
        AutomationProperties.SetName(btnSearch, "工程全局检索");
        ToolTip.SetTip(btnSearch, "工程全局检索 (Ctrl+Shift+F)");
        var btnCopy = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Copy, Size = XyuiIconSize.Medium } };
        AutomationProperties.SetName(btnCopy, "复制引用路径");
        ToolTip.SetTip(btnCopy, "复制引用路径 (Ctrl+C)");
        var btnDisabled = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Info, Size = XyuiIconSize.Medium }, IsEnabled = false };
        AutomationProperties.SetName(btnDisabled, "组件详细信息");
        ToolTip.SetTip(btnDisabled, "组件详细信息 (不可用)");
        tb.Children.Add(btnSelect); tb.Children.Add(btnSearch); tb.Children.Add(btnCopy); tb.Children.Add(btnDisabled);

        return SceneHost(
            Scene("场景 1 · 视口单选模式工具栏 (外部驱动 IsSelected + 规范 AutomationProperties.Name)", tb),
            Scene("场景 2 · 纯图标必须具备可访问名称与 ToolTip 提示规范", new XYCaption { Text = "本组纯图标均已设置 AutomationProperties.Name 与 ToolTip.Tip。" }));
    }

    static Control ToggleButtonExamples()
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        row.Children.Add(new XYToggleButton { Content = "正交视图", IsChecked = true });
        row.Children.Add(new XYToggleButton { Content = "网格吸附", IsChecked = true });
        row.Children.Add(new XYToggleButton { Content = "碰撞体框", IsChecked = false });
        row.Children.Add(new XYToggleButton { Content = "全局光影", IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 编辑器视图持久模式开关 (开启态显露 Persistent Edge 与高亮边框)", row),
            Scene("场景 2 · 键盘 Space 键就地切换 / Tab 键焦点导航测试", new XYCaption { Text = "焦点驻留时按 Space 翻转 IsChecked；Disabled 态下阻断状态反转。" }));
    }
}
