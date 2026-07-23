namespace XuanYu.Editor.UI;

public static class UiText
{
    public static readonly EditorTreeNode[] ProjectTreeItems =
    [
        new("project:root", "玄域示例项目", "项目", "玄域示例项目", 0, "project"),
        new("project:worlds", "世界", "分类", "玄域示例项目/世界", 1, "folder"),
        new("world:main", "主世界", "世界", "玄域示例项目/世界/主世界", 2, "world"),
        new("world:test", "测试世界", "世界", "玄域示例项目/世界/测试世界", 2, "world"),
        new("project:assets", "资源", "分类", "玄域示例项目/资源", 1, "folder"),
        new("asset:icons", "图标", "资源分类", "玄域示例项目/资源/图标", 2, "asset"),
        new("asset:materials", "材质", "资源分类", "玄域示例项目/资源/材质", 2, "asset"),
        new("asset:scripts", "脚本", "资源分类", "玄域示例项目/资源/脚本", 2, "script"),
        new("asset:build", "构建", "资源分类", "玄域示例项目/资源/构建", 2, "build")
    ];

    public static readonly EditorTreeNode[] HierarchyTreeItems =
    [
        new("hierarchy:root", "世界根节点", "场景根", "主世界/世界根节点", 0, "world"),
        new("hierarchy:camera", "主相机", "相机", "主世界/世界根节点/主相机", 1, "camera"),
        new("hierarchy:ground", "地面", "地面", "主世界/世界根节点/地面", 1, "ground"),
        new("EntityId(1)", "基础测试实体", "最小场景实体", "主世界/实体编号(1)", 1, "entity")
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
        "静态对象：否",
        "可拾取：否",
        "参与碰撞：否"
    ];

    public static readonly string[] ProjectInspectorFields =
    [
        "名称：玄域示例项目",
        "类型：项目",
        "路径：玄域示例项目"
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
        ["聚焦"] = "视图聚焦命令已触发。",
        ["查看全部"] = "当前可见实体已进入视野。",
        ["平移"] = "视图平移命令已触发。",
        ["环绕"] = "视图环绕命令已触发。",
        ["运行"] = "运行预览已启动。",
        ["停止"] = "运行预览已停止。",
        ["构建"] = "构建任务已加入队列。"
    };
}
