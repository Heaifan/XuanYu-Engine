namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public EditorApplicationState ApplicationState => EditorApplicationStateProjection.Create(
        CurrentMode, CurrentWorkspace, _contextState.Snapshot, _editorState.ToolSnapshot,
        _editorState.Snapshot, _editorState.InteractionSnapshot, _authoringState.Snapshot,
        LastDrawTool,
        TerrainImportPercentage, TerrainImportMessage, IsTerrainImporting,
        new(NotificationLevel, NotificationText, NotificationCount));
}
