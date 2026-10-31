using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;

namespace XuanYu.Editor.SceneDocument;

static class SceneDocumentMapper
{
    public static SceneDocumentJson ToJson(SceneDocumentSnapshot snapshot) =>
        new("XuanYuScene", 4, new SceneInfoJson(snapshot.SceneId, snapshot.SceneName),
            snapshot.Entities.OrderBy(e => e.SiblingOrder).ThenBy(e => e.Id.Value).Select(ToJson).ToArray(),
            snapshot.Assets?.OrderBy(a => a.AssetId, StringComparer.Ordinal).Select(ToJson).ToArray(),
            snapshot.MapReference is { } map
                ? new MapReferenceJson(map.MapId, map.AssetPath)
                : null);

    public static SceneEntityJson ToJson(SceneDocumentEntity entity)
    {
        var t = entity.Transform;
        return new SceneEntityJson(entity.Id.Value, entity.Name, entity.EntityType,
            entity.ParentId.IsValid ? entity.ParentId.Value : null, entity.SiblingOrder,
            ToJson(t.Position), ToJson(t.Rotation), ToJson(t.Scale), entity.ModelAssetId);
    }

    public static SceneAssetJson ToJson(SceneDocumentAsset asset) =>
        new(asset.AssetId, asset.Kind, asset.RelativePath, asset.DisplayName, asset.ImporterVersion);

    public static SceneDocumentSnapshot ToSnapshot(SceneDocumentJson doc) =>
        new(doc.Scene.Id, doc.Scene.Name, doc.Entities.Select(ToEntity).ToArray(),
            doc.Assets?.Select(ToAsset).ToArray(),
            doc.MapReference is { } map ? new MapReference(map.MapId, map.AssetPath) : null);

    static SceneDocumentEntity ToEntity(SceneEntityJson entity) =>
        new(EntityId.FromInt(entity.Id), entity.Name,
            entity.ParentId is > 0 ? EntityId.FromInt(entity.ParentId.Value) : EntityId.None,
            entity.SiblingOrder,
            new CommittedTransform(ToVector(entity.Position), ToVector(entity.Rotation), ToVector(entity.Scale)),
            entity.EntityType ?? WorldEntityTypes.LegacyMinimalTriangle,
            entity.ModelAssetId);

    static SceneDocumentAsset ToAsset(SceneAssetJson asset) =>
        new(asset.AssetId, asset.Kind, asset.RelativePath, asset.DisplayName, asset.ImporterVersion);

    static Vector3Json ToJson(Vector3d v) => new(v.X, v.Y, v.Z);

    static Vector3d ToVector(Vector3Json v) => new(v.X, v.Y, v.Z);
}
