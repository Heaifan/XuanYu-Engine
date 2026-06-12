using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;

namespace FluidWarfare.Engine.World.EntityPosition;

/// <summary>
/// 对 WorldState 执行位置修改。检查 EntityId 存在性和位置是否实际变化。
/// </summary>
public static class WorldEntityPositionWriter
{
    public static WorldEntityPositionWriteResult Write(
        WorldState worldState,
        EntityId entityId,
        Vector3d newPosition)
    {
        if (!worldState.ContainsEntity(entityId))
        {
            return new WorldEntityPositionWriteResult(
                false, false,
                $"实体 {entityId} 不存在，无法修改位置。",
                null);
        }

        var oldPos = worldState.FindPosition(entityId);
        if (oldPos is null)
        {
            return new WorldEntityPositionWriteResult(
                false, false,
                $"实体 {entityId} 位置数据缺失。",
                null);
        }

        if (oldPos.Value.Value == newPosition)
        {
            // 相同位置 NoOp
            return new WorldEntityPositionWriteResult(
                true, false,
                "位置未变化。",
                new WorldEntityPositionChange(entityId, oldPos.Value.Value, newPosition));
        }

        worldState.SetPosition(entityId, newPosition);

        return new WorldEntityPositionWriteResult(
            true, true,
            string.Empty,
            new WorldEntityPositionChange(entityId, oldPos.Value.Value, newPosition));
    }
}
