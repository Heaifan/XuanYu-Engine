namespace XuanYu.Editor.UI;

public static class LogText
{
    public static readonly string[] Logs =
    [
        "[21:32:08] 信息  编辑器  SampleProject 已选中",
        "[21:32:10] 信息  工具栏  当前工具切换为：选择",
        "[21:32:12] 警告  项目    当前项目未配置构建目标",
        "[21:32:14] 调试  视口    Vulkan 视口暂未接入"
    ];

    public static readonly string[] Problems =
    [
        "[21:32:12] 警告  项目    当前项目未配置构建目标",
        "[21:32:14] 错误  渲染    Vulkan 视口尚未初始化"
    ];

    public static readonly string[] Builds =
    [
        "[21:32:16] 信息  构建    等待构建目标配置",
        "[21:32:18] 调试  构建    暂未接入打包流程"
    ];

    public static readonly string[] Tasks =
    [
        "[21:32:20] 信息  任务    项目索引待接入",
        "[21:32:22] 信息  任务    资源导入队列为空"
    ];
}
