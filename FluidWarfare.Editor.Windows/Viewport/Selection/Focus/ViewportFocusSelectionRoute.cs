using FluidWarfare.Engine.World;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Viewport.Selection.Focus;

using Route;

/// <summary>视口获得焦点时的选择路由。决定应聚焦哪个实体（或无），并返回完整展示结果。</summary>
public sealed class ViewportFocusSelectionRoute
{
    public ViewportFocusSelectionResult Focus(WorldState? world, EditorSelectionRoute selectionRoute)
    {
        // 已有选中实体且 World 存在 → 保持选择
        if (selectionRoute.State.SelectedWorldEntity is not null && world is not null)
        {
            var entity = selectionRoute.State.SelectedWorldEntity;
            return new(
                CreateEntitySelection(entity, world),
                entity.DisplayName, false,
                ["视口获得焦点。", $"当前 World 占位实体：{entity.DisplayName}。"],
                [], entity);
        }

        if (world is not null)
        {
            var entities = world.ListEntities();
            if (entities.Count > 0)
            {
                // 自动选择首个实体
                var r = selectionRoute.SelectEntity(new EditorSelectionRequest(
                    entities[0].EntityId.Value.ToString(),
                    EditorSelectionReason.ViewportFocused, world));
                if (r.Entity is not null)
                {
                    return new(
                        CreateEntitySelection(r.Entity, world),
                        r.Entity.DisplayName, false,
                        ["视口获得焦点。"], [], r.Entity);
                }
            }
            // World 为空
            return ViewportFocusSelectionResult.EmptyWorld;
        }

        // World 未创建
        return ViewportFocusSelectionResult.NoWorld;
    }

    private static EditorSelection CreateEntitySelection(WorldEntityInfo entity, WorldState world)
    {
        var pos = world.FindPosition(entity.EntityId);
        var typeLabel = entity.Source is not null ? "World 占位实体" : "World 实体";
        var desc = pos is not null
            ? $"EntityId({entity.EntityId.Value})，来源：{entity.Source?.RelativePath ?? "无"}，位置：({pos.Value.Value.X}, {pos.Value.Value.Y}, {pos.Value.Value.Z})"
            : $"EntityId({entity.EntityId.Value})，来源：{entity.Source?.RelativePath ?? "无"}";
        return new EditorSelection(typeLabel, entity.DisplayName, desc);
    }
}
