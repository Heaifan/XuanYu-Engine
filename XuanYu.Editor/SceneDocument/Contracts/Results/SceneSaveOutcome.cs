using XuanYu.Core.Identity;

namespace XuanYu.Editor.SceneDocument;

// D4：保存事务结果。SavedSnapshot 带 v3 Assets；HostedSourcePaths 是
// 实体 → 托管 .xyassets 内绝对路径 的改绑映射（保存成功后 Catalog 使用）。
public sealed record SceneSaveOutcome(
    SceneDocumentSnapshot SavedSnapshot,
    IReadOnlyDictionary<EntityId, string> HostedSourcePaths);
