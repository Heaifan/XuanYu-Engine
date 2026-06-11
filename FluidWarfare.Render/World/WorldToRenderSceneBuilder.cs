using FluidWarfare.Engine.World;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Render.World;

/// <summary>
/// 把 Engine.WorldState 转换为 RenderScene。
/// 不写日志，不依赖 Editor，不依赖 Project。
/// </summary>
public static class WorldToRenderSceneBuilder
{
    /// <summary>
    /// 将 WorldState 中的所有实体转换为可渲染对象。
    /// </summary>
    /// <param name="worldState">World 状态，不能为空。</param>
    /// <exception cref="ArgumentNullException">worldState 为空时抛出。</exception>
    public static RenderScene Build(WorldState worldState)
    {
        if (worldState is null)
        {
            throw new ArgumentNullException(nameof(worldState));
        }

        var entities = worldState.ListEntities();
        if (entities.Count == 0)
        {
            return RenderScene.Empty;
        }

        var objects = new List<RenderObjectInfo>(entities.Count);

        foreach (var entity in entities)
        {
            var position = worldState.FindPosition(entity.EntityId);

            // 跳过没有位置的实体（当前 WorldState 保证都有，但防御性处理）
            if (position is null)
            {
                continue;
            }

            objects.Add(new RenderObjectInfo(
                entity.EntityId,
                entity.DisplayName,
                position.Value.Value,
                RenderObjectVisualKind.UnitMarker,
                entity.Source?.RelativePath));
        }

        return new RenderScene(objects);
    }
}
