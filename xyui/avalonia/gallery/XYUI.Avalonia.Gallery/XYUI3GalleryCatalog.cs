using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static class XYUI3GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-3-3.01" => MenuBarPreview(), "XYUI-3-3.02" => MenuPreview(),
        "XYUI-3-3.03" => ContextPreview(), "XYUI-3-3.04" => SubMenuPreview(),
        _ => new TextBlock { Text = "未注册组件" }
    };
    static Control MenuBarPreview()
    {
        var panel = new StackPanel { Spacing = 14 }; panel.Children.Add(new XYMenuBar(
            new XYMenuBarItem { Label = "文件", Menu = MenuPreview("文件") }, new XYMenuBarItem { Label = "编辑", IsHovered = true, Menu = MenuPreview("编辑") },
            new XYMenuBarItem { Label = "视图", IsActive = true, Menu = MenuPreview("视图") }, new XYMenuBarItem { Label = "窗口", Menu = MenuPreview("窗口") }, new XYMenuBarItem { Label = "帮助", Menu = MenuPreview("帮助") }));
        panel.Children.Add(new XYCaption { Text = "编辑 = Hover · 视图 = Active" }); return panel;
    }
    static XYMenu MenuPreview(string title = "") => new XYMenu(
        Item("新建", "Ctrl+N"), Item("打开", "Ctrl+O", hover: true), Item("保存", "Ctrl+S", hover: true),
        XYMenu.Separator(), Item("显示网格", checkedItem: true, check: XyuiMenuCheckKind.Check), Item("正交视图", checkedItem: true, check: XyuiMenuCheckKind.Radio), Item("投影模式", submenu: true),
        XYMenu.Separator(), Item("关闭所有窗口", enabled: false), XYMenu.Separator(), Item("删除", destructive: true));
    static Control ContextPreview()
    {
        var target = new Border { Padding = new Thickness(10, 6), Child = new XYCaption { Text = "右键对象：Infantry_023" } };
        var context = new XYContextMenu { ContextType = "ENTITY", ContextName = "Infantry_023", Menu = new XYMenu(
            Item("定位", hover: true), Item("编辑"), Item("重命名"), XYMenu.Separator(), Item("复制"), Item("移动到", submenu: true), XYMenu.Separator(), Item("删除", destructive: true)) };
        context.AttachTo(target);
        return new StackPanel { Spacing = 8, Children = { target, context } };
    }
    static Control SubMenuPreview() => new StackPanel { Spacing = 12, Children = { new XYSubMenu { ParentMenu = Parent(), ChildMenu = Child() }, new XYCaption { Text = "Open Left · 静态镜像 Variant" }, new XYSubMenu { OpenLeft = true, ParentMenu = Parent(), ChildMenu = Child() } } };
    static XYMenu Parent() => new(Item("打开"), Item("保存"), Item("导出", hover: true, submenu: true), XYMenu.Separator(), Item("关闭"));
    static XYMenu Child() => new(Item("导出地图数据"), Item("导出图片"), Item("导出配置"), XYMenu.Separator(), Item("高级导出", submenu: true));
    static XYMenuItem Item(string label, string shortcut = "", bool enabled = true, bool checkedItem = false,
        XyuiMenuCheckKind check = XyuiMenuCheckKind.None, bool destructive = false, bool hover = false, bool submenu = false) => new()
    { Label = label, Shortcut = shortcut, IsEnabled = enabled, IsChecked = checkedItem, CheckKind = check, IsDestructive = destructive, IsHovered = hover, HasSubMenu = submenu };
}
