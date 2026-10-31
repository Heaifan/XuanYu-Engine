namespace XuanYu.Editor.Mode;

// EDITOR-A-R3：Mode 切换的无副作用合同，由 UI Shell 复用既有取消链执行。
public sealed record EditorModeTransition(
    EditorModeId PreviousMode,
    EditorModeId CurrentMode,
    bool Changed,
    bool EndsTemporaryInput,
    bool PreservesWorld,
    bool PreservesCamera,
    bool PreservesSelection,
    bool PreservesAssets,
    bool PreservesViewport);
