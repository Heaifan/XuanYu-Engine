using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] SearchFields() =>
    [
        SearchSample("默认搜索", new XYSearchField { Width = 360, Placeholder = "搜索地图、区域或资源", FilterContent = FilterPanel() }),
        SearchSample("已有内容", new XYSearchField { Width = 360, Text = "北部区域", FilterContent = FilterPanel() }),
        SearchSample("筛选弹层已打开", new XYSearchField { Width = 360, Text = "资源", FilterContent = FilterPanel(), IsFilterOpen = true }),
        SearchSample("筛选已启用", new XYSearchField { Width = 360, Text = "资源", FilterActive = true, FilterContent = FilterPanel() }),
        SearchSample("查询与筛选已启用", new XYSearchField { Width = 360, Text = "北部", FilterActive = true, FilterContent = FilterPanel() }),
        SearchSample("无结果", new XYSearchField { Width = 360, Text = "不存在的资源", IsNoResult = true, FilterContent = FilterPanel() }),
        SearchSample("禁用", new XYSearchField { Width = 360, Text = "禁用搜索", IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "输入文本 → 实时编辑\n回车键 → 发起搜索\nEsc 键 → 清空文本；弹层打开时关闭弹层\n清除按钮 → 清空并保留焦点\n筛选按钮 → 打开或关闭真实筛选面板\n筛选激活态 → 独立于弹层开关\n首次聚焦 → 全选\n禁用 → 禁止编辑、清除和筛选" } } },
    ];

    static Control[] PasswordFields() =>
    [
        PasswordSample("默认遮罩", new XYPasswordField { Width = 360, Placeholder = "请输入访问密码", Password = "部署密钥" }),
        PasswordSample("空值提示", new XYPasswordField { Width = 360, Placeholder = "请输入访问密码" }),
        PasswordSample("按住显示密码", new XYPasswordField { Width = 360, Placeholder = "请按住右侧眼睛", Password = "临时访问口令" }),
        PasswordSample("禁用", new XYPasswordField { Width = 360, Password = "禁用密钥", IsEnabled = false }),
        PasswordSample("较长密码", new XYPasswordField { Width = 360, Password = "地图发布前的临时安全访问口令" }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "首次聚焦 → 全选\n按住右侧眼睛 → 临时显示密码\n松开、失去捕获或失去焦点 → 立即遮罩\n回车键 / 空格键 → 按住显示，抬键遮罩\n禁用 → 禁止显示密码" } } },
    ];

    static Control FilterPanel() => new Border { Child = new StackPanel { Spacing = 8, Children = { new XYCaption { Text = "筛选条件" }, new XYCheckbox { Content = "仅显示已启用", IsChecked = true }, new XYCheckbox { Content = "仅显示当前区域" }, new XYCheckbox { Content = "包含子资源" } } } };

    static Control SearchSample(string caption, XYSearchField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
    static Control PasswordSample(string caption, XYPasswordField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
}
