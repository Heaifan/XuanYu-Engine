namespace XuanYu.Core.History;

public sealed class HistorySnapshot
{
    internal HistorySnapshot(object entry, long beforeRevision,
        long afterRevision, long currentRevision, object historyItemIdentity)
    {
        Entry = entry;
        BeforeRevision = beforeRevision;
        AfterRevision = afterRevision;
        CurrentRevision = currentRevision;
        HistoryItemIdentity = historyItemIdentity;
    }

    public object Entry { get; }
    public long BeforeRevision { get; }
    public long AfterRevision { get; }
    public long CurrentRevision { get; }
    internal object HistoryItemIdentity { get; }

    public bool IdentifiesSameHistoryItem(HistorySnapshot other) =>
        other is not null && ReferenceEquals(HistoryItemIdentity, other.HistoryItemIdentity) &&
        BeforeRevision == other.BeforeRevision && AfterRevision == other.AfterRevision;
}
