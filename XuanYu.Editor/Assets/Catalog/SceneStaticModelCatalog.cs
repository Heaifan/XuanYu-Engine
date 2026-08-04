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

    // D4：加载事务候选提交。整体替换目录内容；SourcePath 已解析为托管绝对路径。
    public void ReplaceAll(IEnumerable<SceneStaticModelBinding> bindings, IReadOnlyDictionary<AssetId, StaticModelData> models)
    {
        _byEntity.Clear();
        _byAsset.Clear();
        foreach (var binding in bindings)
        {
            if (!binding.EntityId.IsValid || !binding.AssetId.IsValid) continue;
            _byEntity[binding.EntityId] = binding;
        }

        foreach (var (assetId, model) in models)
        {
            if (assetId.IsValid && model is not null) _byAsset[assetId] = model;
        }

        _revision++;
        Changed?.Invoke();
    }

    // D4：保存成功后把外部 SourcePath 改绑为托管 .xyassets 内绝对路径。
    public void RebindSourcePaths(IReadOnlyDictionary<EntityId, string> hostedPaths)
    {
        foreach (var (entityId, hostedPath) in hostedPaths)
        {
            if (!_byEntity.TryGetValue(entityId, out var binding)) continue;
            _byEntity[entityId] = binding with { SourcePath = hostedPath };
        }

        _revision++;
        Changed?.Invoke();
    }

    public IReadOnlyList<SceneStaticModelBinding> Snapshot =>
        _byEntity.Values.OrderBy(binding => binding.AssetId.Value).ToArray();
}
