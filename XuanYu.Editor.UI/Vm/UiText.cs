namespace XuanYu.Editor.UI;

public static class UiText
{
    public static readonly string[] ProjectItems =
    [
        "SampleProject",
        "世界",
        "MainWorld",
        "TestWorld",
        "资源",
        "图标",
        "材质",
        "脚本",
        "构建"
    ];

    public static readonly string[] HierarchyItems =
    [
        "世界根节点",
        "  ├─ 主相机",
        "  ├─ 地面",
        "  └─ 示例实体"
    ];

    public static readonly string[] ToolItems =
    [
        "选择工具",
        "移动工具",
        "旋转工具",
        "缩放工具",
        "框选工具"
    ];

    public static readonly string[] InspectorFields =
    [
        "Transform",
        "位置    X 0    Y 0    Z 0",
        "旋转    X 0    Y 0    Z 0",
        "缩放    X 1    Y 1    Z 1",
        "标记",
        "□ 静态对象",
        "□ 可拾取",
        "□ 参与碰撞"
    ];

    public static readonly string[] ProjectInspectorFields =
    [
        "名称：SampleProject",
        "类型：项目",
        "路径：SampleProject"
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

    public static readonly string[] PropertyItems =
    [
        "布局保存：待接入",
        "主题：浅色外壳 + 深色视口",
        "快捷键：待接入",
        "编辑器偏好：待接入"
    ];

    public static readonly Dictionary<string, string> CommandMessages = new(StringComparer.Ordinal)
    {
        ["新建"] = "已准备创建新资源。",
        ["打开"] = "请选择要打开的项目或资源。",
        ["保存"] = "当前进度已进入保存流程。",
        ["撤销"] = "撤销上一项编辑操作。",
        ["重做"] = "重做上一项撤销操作。",
        ["运行"] = "运行预览已启动。",
        ["停止"] = "运行预览已停止。",
        ["构建"] = "构建任务已加入队列。"
    };
}
