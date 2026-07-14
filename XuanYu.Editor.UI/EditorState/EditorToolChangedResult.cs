namespace XuanYu.Editor.UI;

public sealed record EditorToolChangedResult(
    long OldRevision,
    long NewRevision,
    EditorToolSnapshot OldSnapshot,
    EditorToolSnapshot Snapshot);
