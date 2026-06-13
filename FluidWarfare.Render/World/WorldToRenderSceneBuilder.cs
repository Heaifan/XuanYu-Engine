using FluidWarfare.Core.Math;
using FluidWarfare.Engine.World;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.World;

/// <summary>
/// 把 Engine.WorldState 转换为 RenderScene (Z-Up)。
/// 为每个 UnitMarker 对象创建与渲染尺寸一致的 SelectionBounds，
/// 确保绘制和 Picking 使用同一数据源。
/// </summary>
public static class WorldToRenderSceneBuilder
{
    /// <summary>
    /// 单位立方体缩放系数（与 VulkanScene3dUnitDrawInfo.Scale 一致）。
    /// </summary>
    private const double UnitScale = 1.25;

    /// <summary>
    /// BuildCube size=1 → half=0.5，缩放后 half=0.5*UnitScale。
    /// </summary>
    private const double UnitHalfExtent = 0.5 * UnitScale;

    /// <summary>
    /// Z 偏移使单位底部与地面接触（与 EditorShell BuildUnitDrawList 一致）。
    /// </summary>
    private const double UnitZOffset = 0.5;

    /// <summary>
    /// 将 WorldState 中的所有实体转换为可渲染对象 (Z-Up)。
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

            var pos = position.Value.Value;

            // 与渲染尺寸一致的 SelectionBounds（单位缩放 1.25，Z 偏移 0.5）
            // Z-Up: 视觉中心在 Position.Z + 0.5
            var bounds = new SceneAxisAlignedBounds(
                new Vector3d(pos.X, pos.Y, pos.Z + UnitZOffset),
                new Vector3d(UnitHalfExtent, UnitHalfExtent, UnitHalfExtent));

            objects.Add(new RenderObjectInfo(
                entity.EntityId,
                entity.DisplayName,
                pos,
                RenderObjectVisualKind.UnitMarker,
                entity.Source?.RelativePath,
                bounds));
        }

        return new RenderScene(objects);
    }
}
