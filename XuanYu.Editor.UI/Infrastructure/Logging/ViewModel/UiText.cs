namespace XuanYu.Editor.UI;

public static class UiText
{
    public static readonly string[] ToolItems =
    [
        "选择工具",
        "移动工具",
        "旋转工具",
        "缩放工具",
        "框选工具"
    ];

    public static readonly InspectorFieldRow[] InspectorFields =
    [
        new("变换", "", IsGroupHeader: true),
        new("位置", "X 0    Y 0    Z 0"),
        new("旋转", "X 0    Y 0    Z 0"),
        new("缩放", "X 1    Y 1    Z 1"),
        new("标记", "", IsGroupHeader: true),
        new("静态对象", "否"),
        new("可拾取", "否"),
        new("参与碰撞", "否")
    ];

    public static readonly string[] EmptyHints =
    [
        "在左侧选择一个项目资源",
        "在视口中选择一个实体",
        "在世界层级中选择一个节点"
    ];

    public static readonly string[] DebugItems =
    [
        "Shell 已挂载",
        "Avalonia UI 正在运行",
        "Vulkan 视口暂不接入",
        "当前阶段：UI 骨架实用化"
    ];

    public static readonly Dictionary<string, string> CommandMessages = new(StringComparer.Ordinal)
    {
        ["新建"] = "已准备创建新资源。",
        ["打开"] = "请选择要打开的项目或资源。",
        ["保存"] = "当前进度已进入保存流程。",
        ["撤销"] = "撤销上一项编辑操作。",
        ["重做"] = "重做上一项撤销操作。",
        ["聚焦"] = "视图聚焦命令已触发。",
        ["查看全部"] = "当前可见实体已进入视野。",
        ["平移"] = "视图平移命令已触发。",
        ["环绕"] = "视图环绕命令已触发。",
        ["运行"] = "运行预览已启动。",
        ["停止"] = "运行预览已停止。",
        ["构建"] = "构建任务已加入队列。"
    };

}
