namespace XuanYu.Editor;

public sealed record EditorToolChangedResult(
    long OldRevision,
    long NewRevision,
    EditorToolSnapshot OldSnapshot,
    EditorToolSnapshot Snapshot);
