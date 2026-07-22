using XuanYu.Core.World;

namespace XuanYu.Core.Scene;

public static class SceneWorldProjection
{
    public static SceneEntitySnapshot ToSceneEntity(WorldEntitySnapshot entity) =>
        new(entity.EntityKey, entity.Name, entity.Type, entity.Transform);

    public static SceneRenderSnapshot ToRenderSnapshot(
        WorldEntitySnapshot? entity,
        bool isSelected = false)
    {
        return entity is null
            ? SceneRenderSnapshot.Empty
            : new SceneRenderSnapshot(ToSceneEntity(entity.Value), isSelected);
    }
}
