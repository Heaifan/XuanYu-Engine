namespace XuanYu.Editor.UI;

public enum EditorStateChangeKind
{
    SelectionChanged,
    SelectionCleared
}

public sealed record EditorStateChangedResult(
    long OldRevision,
    long NewRevision,
    EditorStateChangeKind ChangeKind,
    EditorSelectionSnapshot Snapshot);
