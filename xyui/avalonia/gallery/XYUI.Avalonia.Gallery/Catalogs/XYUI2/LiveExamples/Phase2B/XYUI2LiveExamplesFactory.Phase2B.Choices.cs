using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control RadioButtonExamples()
    {
        var col1 = new StackPanel { Spacing = 6 };
        col1.Children.Add(new XYRadioButton { GroupName = "vport_render", Content = "实时预览 (Preview)", IsChecked = true });
        col1.Children.Add(new XYRadioButton { GroupName = "vport_render", Content = "最终渲染质量 (Production)" });
        col1.Children.Add(new XYRadioButton { GroupName = "vport_render", Content = "性能调试模式 (Wireframe)" });

        var col2 = new StackPanel { Spacing = 6 };
        col2.Children.Add(new XYRadioButton { GroupName = "export_lvl", Content = "草稿质量 (Draft)" });
        col2.Children.Add(new XYRadioButton { GroupName = "export_lvl", Content = "生产标准 (Standard)", IsChecked = true });
        col2.Children.Add(new XYRadioButton { GroupName = "export_lvl", Content = "电影级最高精度 (Cinema)" });
        col2.Children.Add(new XYRadioButton { GroupName = "export_lvl", Content = "神经超分重建 (不可用)", IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 视口渲染模式单选 (GroupName 互斥 / 点击文字与圆点均可切换)", col1),
            Scene("场景 2 · 资产导出质量分级 (单选项互斥 / 包含禁用态对照)", col2));
    }

    static Control SwitchExamples()
    {
        var col1 = new StackPanel { Spacing = 10 };
        col1.Children.Add(new XYSwitch { Content = "实时自动保存工程 (AutoSave)", IsChecked = true });
        col1.Children.Add(new XYSwitch { Content = "后台增量资源编译 (Incremental)", IsChecked = false });
        col1.Children.Add(new XYSwitch { Content = "GPU 硬件光线追踪加速 (RTX)", IsChecked = true });

        var col2 = new StackPanel { Spacing = 10 };
        col2.Children.Add(new XYSwitch { Content = "高精度深度缓冲模拟", IsChecked = true });
        col2.Children.Add(new XYSwitch { Content = "实验性流式几何加载 (功能锁定)", IsChecked = false, IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 引擎系统级立即设置 (二元立即生效 / 点击滑块或整行均可切换)", col1),
            Scene("场景 2 · 实验性配置与禁用状态 (Disabled 态弱化且阻断点击)", col2));
    }
}
