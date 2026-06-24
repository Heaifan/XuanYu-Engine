using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Scene.Position;

/// <summary>
/// 替换 RenderScene 中指定 EntityId 的 Position 和 SelectionBounds。
/// 使用 RenderUnitPlacement 统一计算，确保渲染与 Picking 对齐。
/// </summary>
public static class RenderSceneObjectPositionWriter
{
    /// <summary>
    /// 替换指定实体的位置和 SelectionBounds。
    /// </summary>
    /// <param name="scene">当前 RenderScene。</param>
    /// <param name="entityId">目标实体 ID。</param>
    /// <param name="newPosition">新世界位置。</param>
    /// <returns>写入结果，包含新的 RenderScene 实例。</returns>
    public static RenderObjectPositionWriteResult Update(
        RenderScene scene,
        EntityId entityId,
        Vector3d newPosition)
    {
        var oldList = scene.Objects;
        var found = false;
        RenderObjectPositionChange? change = null;
        var newObjects = new List<RenderObjectInfo>(oldList.Count);

        foreach (var obj in oldList)
        {
            if (obj.EntityId == entityId)
            {
                found = true;
                change = new RenderObjectPositionChange(entityId, obj.Position, newPosition);

                if (obj.Position == newPosition)
                {
                    return new RenderObjectPositionWriteResult(
                        true, false, "位置未变化。", scene, change);
                }

                // 使用 RenderUnitPlacement 统一计算
                var placement = new RenderUnitPlacement(newPosition);

                newObjects.Add(new RenderObjectInfo(
                    obj.EntityId, obj.DisplayName, newPosition,
                    obj.VisualKind, obj.SourcePath, placement.SelectionBounds)
                {
                    Placement = placement
                });
            }
            else
            {
                newObjects.Add(obj);
            }
        }

        if (!found)
        {
            return new RenderObjectPositionWriteResult(
                false, false,
                $"EntityId {entityId.Value} 在 RenderScene 中未找到。",
                scene, null);
        }

        return new RenderObjectPositionWriteResult(
            true, true, string.Empty,
            new RenderScene(newObjects), change);
    }
}
