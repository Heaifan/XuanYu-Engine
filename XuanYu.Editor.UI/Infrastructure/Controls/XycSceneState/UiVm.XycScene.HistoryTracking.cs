using XuanYu.Core.Identity;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void TrackXycUndoTop(EntityId expectedEntity)
    {
        if (!_historyOwner.TryPeekUndoAny(out var top) ||
            !TryDescribeXycEntry(top.Entry, out _, out var entity) || entity != expectedEntity)
        {
            MarkXycInDoubt("WORLD_COMMITTED_HISTORY_OWNERSHIP_UNCONFIRMED");
            throw new XycSceneOperationException("HISTORY_COMMIT_UNCONFIRMED");
        }
        _xycHistoryItems.Add(top);
    }
}
