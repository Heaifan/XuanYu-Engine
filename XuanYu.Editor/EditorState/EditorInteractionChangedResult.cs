namespace XuanYu.Editor;

public enum EditorInteractionChangeKind { Began, Previewed, Committed, Canceled }

public sealed record EditorInteractionChangedResult(
    long OldRevision,
    long NewRevision,
    EditorInteractionChangeKind ChangeKind,
    EditorInteractionSnapshot OldSnapshot,
    EditorInteractionSnapshot Snapshot);
