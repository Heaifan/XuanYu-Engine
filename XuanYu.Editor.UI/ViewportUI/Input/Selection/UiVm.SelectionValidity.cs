namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool IsSelectedEntityValid()
    {
        if (!TrySelectedEntityKey(out var key)) return true;
        return _selectionCoordinator.IsEntityValid(key);
    }

    void ClearInvalidEntitySelection(string source)
    {
        if (!_editorState.Snapshot.HasSelection || IsSelectedEntityValid()) return;
        ApplySelectionCommand(new ClearEditorSelectionCommand(), source);
    }
}
