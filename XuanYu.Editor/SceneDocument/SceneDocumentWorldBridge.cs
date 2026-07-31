using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.SceneDocument;

public static class SceneDocumentWorldBridge
{
    static readonly SpatialAabb MinimalExtent =
        new(new Vector3d(-0.5, -0.5, -0.5), new Vector3d(0.5, 0.5, 0.5));

    public static SceneDocumentSnapshot Capture(
        SceneStateOwner scene,
        string sceneId,
        string sceneName)
    {
        var entities = scene.Entities.OrderBy(e => e.SiblingOrder).ThenBy(e => e.EntityKey.Value)
            .Select(e => new SceneDocumentEntity(
                e.EntityKey, e.Name, e.ParentId, e.SiblingOrder, e.Transform,
                NormalizeType(e.Type)))
            .ToArray();
        return new SceneDocumentSnapshot(sceneId, sceneName, entities);
    }

    public static IReadOnlyList<WorldEntitySnapshot> ToWorld(
        SceneDocumentSnapshot snapshot,
        IWorldPartitionStrategy partitionStrategy)
    {
        return snapshot.Entities.OrderBy(e => e.SiblingOrder).ThenBy(e => e.Id.Value)
            .Select(e =>
            {
                var region = partitionStrategy.RegionFor(e.Transform.Position);
                return new WorldEntitySnapshot(e.Id, e.Name, NormalizeType(e.EntityType),
                    e.Transform, e.Transform.Position, region,
                    WorldEntityActivity.Active, MinimalExtent, e.ParentId, e.SiblingOrder);
            })
            .ToArray();
    }

    static string NormalizeType(string type) =>
        WorldEntityTypes.TryParse(type, out var parsed)
            ? parsed.ToString()
            : throw new ArgumentException($"Unsupported entity type: {type}");
}
