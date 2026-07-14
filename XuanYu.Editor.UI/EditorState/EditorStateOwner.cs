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

    public EditorStateChangedResult Select(SelectEditorItemCommand command)
    {
        EnsureWriteThread();
        if (string.IsNullOrWhiteSpace(command.Source))
        {
            throw new ArgumentException("选择来源不能为空。", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Item))
        {
            throw new ArgumentException("选择对象不能为空。", nameof(command));
        }

        var old = _snapshot;
        var title = command.Item.TrimStart(' ', '├', '└', '─');
        _snapshot = new EditorSelectionSnapshot(
            old.Revision + 1,
            true,
            title,
            command.Source);
        return Changed(old, EditorStateChangeKind.SelectionChanged);
    }

    public EditorStateChangedResult Clear(ClearEditorSelectionCommand command)
    {
        EnsureWriteThread();
        var old = _snapshot;
        _snapshot = new EditorSelectionSnapshot(
            old.Revision + 1,
            false,
            "未选择对象",
            "无");
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
