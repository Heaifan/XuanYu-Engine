using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control SplitButtonExamples()
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var status = new XYCaption { Text = "外部宿主等待命令……" };
        var splitBuild = new XYSplitButton { Content = "构建工程 (Main)" };
        splitBuild.MainCommand = new XYUI2GalleryCommand(() => status.Text = "MainCommand：外部宿主执行构建工程");
        splitBuild.MenuCommand = new XYUI2GalleryCommand(() => status.Text = "MenuCommand：外部宿主可在此打开构建配置菜单");
        var splitExport = new XYSplitButton { Content = "快速导出 (FBX)" };
        splitExport.MainCommand = new XYUI2GalleryCommand(() => status.Text = "MainCommand：外部宿主执行快速导出");
        splitExport.MenuCommand = new XYUI2GalleryCommand(() => status.Text = "MenuCommand：外部宿主可在此打开导出配置菜单");
        var splitDisabled = new XYSplitButton { Content = "发布制品", IsEnabled = false };
        row.Children.Add(splitBuild); row.Children.Add(splitExport); row.Children.Add(splitDisabled);

        var note = new XYCaption { Text = "契约验证：主区执行 MainCommand；右槽 34 DIP 图标区执行 MenuCommand。Trigger ≠ Popup owner。" };

        return SceneHost(
            Scene("场景 1 · 资产烘焙与工程发布 (双命令独立命中区 / 共享 Chrome / 18 DIP Divider)", row),
            Scene("场景 2 · 触发器语义规范 (本组件无内建 Menu/Popup，菜单由外部响应弹出)", new StackPanel { Spacing = 6, Children = { note, status } }));
    }

    static Control DropDownButtonExamples()
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        var status = new XYCaption { Text = "外部宿主等待 OpenCommand……" };
        var ddbExport = new XYDropDownButton { Content = "导出格式: ASTC_4x4" };
        ddbExport.OpenCommand = new XYUI2GalleryCommand(() => status.Text = "OpenCommand：外部宿主可在此打开导出格式菜单");
        var ddbFilter = new XYDropDownButton { Content = "筛选规则: 仅修改项" };
        ddbFilter.OpenCommand = new XYUI2GalleryCommand(() => status.Text = "OpenCommand：外部宿主可在此打开筛选菜单");
        var ddbDisabled = new XYDropDownButton { Content = "构建配置: Release", IsEnabled = false };
        row.Children.Add(ddbExport); row.Children.Add(ddbFilter); row.Children.Add(ddbDisabled);

        var note = new XYCaption { Text = "规范验证：整钮为单一点击区（无分割线），Chevron 槽纯装饰。Trigger ≠ Popup owner。" };

        return SceneHost(
            Scene("场景 1 · 下拉选项选择入口 (单一命中区 / 右侧 Chevron 装饰槽 / 禁用态同步衰减)", row),
            Scene("场景 2 · 触发器与外部浮层契约 (点击整钮发出 OpenCommand，宿主承载具体弹窗)", new StackPanel { Spacing = 6, Children = { note, status } }));
    }

    static Control CheckboxExamples()
    {
        var col = new StackPanel { Spacing = 8 };
        col.Children.Add(new XYCheckbox { Content = "启用环境光遮蔽 (SSAO)", IsChecked = true });
        col.Children.Add(new XYCheckbox { Content = "屏幕空间反射 (SSR)", IsChecked = true });
        col.Children.Add(new XYCheckbox { Content = "景深模拟 (Depth of Field)", IsChecked = false });

        var tree = new StackPanel { Spacing = 6, Margin = new Thickness(16, 0, 0, 0) };
        var root = new XYCheckbox { Content = "地形地貌图层 (部分可见 · Mixed)", IsThreeState = true, IsChecked = null };
        var sub1 = new XYCheckbox { Content = "基础高程网格 (Base Heightmap)", IsChecked = true, Margin = new Thickness(20, 0, 0, 0) };
        var sub2 = new XYCheckbox { Content = "地表植被分布 (Vegetation Scatter)", IsChecked = false, Margin = new Thickness(20, 0, 0, 0) };
        tree.Children.Add(root); tree.Children.Add(sub1); tree.Children.Add(sub2);

        return SceneHost(
            Scene("场景 1 · 视口后处理效果多选 (Checked / Unchecked 独立复选)", col),
            Scene("场景 2 · 资源层级树状批量选择 (IsThreeState=true · 呈现 Indeterminate 混合态)", tree));
    }
}
