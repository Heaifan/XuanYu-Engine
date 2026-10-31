namespace XuanYu.Editor.Workspace;

public sealed class EditorWorkspaceTransitionCoordinator(EditorWorkspaceManager owner)
{
    public EditorWorkspaceDefinition CurrentWorkspace => owner.CurrentWorkspace;

    public bool Switch(EditorWorkspaceId target, bool isEditing, Action cancelInteraction,
        Action clearSelection, Action syncRegionContext, Action resetTool,
        Action<EditorWorkspaceTransition> publish)
    {
        if (owner.CurrentWorkspace.Id == target) return false;
        if (isEditing) cancelInteraction();
        var transition = owner.Switch(target);
        if (!transition.Changed) return false;
        clearSelection();
        if (target == EditorWorkspaceId.RegionEditor) syncRegionContext();
        if (isEditing) resetTool();
        publish(transition);
        return true;
    }
}
