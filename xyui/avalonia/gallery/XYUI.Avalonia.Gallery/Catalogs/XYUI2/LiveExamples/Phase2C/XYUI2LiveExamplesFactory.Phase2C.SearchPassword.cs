using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    static Control SearchFieldExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYSearchField { Width = 340, Text = "terrain_texture_albedo", Placeholder = "搜索资产..." });
        col1.Children.Add(new XYSearchField { Width = 340, Placeholder = "搜索材质、着色器或模型 (回车发起搜索)..." });

        var filterPanel = new Border
        {
            Child = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new XYCaption { Text = "筛选选项" },
                    new XYCheckbox { Content = "仅显示活跃项", IsChecked = true },
                    new XYCheckbox { Content = "匹配全字" }
                }
            }
        };
        var col2 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYSearchField { Width = 340, Text = "mesh", FilterActive = true, FilterContent = filterPanel });
        col2.Children.Add(new XYSearchField { Width = 340, Text = "not_found_query", IsNoResult = true });

        return SceneHost(
            Scene("场景 1 · 资产搜索与清除 (Enter 触发搜索 / Escape 或点击 X 快速清空)", col1),
            Scene("场景 2 · 复合条件筛选 (FilterActive 高亮 / 自定义筛选面板浮层)", col2));
    }

    static Control PasswordFieldExamples()
    {
        var col1 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col1.Children.Add(new XYPasswordField { Width = 340, Password = "XuanYu_SecurePass_2026", Placeholder = "请输入数据库密码" });
        col1.Children.Add(new XYPasswordField { Width = 340, Placeholder = "请输入开发者访问密钥..." });

        var col2 = new StackPanel { Spacing = 8, Width = 360, HorizontalAlignment = HorizontalAlignment.Left };
        col2.Children.Add(new XYPasswordField { Width = 340, Password = "LockedKey_888", IsEnabled = false });

        return SceneHost(
            Scene("场景 1 · 密码输入与临时明文 (按住右侧眼睛查看 / 松开或失焦立即遮罩)", col1),
            Scene("场景 2 · 占位提示与禁用安全保护", col2));
    }
}
