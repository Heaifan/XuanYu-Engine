namespace FluidWarfare.Editor.Windows.Viewport.Project;

/// <summary>项目启动结果。Shell 根据结果应用 UI。</summary>
public sealed record ProjectBootstrapResult(
    bool Success,
    string LogLevel,   // "info" | "warning" | "error"
    string LogMessage,
    string? StatusBarSelection)
{
    public static ProjectBootstrapResult Failed(string msg) =>
        new(false, "error", msg, "项目加载失败");
}
