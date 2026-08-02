using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.Editor.SceneDocument;

// D4：加载候选。候选阶段构建，提交阶段一次性替换 World/Catalog。
public sealed record SceneLoadCandidate(
    SceneDocumentSnapshot Snapshot,
    IReadOnlyList<World.WorldEntitySnapshot> Entities,
    IReadOnlyList<SceneStaticModelBinding> Bindings,
    IReadOnlyDictionary<AssetId, StaticModelData> Models,
    int MissingCount,
    int FailedCount)
{
    public bool HasUnavailableAssets => MissingCount > 0 || FailedCount > 0;
}
