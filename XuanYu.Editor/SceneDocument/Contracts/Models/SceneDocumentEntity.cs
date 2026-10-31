using XuanYu.Core.Identity;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.SceneDocument;

public readonly record struct SceneDocumentEntity(
    EntityId Id,
    string Name,
    EntityId ParentId,
    int SiblingOrder,
    CommittedTransform Transform,
    string EntityType = "LegacyMinimalTriangle",
    string? ModelAssetId = null);
