using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3GalleryCatalog
{
    public const string ContextToolbarId = "XYUI-3-3.17-context";
    public static Control ContextToolbarPreview() => BuildDemos();
    static Control BuildDemos()
    {
        var standard = CreateToolbar(false); var open = CreateToolbar(true); var dynamic = CreateDynamicToolbar();
        return new StackPanel { Spacing = 12, Children = { Card("Demo 1 · 标准状态", standard), Card("Demo 2 · Dropdown 展开状态", open), Card("Demo 3 · 动态模式", dynamic) } };
    }
    static Control CreateToolbar(bool open)
    {
        var board = ContextToolbarBoard(); var split = new XYSplitButton { Content = "道路", Height = 32, Width = 96 }; board.ActionExecuted += (_, action) => split.Content = action.Label; board.AttachTrigger(split); split.MenuCommand = new BoardCommand(board, split); var groups = new[] { Group("编辑", "选择", "框选", "移动", "旋转", "缩放"), Group("绘制", split), Group("视图", "聚焦", "全览", "平移", "环绕"), Group("环境", "环境", "视图"), Group("吸附", "吸附：关闭") };
        var root = new StackPanel { Spacing = 8, Children = { new XYContextToolbar(groups), board, new TextBlock { Text = open ? "默认打开 Board，验证 A3 + A4" : "道路 Dropdown 默认关闭", Classes = { "xyui-text-caption" } } } };
        if (open) root.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(() => board.Open(split), DispatcherPriority.Loaded); return root;
    }
    static Control CreateDynamicToolbar()
    {
        var edit = Group("编辑", "选择", "移动"); var draw = Group("绘制", "道路", "区域"); var view = Group("视图", "聚焦", "全览"); var env = Group("环境", "环境", "视图"); var snap = Group("吸附", "吸附：关闭"); var bar = new XYContextToolbar(edit, draw, view, env, snap); var modes = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { Mode("普通模式", () => Set(false, false, true, false, false)), Mode("区域编辑", () => Set(true, true, true, true, true)), Mode("道路编辑", () => Set(true, true, true, false, true)) } }; void Set(bool e, bool d, bool v, bool n, bool s) { edit.IsVisible = e; draw.IsVisible = d; view.IsVisible = v; env.IsVisible = n; snap.IsVisible = s; } Set(true, false, true, false, false); return new StackPanel { Spacing = 8, Children = { modes, bar } };
    }
    static XYContextGroup Group(string title, params object[] controls) { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 }; foreach (var control in controls) panel.Children.Add(control as Control ?? new XYButton { Content = control }); return new XYContextGroup(title, panel); }
    static Border Card(string title, Control child) => new() { Classes = { "xyui-surface-panel" }, Padding = new Thickness(12), Child = new StackPanel { Spacing = 7, Children = { new TextBlock { Text = title, Classes = { "xyui-text-section" } }, child } } };
    internal static XYContextDropdownBoard ContextToolbarBoard() => new("绘制", [new("point", "点"), new("line", "线"), new("area", "面")], new Dictionary<string, IReadOnlyList<XYContextAction>> { ["point"] = [new("marker", "点标记"), new("poi", "兴趣点")], ["line"] = [new("road", "道路"), new("boundary", "边界线"), new("river", "河流")], ["area"] = [new("region", "区域"), new("blocked", "禁行区"), new("parcel", "地块")] });
    static XYButton Mode(string text, Action action) { var button = new XYButton { Content = text, Variant = XyuiButtonVariant.Secondary }; button.Click += (_, _) => action(); return button; }
    sealed class BoardCommand(XYContextDropdownBoard board, Control target) : System.Windows.Input.ICommand { public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? p) => true; public void Execute(object? p) => board.Toggle(target); }
}

public static partial class XYUI3LiveExamplesFactory
{
    internal static Control CreateContextToolbarLiveExamples() => XYUI3GalleryCatalog.ContextToolbarPreview();
    internal static Control CreateContextToolbarComposition() => CreateContextComposition();
    static Control CreateContextComposition()
    {
        var board = XYUI3GalleryCatalog.ContextToolbarBoard(); var split = new XYSplitButton { Content = "道路", Height = 32, Width = 96 }; board.ActionExecuted += (_, action) => split.Content = action.Label; board.AttachTrigger(split); split.MenuCommand = new ContextBoardCommand(board, split);
        var toolbar = new XYContextToolbar(new XYContextGroup("绘制", new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { split } }));
        return new StackPanel { Spacing = 8, Children = { new TextBlock { Text = "A3 + A4 · 单 Board 组合", Classes = { "xyui-text-section" } }, toolbar, board } };
    }
    sealed class ContextBoardCommand(XYContextDropdownBoard board, Control target) : System.Windows.Input.ICommand { public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? p) => true; public void Execute(object? p) => board.Toggle(target); }
}

public static partial class XYUI3DocumentationCatalog
{
    public static XYUI1ComponentDocument ContextToolbarDocument() => new(XYUI3GalleryCatalog.ContextToolbarId, "上下文工具栏", "Context Toolbar", "分组卡片式上下文工具栏与一体展开 Board。", "用于编辑器上下文模式的紧凑工具组与单层级下拉操作。", XYUI3GalleryCatalog.ContextToolbarPreview, ["<c:XYContextToolbar />", "<c:XYContextDropdownBoard />"], [new("A + A3 + A4", "48 DIP toolbar / compact board", "Editor context")], [new("Open", "单 Popup Board"), new("Keyboard", "Esc / arrows / Enter")], [], [], "XYUI.Avalonia.Controls.XYContextToolbar") { CanonicalIdentity = "XYUI-3-3.17~3.22 · Context Toolbar Gallery", Category = "XYUI-3 · Context Toolbar", Acceptance = "GALLERY IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE", LiveExamplesFactory = XYUI3LiveExamplesFactory.CreateContextToolbarLiveExamples, CompositionFactory = XYUI3LiveExamplesFactory.CreateContextToolbarComposition };
}
