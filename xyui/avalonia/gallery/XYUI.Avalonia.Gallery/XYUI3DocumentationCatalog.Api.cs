namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id == "XYUI-3-3.23" ?
    [
        P("XYBottomNavigationItem.Id", "string", "必填", "目的地唯一标识；SelectDestination 使用它。"),
        P("XYBottomNavigationItem.Label", "string", "必填", "目的地显示文本。"),
        P("XYBottomNavigationItem.Icon", "XyuiVectorIcon", "必填", "来自 XYUI Vector Icon Registry。"),
        P("XYBottomNavigationItem.Badge", "string?", "null", "可选状态提示文本；null 时不显示 Badge。"),
        P("XYBottomNavigationItem.IsEnabled", "bool", "true", "false 时目的地不可点击。"),
        P("new XYBottomNavigation(items)", "XYBottomNavigation", "—", "快速构造；自动创建 XYNavigationState。"),
        P("new XYBottomNavigation(state, items?, primaryAction?)", "XYBottomNavigation", "—", "显式注入共享状态、目的地和可选 Primary Action。"),
        P("NavigationState", "XYNavigationState", "必填", "共享目的地与当前 SelectedId 的状态源。"),
        P("PrimaryAction", "XYButton?", "null", "可选中心 Primary Action；不改变目的地选中状态。"),
        P("Items", "IReadOnlyList<XYBottomNavigationItem>", "state.Entries", "只读目的地集合，按等宽 Slot 渲染。"),
        P("CurrentDestinationId", "string?", "state.SelectedId", "当前目的地 Id，只读投影。"),
        P("SafeAreaBottom", "double", "0", "宿主提供的底部安全区 DIP。"),
        P("DestinationRequested", "event EventHandler<XYBottomNavigationRequest>", "未订阅", "请求切换；处理程序必须 Accept 或 Reject。"),
        P("PrimaryActionRequested", "event EventHandler", "未订阅", "点击中心 PrimaryAction 时触发。"),
        P("DestinationChanged", "event EventHandler<string>", "未订阅", "请求被 Accept 后提交时触发。"),
        P("SelectDestination(id)", "void", "—", "发起请求；未 Accept、Reject 或当前 Id 时不提交。"),
        P("CommitDestination(id)", "void", "—", "绕过请求直接提交已确认的目的地。"),
        P("XYBottomNavigationRequest.Destination", "XYNavigationEntry", "必填", "本次请求的目标目的地。"),
        P("XYBottomNavigationRequest.IsAccepted", "bool", "false", "Accept() 后为 true。"),
        P("XYBottomNavigationRequest.IsRejected", "bool", "false", "Reject() 后为 true。"),
        P("new XYBottomNavigationRequest(destination)", "XYBottomNavigationRequest", "—", "由控件为每次切换请求创建。"),
        P("XYBottomNavigationRequest.Accept()", "void", "—", "允许 SelectDestination 提交。"),
        P("XYBottomNavigationRequest.Reject()", "void", "—", "拒绝本次切换并保持原状态。"),
        P("new XYNavigationState(entries, selectedId?)", "XYNavigationState", "首项 Id", "创建共享状态；selectedId 省略时选中首项。"),
        P("XYNavigationState.SelectedId", "string?", "首项 Id", "共享状态当前选中 Id。"),
        P("XYNavigationState.Entries", "IReadOnlyList<XYNavigationEntry>", "必填", "共享状态中的目的地条目。"),
        P("XYNavigationState.Selected", "XYNavigationEntry?", "null", "按 SelectedId 查询当前条目。"),
        P("XYNavigationState.Changed", "event EventHandler", "未订阅", "Select(id) 成功更新时触发。"),
        P("XYNavigationState.Select(id)", "void", "—", "校验 Id 后更新状态并触发 Changed。"),
        P("XYNavigationEntry.Id / Label / Icon", "string / string / XyuiVectorIcon", "必填", "共享状态条目的标识、显示文本和图标。")
    ] : [];

    static XYUIDocProperty P(string name, string type, string value, string description) =>
        new(name, type, value, description);
}
