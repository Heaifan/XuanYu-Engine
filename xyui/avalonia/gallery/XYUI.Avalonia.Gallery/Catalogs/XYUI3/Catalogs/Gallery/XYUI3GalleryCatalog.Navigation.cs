using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    static Control TabBarPreview()
    {
        var labels = new[] { "地图基础", "地图环境", "数据集", "Region.cs", "World.cs", "Scene.cs", "Camera.cs", "Light.cs", "材质", "纹理", "脚本", "日志" };
        var tabs = labels.Select((label, index) => new XYTab { Label = label, IsSelected = index == 1, IsModified = index == 2, IsClosable = index is > 1 and < 11 }).ToArray();
        var bar = new XYTabBar(tabs) { Width = 520 };
        var serial = labels.Length;
        bar.NewRequested += (_, _) => { var tab = new XYTab { Label = $"新页签-{++serial}", IsClosable = true }; bar.Tabs.Add(tab, true); bar.EnsureVisible(tab); };
        return bar;
    }

    static Control DockTabsPreview() => new XYDockTabs(
        Dock("Hierarchy"), Dock("Inspector", selected: true),
        Dock("Console", modified: true), Dock("Assets")) { Width = 676 };

    static XYDockTab Dock(string label, bool selected = false, bool modified = false) =>
        new(new XYTab { Label = label, IsSelected = selected, IsModified = modified, IsClosable = selected });

    static Control BreadcrumbPreview() => new XYBreadcrumb(
        new XYBreadcrumbItem { Label = "玄域项目" },
        new XYBreadcrumbItem { Label = "地图" },
        new XYBreadcrumbItem { IsCollapsed = true, HiddenPathOptions = ["中国", "华南"] },
        new XYBreadcrumbItem { Label = "行政区" },
        new XYBreadcrumbItem { Label = "广东省", IsCurrent = true, HasDropdown = true, DropdownOptions = ["广东省", "广西", "福建", "湖南"] }) { Width = 696 };

    static Control TreeNavigationPreview()
    {
        var root = new XYTreeNode { Label = "玄域项目", Icon = XyuiVectorIcon.Section, IsExpanded = true };
        var map = new XYTreeNode { Label = "地图", Icon = XyuiVectorIcon.Locate, IsExpanded = true, ActiveGuideDepth = 1 };
        var data = new XYTreeNode { Label = "数据集", Icon = XyuiVectorIcon.Section, IsExpanded = true, ActiveGuideDepth = 2 };
        data.Children.Add(new XYTreeNode { Label = "行政区", Icon = XyuiVectorIcon.Section, IsSelected = true, ActiveGuideDepth = 3 });
        data.Children.Add(new XYTreeNode { Label = "广东省", Icon = XyuiVectorIcon.StatusDot, ActiveGuideDepth = 2 });
        map.Children.Add(data);
        var res = new XYTreeNode { Label = "资源", Icon = XyuiVectorIcon.Browse, IsExpanded = true };
        res.Children.Add(new XYTreeNode { Label = "模型", Icon = XyuiVectorIcon.Section });
        res.Children.Add(new XYTreeNode { Label = "材质", Icon = XyuiVectorIcon.Section });
        root.Children.Add(map);
        root.Children.Add(res);
        return new XYTreeNavigation(root) { Width = 392, Height = 404, Padding = new Thickness(12) };
    }

    static Control PaginationPreview() => new StackPanel { Spacing = 14, Children = { new XYPagination { CurrentPage = 3, TotalPages = 24, TotalItems = 468, ShowTotalItems = true }, new XYPagination { CurrentPage = 1, TotalPages = 24 }, new XYPaginationFooter() } };
    static Control StepsPreview() { var states = new[] { ("创建项目", XYStepState.Completed), ("地图设置", XYStepState.Completed), ("数据配置", XYStepState.Current), ("验证", XYStepState.Pending), ("完成", XYStepState.Pending) }; var horizontal = states.Select(x => new XYStepNode(x.Item1, x.Item2)).ToArray(); var vertical = states.Select(x => new XYStepNode(x.Item1, x.Item2)).ToArray(); return new StackPanel { Spacing = 20, Children = { new XYSteps(horizontal) { Width = 760 }, new XYSteps(vertical) { Orientation = XYStepsOrientation.Vertical, Width = 300 } } }; }
    static Control ToolbarPreview() => new XYToolbar(new XYToolbarTool { Label = "选择", Icon = XyuiVectorIcon.Locate }, new XYToolbarTool { Label = "移动", Icon = XyuiVectorIcon.Locate, IsSelected = true }, new XYToolbarTool { Label = "旋转", Icon = XyuiVectorIcon.StatusDot }, new XYToolbarTool { Label = "缩放", Icon = XyuiVectorIcon.Section }, new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }, new XYToolbarTool { Label = "区域" }, new XYToolbarTool { Label = "道路" }) { Width = 740 };
    static Control ToolGroupPreview() => new XYToolbar(new XYToolGroup(new XYToolbarTool { Label = "选择" }, new XYToolbarTool { Label = "移动", IsSelected = true }, new XYToolbarTool { Label = "旋转" }, new XYToolbarTool { Label = "缩放" }), new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }, new XYToolGroup(new XYToolbarTool { Label = "区域" }, new XYToolbarTool { Label = "道路" }), new XYToolGroup(new XYToolbarTool { Label = "移动", IsSelected = true }) { IsCollapsed = true }) { Width = 740 };
    static Control CommandBarPreview()
    {
        var feedback = new TextBlock { Text = "Last Action · —", Classes = { "xyui-command-feedback" } };
        var bar = new XYCommandBar(new XYCommandItem("新建", "new", XYCommandRole.Primary, XyuiVectorIcon.Add), new XYCommandItem("导入", "import"), new XYCommandItem("保存", "save"), new XYCommandItem("验证", "validate"), new XYCommandItem("刷新", "refresh"), new XYCommandItem("删除", "delete", XYCommandRole.Danger));
        bar.CommandExecuted += (_, item) => feedback.Text = $"Last Action · {item.CommandId}";
        var saveAs = ActionItem("另存为", "save-as", feedback); var export = ActionItem("导出", "export", feedback); var copyLink = ActionItem("复制链接", "copy-link", feedback); var advanced = ActionItem("高级操作", "advanced", feedback);
        bar.MoreMenu.Items = [saveAs, export, copyLink, XYMenu.Separator(), advanced]; bar.RefreshMore();
        var contextual = new XYCommandBar(XYCommandBarVariant.Contextual, "roads", new XYCommandItem("编辑", "edit"), new XYCommandItem("复制", "copy"), new XYCommandItem("验证", "validate"), new XYCommandItem("删除", "delete", XYCommandRole.Danger));
        contextual.CommandExecuted += (_, item) => feedback.Text = $"Last Action · context.{item.CommandId}"; contextual.RefreshMore();
        return new StackPanel { Spacing = 8, Children = { bar, feedback, contextual } };
    }
    static XYMenuItem ActionItem(string label, string id, TextBlock feedback) { var item = Item(label); item.SelectionRequested += (_, _) => feedback.Text = $"Last Action · {id}"; return item; }
    static Control CommandPalettePreview()
    {
        var commands = new[]
        {
            new XYPaletteCommand("create-road", "创建道路", XYPaletteCommandType.Command, "地图", "创建一条新的道路。", "Ctrl+N", ["创建", "道路"]),
            new XYPaletteCommand("validate-road", "验证道路", XYPaletteCommandType.Command, "地图", "验证当前道路数据。", "Ctrl+V", ["验证", "道路"]),
            new XYPaletteCommand("open-road-dataset", "打开道路数据集", XYPaletteCommandType.Navigation, "数据集", "打开道路数据集。", "Ctrl+O", ["打开", "道路", "数据集"]),
            new XYPaletteCommand("road-tool", "进入道路工具", XYPaletteCommandType.Object, "道路", "进入道路编辑工具。", "Ctrl+E", ["道路", "工具"])
        };
        var palette = new XYCommandPalette(commands, [commands[0], commands[2], commands[3]]) { Width = 610 };
        var feedback = new TextBlock { Text = "Last Executed · —", Classes = { "xyui-command-feedback" } }; palette.ExecuteRequested += (_, item) => feedback.Text = $"Last Executed · {item.Id}";
        return new StackPanel { Spacing = 8, Children = { palette, feedback } };
    }
    static Control BackForwardPreview() { var nav = new XYBackForwardNavigation(); nav.Navigate("roads / 道路编辑"); nav.Navigate("广东省"); return nav; }
    static Control WorkspacePreview()
    {
        var items = new[]
        {
            new XYWorkspaceItem("map-edit", "地图编辑", Icon: XyuiVectorIcon.Locate),
            new XYWorkspaceItem("data-edit", "数据编辑", Icon: XyuiVectorIcon.Code),
            new XYWorkspaceItem("war-lab", "战争实验", Icon: XyuiVectorIcon.Eye),
            new XYWorkspaceItem("debug-analysis", "调试分析 (已禁用)", IsEnabled: false, Icon: XyuiVectorIcon.Section)
        };
        var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), items); var feedback = new TextBlock { Text = "Last Action · —", Classes = { "xyui-command-feedback" } };
        switcher.WorkspaceChangeRequested += (_, request) => request.Accept(); switcher.WorkspaceChanged += (_, id) => feedback.Text = $"Last Action · {id}"; switcher.ManageRequested += (_, _) => feedback.Text = "Last Action · manage-workspaces"; switcher.Open();
        return new StackPanel { Spacing = 8, Children = { switcher, feedback } };
    }
}
