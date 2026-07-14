namespace XuanYu.Editor.UI;

public sealed class EditorStateOwner
{
    readonly Func<bool> _isWriteThread;
    EditorSelectionSnapshot _snapshot = EditorSelectionSnapshot.Initial;

    public EditorStateOwner(Func<bool> isWriteThread)
    {
        _isWriteThread = isWriteThread;
    }

    public EditorSelectionSnapshot Snapshot => _snapshot;

    public EditorStateChangedResult? Select(SelectEditorItemCommand command)
    {
        EnsureWriteThread();
        if (string.IsNullOrWhiteSpace(command.Source))
        {
            throw new ArgumentException("选择来源不能为空。", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Key))
        {
            throw new ArgumentException("选择身份不能为空。", nameof(command));
        }

        var old = _snapshot;
        if (old.HasSelection && old.SelectionKey == command.Key)
        {
            return null;
        }

        _snapshot = new EditorSelectionSnapshot(
            old.Revision + 1,
            true,
            command.Key,
            command.Title,
            command.Type,
            command.Path);
        return Changed(old, EditorStateChangeKind.SelectionChanged);
    }

    public EditorStateChangedResult? Clear(ClearEditorSelectionCommand command)
    {
        EnsureWriteThread();
        var old = _snapshot;
        if (!old.HasSelection)
        {
            return null;
        }

        _snapshot = new EditorSelectionSnapshot(
            old.Revision + 1,
            false,
            "",
            "未选择对象",
            "无",
            "");
        return Changed(old, EditorStateChangeKind.SelectionCleared);
    }

    void EnsureWriteThread()
    {
        if (!_isWriteThread())
        {
            throw new InvalidOperationException("Editor State Owner 只允许在 UI 线程写入。");
        }
    }

    EditorStateChangedResult Changed(EditorSelectionSnapshot old, EditorStateChangeKind kind) =>
        new(old.Revision, _snapshot.Revision, kind, _snapshot);
}
