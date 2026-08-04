using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;
using XuanYu.World;

namespace XuanYu.Editor.UI;

abstract record SceneHistoryEntry(EntityId EntityKey);

sealed record AddEntityHistoryEntry(WorldEntitySnapshot Snapshot, SceneStaticModelBinding? Binding = null)
    : SceneHistoryEntry(Snapshot.EntityKey);

sealed record DeleteEntityHistoryEntry(WorldEntitySnapshot Snapshot, SceneStaticModelBinding? Binding = null)
    : SceneHistoryEntry(Snapshot.EntityKey);

sealed record RenameEntityHistoryEntry(EntityId Key, string Before, string After)
    : SceneHistoryEntry(Key);
