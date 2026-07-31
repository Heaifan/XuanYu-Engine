using XuanYu.Core.History;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool ApplyHistory(object entry, bool undo)
    {
        return entry switch
        {
            TransformHistoryEntry transform => _sceneState.RestoreTransform(
                transform.EntityKey, undo ? transform.Before : transform.After),
            AddEntityHistoryEntry add => ApplyEntityPresence(add.Snapshot, !undo),
            DeleteEntityHistoryEntry delete => ApplyEntityPresence(delete.Snapshot, undo),
            RenameEntityHistoryEntry rename => _sceneState.RenameEntity(
                rename.EntityKey, undo ? rename.Before : rename.After, out _),
            _ => false
        };
    }

    bool ApplyEntityPresence(WorldEntitySnapshot snapshot, bool present)
    {
        if (present)
        {
            if (!_sceneState.RestoreEntity(snapshot)) return false;
            SelectEntity(snapshot.EntityKey, "历史恢复实体");
            return true;
        }
        if (!_sceneState.DestroyEntity(snapshot.EntityKey)) return false;
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "历史移除实体");
        return true;
    }
}
