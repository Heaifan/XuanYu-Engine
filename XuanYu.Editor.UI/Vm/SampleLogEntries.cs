namespace XuanYu.Editor.UI;

public static class SampleLogEntries
{
    public static readonly LogEntry[] All =
    [
        new("21:32:08", EditorLogLevel.Info, EditorLogSource.Editor,
            EditorLogCategory.Layout, "编辑器布局已恢复",
            "底部日志只保留低频事实记录。", "Layout", "Editor-213208"),
        new("21:32:10", EditorLogLevel.Info, EditorLogSource.Project,
            EditorLogCategory.Load, "已打开项目：SampleProject",
            "真实项目加载事件后续由 EditorLogBus 接入。", "SampleProject", "Project-213210"),
        new("21:32:12", EditorLogLevel.Info, EditorLogSource.Render,
            EditorLogCategory.Backend, "Vulkan Surface 生命周期已接入；Device / Swapchain 尚未接入",
            "详细后端信息后续进入详情或文件日志。", "Viewport", "Render-213212"),
        new("21:32:15", EditorLogLevel.Info, EditorLogSource.Build,
            EditorLogCategory.Queue, "构建队列空闲",
            "构建入口已预留，当前为示例状态。", "Build", "Build-213215"),
        new("21:32:17", EditorLogLevel.Warning, EditorLogSource.Input,
            EditorLogCategory.Capture, "点击拾取未命中任何对象", "低频点击事实日志；Hover 空结果不得逐条进入底部日志。",
            "Viewport", "Input-213217", 6),
        new("21:32:20", EditorLogLevel.Info, EditorLogSource.Task,
            EditorLogCategory.Import, "资源导入队列为空",
            "任务摘要可以进入日志，帧级进度不进入日志。", "Import", "Task-213220")
    ];

    public static readonly LogEntry[] Problems =
        All.Where(x => x.Level is EditorLogLevel.Warning or EditorLogLevel.Error).ToArray();

    public static readonly LogEntry[] Builds =
        All.Where(x => x.Source is EditorLogSource.Build).ToArray();

    public static readonly LogEntry[] Tasks =
        All.Where(x => x.Source is EditorLogSource.Task).ToArray();
}
