namespace XuanYu.Editor.UI;

public enum EditorInteractionChangeKind { Began, Previewed, Committed, Canceled }

public sealed record EditorInteractionChangedResult(
    long OldRevision,
    long NewRevision,
    EditorInteractionChangeKind ChangeKind,
    EditorInteractionSnapshot OldSnapshot,
    EditorInteractionSnapshot Snapshot);
