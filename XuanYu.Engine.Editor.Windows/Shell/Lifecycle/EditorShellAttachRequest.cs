namespace XuanYu.Engine.Editor.Windows.Shell.Lifecycle;

/// <summary>Shell → AttachRoute 的请求。NativeHostReport 和 InputPipelineInit 在 UI 线程首次空闲时按序执行。</summary>
public sealed record EditorShellAttachRequest(
    Action NativeHostReportAction,
    Action InputPipelineInitAction);
