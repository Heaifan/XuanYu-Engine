using XuanYu.Engine.Core.Math;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection;

namespace XuanYu.Engine.Render.World;

/// <summary>
/// 把 Engine.WorldState 转换为 RenderScene (Z-Up)。
/// 使用 RenderUnitPlacement 作为渲染位置与 Picking 包围盒的唯一真源。
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

            if (position is null) continue;

            var pos = position.Value.Value;
            var placement = new RenderUnitPlacement(pos);

            objects.Add(new RenderObjectInfo(
                entity.EntityId,
                entity.DisplayName,
                pos,
                RenderObjectVisualKind.UnitMarker,
                entity.Source?.RelativePath,
                placement.SelectionBounds)
            {
                Placement = placement
            });
        }

        return new RenderScene(objects);
    }
}
