namespace XuanYu.Editor.Workspace;

// EDITOR-A-R1：Current Workspace 的唯一 Owner；不持有 World、Camera 或 UI 状态。
public sealed class EditorWorkspaceManager
{
    public EditorWorkspaceManager()
    {
        CurrentWorkspace = EditorWorkspaceDefinitions.MapEditor;
    }

    public EditorWorkspaceDefinition CurrentWorkspace { get; private set; }

    public EditorWorkspaceTransition Enter(EditorWorkspaceId id) => Switch(id);

    public EditorWorkspaceTransition Leave() => CreateTransition(
        CurrentWorkspace, CurrentWorkspace, false, true);

    public EditorWorkspaceTransition Switch(EditorWorkspaceId id)
    {
        var next = EditorWorkspaceDefinitions.Resolve(id);
        var previous = CurrentWorkspace;
        var changed = previous.Id != next.Id;
        if (changed) CurrentWorkspace = next;
        return CreateTransition(previous, CurrentWorkspace, changed, false);
    }

    static EditorWorkspaceTransition CreateTransition(
        EditorWorkspaceDefinition previous,
        EditorWorkspaceDefinition current,
        bool changed,
        bool isLeave) => new(
        previous,
        current,
        changed,
        isLeave,
        changed || isLeave,
        true,
        true,
        true);
}
