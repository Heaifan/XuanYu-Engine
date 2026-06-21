namespace FluidWarfare.Editor.Windows.Shell.Lifecycle;

/// <summary>DetachRoute → Shell 的结果。Shell 据此恢复状态标志。</summary>
public sealed record EditorShellDetachResult(
    bool SessionStopped,
    bool TimerCleanedUp);
