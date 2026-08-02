using XuanYu.Core.History;
using XuanYu.Editor.Assets;
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
            AddEntityHistoryEntry add => ApplyEntityPresence(add.Snapshot, !undo, add.Binding),
            DeleteEntityHistoryEntry delete => ApplyEntityPresence(delete.Snapshot, undo, delete.Binding),
            RenameEntityHistoryEntry rename => _sceneState.RenameEntity(
                rename.EntityKey, undo ? rename.Before : rename.After, out _),
            _ => false
        };
    }

    bool ApplyEntityPresence(WorldEntitySnapshot snapshot, bool present, SceneStaticModelBinding? binding)
    {
        if (present)
        {
            if (!_sceneState.RestoreEntity(snapshot)) return false;
            if (binding is { } b)
            {
                if (!_staticModelCatalog.TryGetByAsset(b.AssetId, out var model) || model is null)
                    return false;
                if (!_staticModelCatalog.Bind(b.EntityId, b.AssetId,
                        b.SourcePath, model)) return false;
            }
            SelectEntity(snapshot.EntityKey, "历史恢复实体");
            return true;
        }

        if (!_sceneState.DestroyEntity(snapshot.EntityKey)) return false;
        if (binding is { } removed) _staticModelCatalog.Remove(removed.EntityId);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "历史移除实体");
        return true;
    }
}
