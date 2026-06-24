namespace XuanYu.Engine.Editor.Windows.Shell.Scene3D.Commands;

/// <summary>Scene3dCommandRoute → Shell 的结果。</summary>
public sealed record EditorScene3dCommandResult(
    bool SessionStarted,
    bool NeedsDiagnosticsRefresh,
    int NewRenderSeq,
    bool NeedsTransformInit);
