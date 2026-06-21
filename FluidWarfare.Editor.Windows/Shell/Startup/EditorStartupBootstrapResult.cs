using FluidWarfare.Project.Metadata;

namespace FluidWarfare.Editor.Windows.Shell.Startup;

/// <summary>启动引导的完整结果。Shell 根据此结果应用 UI。</summary>
public sealed record EditorStartupBootstrapResult(
    bool Success,
    GameProjectInfo? Project,
    EditorStartupWorldResult? WorldResult,
    string? FailureMessage,
    IReadOnlyList<string> LogMessages,
    IReadOnlyList<string> LogWarnings)
{
    public static EditorStartupBootstrapResult Failed(string msg) =>
        new(false, null, null, msg, [], []);

    public static EditorStartupBootstrapResult Succeeded(
        GameProjectInfo project, EditorStartupWorldResult? world) =>
        new(true, project, world, null,
            world?.LogMessages ?? [], world?.LogWarnings ?? []);
}
