using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] SearchFields() =>
    [
        SearchSample("默认搜索", new XYSearchField { Width = 360, Placeholder = "搜索地图、区域或资源" }),
        SearchSample("已有内容", new XYSearchField { Width = 360, Text = "北部区域" }),
        SearchSample("筛选已启用", new XYSearchField { Width = 360, Text = "资源", FilterActive = true }),
        SearchSample("搜索中", new XYSearchField { Width = 360, Text = "正在查找", IsSearching = true }),
        SearchSample("无结果", new XYSearchField { Width = 360, Text = "不存在的资源", IsNoResult = true }),
        SearchSample("禁用", new XYSearchField { Width = 360, Text = "禁用搜索", IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "输入文本 → 实时编辑\nEnter → 发起搜索\nEsc → 清空文本\n清除按钮 → 清空并保留焦点\n筛选按钮 → 切换筛选态\n首次聚焦 → 全选\n禁用 → 禁止编辑、清除和筛选" } } },
    ];

    static Control[] PasswordFields() =>
    [
        PasswordSample("默认遮罩", new XYPasswordField { Width = 360, Placeholder = "请输入访问密码", Password = "部署密钥" }),
        PasswordSample("空值提示", new XYPasswordField { Width = 360, Placeholder = "请输入访问密码" }),
        PasswordSample("禁用", new XYPasswordField { Width = 360, Password = "禁用密钥", IsEnabled = false }),
        new StackPanel { Spacing = 4, Children = { new XYCaption { Text = "交互提示" }, new TextBlock { Text = "首次聚焦 → 全选\n按住眼睛按钮 → 临时显示密码\n松开、失去捕获或失去焦点 → 立即遮罩\nEnter / 空格 → 键盘按住显示\n禁用 → 禁止显示密码" } } },
    ];

    static Control SearchSample(string caption, XYSearchField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
    static Control PasswordSample(string caption, XYPasswordField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
}
