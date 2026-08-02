using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.Assets;
using XuanYu.World;

namespace XuanYu.Editor.SceneDocument;

// D4：加载候选阶段。只读构建候选，不修改当前 World/Catalog/Selection/History/Dirty。
public sealed class SceneDocumentLoadTransaction
{
    static readonly SpatialAabb PlaceholderExtent =
        new(new Vector3d(-0.5, -0.5, -0.5), new Vector3d(0.5, 0.5, 0.5));

    readonly SceneStorageService _storage;
    readonly GlbImportService _importer;
    readonly IWorldPartitionStrategy _partitionStrategy;

    public SceneDocumentLoadTransaction(
        SceneStorageService storage,
        GlbImportService importer,
        IWorldPartitionStrategy partitionStrategy)
    {
        _storage = storage;
        _importer = importer;
        _partitionStrategy = partitionStrategy;
    }

    public async Task<SceneDocumentResult<SceneLoadCandidate>> BuildCandidateAsync(string path)
    {
        var loaded = await _storage.LoadAsync(path);
        if (!loaded.Succeeded || loaded.Value is null)
            return Fail(loaded.ErrorCode, loaded.Message, loaded.Detail);
        var snapshot = loaded.Value!;

        var assetRoot = AssetRootFor(path);
        var assets = snapshot.Assets ?? [];
        var assetPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var models = new Dictionary<AssetId, StaticModelData>();
        var missing = 0;
        var failed = 0;

        foreach (var asset in assets)
        {
            if (!AssetId.TryParse(asset.AssetId, out var assetId))
                return Fail("InvalidAssetId", "资产ID非法。", asset.AssetId);
            if (!SceneAssetPathPolicy.TryResolveManagedPath(assetRoot, asset.RelativePath, out var fullPath))
                return Fail("UnsafeAssetPath", "资产路径不安全。", asset.RelativePath);
            assetPaths[asset.AssetId] = fullPath;
            if (!File.Exists(fullPath)) { missing++; continue; }
            var imported = _importer.ImportFile(fullPath);
            if (!imported.Succeeded) { failed++; continue; }
            models[assetId] = imported.Model!;
        }

        var bindings = new List<SceneStaticModelBinding>();
        foreach (var entity in snapshot.Entities)
        {
            if (entity.EntityType != WorldEntityTypes.StaticModel || entity.ModelAssetId is null) continue;
            if (!assetPaths.TryGetValue(entity.ModelAssetId, out var modelPath)) continue;
            bindings.Add(new SceneStaticModelBinding(entity.Id, ParseAssetId(entity.ModelAssetId), modelPath));
        }

        var entities = snapshot.Entities
            .OrderBy(e => e.SiblingOrder).ThenBy(e => e.Id.Value)
            .Select(e =>
            {
                var region = _partitionStrategy.RegionFor(e.Transform.Position);
                var extent = ExtentFor(e, models);
                return new WorldEntitySnapshot(e.Id, e.Name, e.EntityType, e.Transform,
                    e.Transform.Position, region, WorldEntityActivity.Active, extent, e.ParentId, e.SiblingOrder);
            })
            .ToArray();

        return SceneDocumentResult<SceneLoadCandidate>.Ok(new SceneLoadCandidate(
            snapshot, entities, bindings, models, missing, failed));
    }

    static SpatialAabb ExtentFor(SceneDocumentEntity e, IReadOnlyDictionary<AssetId, StaticModelData> models)
    {
        if (e.EntityType != WorldEntityTypes.StaticModel || e.ModelAssetId is null) return PlaceholderExtent;
        if (!AssetId.TryParse(e.ModelAssetId, out var assetId)) return PlaceholderExtent;
        return models.TryGetValue(assetId, out var model) ? model.LocalBounds : PlaceholderExtent;
    }

    static AssetId ParseAssetId(string value) =>
        AssetId.TryParse(value, out var id) ? id : default;

    static string AssetRootFor(string scenePath)
    {
        var directory = Path.GetDirectoryName(scenePath) ?? "";
        var name = Path.GetFileNameWithoutExtension(scenePath);
        return Path.Combine(directory, name + SceneAssetPathPolicy.AssetFolderExtension);
    }

    static SceneDocumentResult<SceneLoadCandidate> Fail(string code, string message, string detail) =>
        SceneDocumentResult<SceneLoadCandidate>.Fail(code, message, "Load", detail);
}
