using XuanYu.Core.Identity;

namespace XuanYu.Editor.Assets;

// D3：场景静态模型绑定目录。Editor 层唯一事实源：实体 → 资产 → 模型数据。
// 不存储 GPU 资源（Editor 不引用 Render.Abstractions）；UI 层按 AssetId 派生
// RenderStaticModelKey 并转换为 RenderStaticModelResource。
public sealed class SceneStaticModelCatalog
{
    readonly Dictionary<EntityId, SceneStaticModelBinding> _byEntity = new();
    readonly Dictionary<AssetId, StaticModelData> _byAsset = new();
    long _revision;

    public long Revision => _revision;

    public event Action? Changed;

    public bool Bind(
        EntityId entityId,
        AssetId assetId,
        string sourcePath,
        StaticModelData model)
    {
        if (!entityId.IsValid || !assetId.IsValid) return false;
        if (_byEntity.ContainsKey(entityId)) return false;
        _byEntity[entityId] = new SceneStaticModelBinding(entityId, assetId, sourcePath);
        _byAsset[assetId] = model;
        _revision++;
        Changed?.Invoke();
        return true;
    }

    public bool TryGetByEntity(EntityId entityId, out SceneStaticModelBinding binding) =>
        _byEntity.TryGetValue(entityId, out binding);

    public bool TryGetByAsset(AssetId assetId, out StaticModelData? model) =>
        _byAsset.TryGetValue(assetId, out model);

    public bool Remove(EntityId entityId)
    {
        if (!_byEntity.Remove(entityId)) return false;
        _revision++;
        Changed?.Invoke();
        return true;
    }

    public void Clear()
    {
        _byEntity.Clear();
        _byAsset.Clear();
        _revision++;
        Changed?.Invoke();
    }

    public IReadOnlyList<SceneStaticModelBinding> Snapshot =>
        _byEntity.Values.OrderBy(binding => binding.AssetId.Value).ToArray();
}
