using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.Editor.SceneDocument;

// D4：保存完整事务。候选构建 → Hosting Prepare/Activate → 原子写 .xyscene
// → Complete；场景写入失败时 Rollback 恢复旧目录。不得直接修改正式状态。
public sealed class SceneDocumentSaveTransaction
{
    readonly SceneStorageService _storage;

    public SceneDocumentSaveTransaction(SceneStorageService storage) => _storage = storage;

    public async Task<SceneDocumentResult<SceneSaveOutcome>> ExecuteAsync(
        string targetPath,
        SceneDocumentSnapshot snapshot,
        IReadOnlyList<SceneStaticModelBinding> bindings)
    {
        // 合同 §10.1：StaticModel 实体必须存在 Catalog Binding，否则保存失败。
        var boundEntities = bindings.Select(b => b.EntityId).ToHashSet();
        var unboundStatic = snapshot.Entities
            .FirstOrDefault(e => e.EntityType == XuanYu.World.WorldEntityTypes.StaticModel &&
                !boundEntities.Contains(e.Id));
        if (unboundStatic.EntityType is not null)
            return Fail("MissingStaticModelBinding", "静态模型实体缺少托管绑定，保存失败。",
                $"entity={unboundStatic.Id} name={unboundStatic.Name}");

        var planResult = SceneAssetHostingPlanner.Create(targetPath, bindings);
        if (!planResult.Succeeded) return Fail(planResult.ErrorCode, planResult.Message, planResult.Detail);

        var plan = planResult.Value!;
        var hosting = SceneAssetHostingTransaction.Prepare(plan);
        if (hosting.State != SceneAssetHostingState.Prepared)
            return Fail(hosting.LastError, "托管资产准备失败。", "");
        if (!hosting.Activate().Succeeded)
            return Fail(SceneAssetHostingError.ActivateFailed, "托管资产激活失败。", hosting.LastError);

        var assets = plan.Assets
            .Select(a => SceneDocumentAsset.ModelGltf(
                a.AssetId.Value, a.RelativePath,
                Path.GetFileName(a.SourcePath) ?? a.AssetId.Value, 1))
            .ToArray();
        var saved = snapshot with { Assets = assets };

        var write = await _storage.SaveAsync(targetPath, saved);
        if (!write.Succeeded)
        {
            hosting.Rollback();
            return Fail(write.ErrorCode, write.Message, write.Detail);
        }

        if (!hosting.Complete().Succeeded)
            return Fail(SceneAssetHostingError.CompleteFailed, "托管资产收尾失败。", hosting.LastError);

        var hosted = plan.Assets.ToDictionary(
            a => a.AssetId,
            a => a.FinalPath,
            new AssetIdComparer());
        var rebind = bindings
            .Where(b => hosted.TryGetValue(b.AssetId, out _))
            .ToDictionary(b => b.EntityId, b => hosted[b.AssetId]);
        return SceneDocumentResult<SceneSaveOutcome>.Ok(new SceneSaveOutcome(saved, rebind));
    }

    sealed class AssetIdComparer : IEqualityComparer<AssetId>
    {
        public bool Equals(AssetId x, AssetId y) => x.Value == y.Value;
        public int GetHashCode(AssetId obj) => obj.Value.GetHashCode();
    }

    static SceneDocumentResult<SceneSaveOutcome> Fail(string code, string message, string detail) =>
        SceneDocumentResult<SceneSaveOutcome>.Fail(code, message, "Save", detail);
}
