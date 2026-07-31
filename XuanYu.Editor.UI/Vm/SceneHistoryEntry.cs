using XuanYu.Core.Identity;
using XuanYu.World;

namespace XuanYu.Editor.UI;

abstract record SceneHistoryEntry(EntityId EntityKey);

sealed record AddEntityHistoryEntry(WorldEntitySnapshot Snapshot)
    : SceneHistoryEntry(Snapshot.EntityKey);

sealed record DeleteEntityHistoryEntry(WorldEntitySnapshot Snapshot)
    : SceneHistoryEntry(Snapshot.EntityKey);

sealed record RenameEntityHistoryEntry(EntityId Key, string Before, string After)
    : SceneHistoryEntry(Key);
