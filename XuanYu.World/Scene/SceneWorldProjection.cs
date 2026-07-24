using XuanYu.World;

using XuanYu.Core.Scene;
namespace XuanYu.World.Scene;

public static class SceneWorldProjection
{
    public static SceneEntitySnapshot ToSceneEntity(WorldEntitySnapshot entity) =>
        new(entity.EntityKey, entity.Name, entity.Type, entity.Transform);

    public static SceneRenderSnapshot ToRenderSnapshot(
        WorldEntitySnapshot? entity,
        IReadOnlyList<WorldEntitySnapshot>? allEntities = null,
        bool isSelected = false)
    {
        var renderEntities = allEntities?.Select(ToSceneEntity).ToArray();
        return entity is null
            ? SceneRenderSnapshot.Empty
            : new SceneRenderSnapshot(
                ToSceneEntity(entity.Value),
                isSelected,
                RenderEntities: renderEntities);
    }
}
