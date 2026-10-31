namespace XuanYu.Editor.Mode;

// EDITOR-A-R3：Manage/Edit 的唯一 Mode Owner；Workspace 仍由 EditorWorkspaceManager 独立持有。
public sealed class EditorModeManager
{
    public EditorModeId CurrentMode { get; private set; } = EditorModeId.Manage;

    public EditorModeTransition Toggle() => Switch(CurrentMode == EditorModeId.Manage
        ? EditorModeId.Edit : EditorModeId.Manage);

    public EditorModeTransition Switch(EditorModeId next)
    {
        var previous = CurrentMode;
        var changed = previous != next;
        if (changed) CurrentMode = next;
        return new EditorModeTransition(previous, CurrentMode, changed, changed,
            true, true, true, true, true);
    }
}
