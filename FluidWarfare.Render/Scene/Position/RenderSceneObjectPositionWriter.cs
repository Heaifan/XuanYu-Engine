using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Scene.Position;

/// <summary>
/// 替换 RenderScene 中指定 EntityId 的 Position 和 SelectionBounds。
/// 创建新的 RenderScene 实例（不可变快照替换）。
/// </summary>
public static class RenderSceneObjectPositionWriter
{
    // Constants matching WorldToRenderSceneBuilder
    private const double UnitScale = 1.25;
    private const double UnitHalfExtent = 0.5 * UnitScale;
    private const double UnitYOffset = 0.5;

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
                    // NoOp — return unchanged scene
                    return new RenderObjectPositionWriteResult(
                        true, false, "位置未变化。", scene, change);
                }

                // Create new bounds matching the new position
                var newBounds = new SceneAxisAlignedBounds(
                    new Vector3d(newPosition.X, newPosition.Y + UnitYOffset, newPosition.Z),
                    new Vector3d(UnitHalfExtent, UnitHalfExtent, UnitHalfExtent));

                newObjects.Add(new RenderObjectInfo(
                    obj.EntityId, obj.DisplayName, newPosition,
                    obj.VisualKind, obj.SourcePath, newBounds));
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
