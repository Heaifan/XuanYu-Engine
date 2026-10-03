namespace XuanYu.Editor;

public sealed class EditorContextOwner
{
    EditorContextSnapshot _snapshot = EditorContextSnapshot.Initial;
    public EditorContextSnapshot Snapshot => _snapshot;

    public EditorContextSnapshot Change(ChangeEditorContextCommand command)
    {
        if (_snapshot.Context == command.Context) return _snapshot;
        _snapshot = new(_snapshot.Revision + 1, command.Context);
        return _snapshot;
    }
}
