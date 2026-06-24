using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Data;

/// <summary>
/// WorldState ↔ SceneTransform 的读写入口。
/// Editor 层通过此类型访问实体位置，不直接操作 WorldState 内部结构。
/// </summary>
public static class EntitySceneTransformAccess
{
    public static SceneTransform? Read(WorldState? world, EntityId id)
    {
        var pos = world?.FindPosition(id);
        if (pos is null) return null;
        return SceneTransformDefaults.FromPosition(pos.Value.Value);
    }

    public static void WritePosition(WorldState world, EntityId id, Vector3d position)
    {
        world.SetPosition(id, position);
    }
}
