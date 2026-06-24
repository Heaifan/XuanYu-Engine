using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;

/// <summary>从 WorldState 读取选中实体的完整 Transform。</summary>
public static class SelectedEntityTransformReader
{
    public static TransformInspectorSnapshot? Read(EntityId entityId, WorldState world)
    {
        var position = world.FindPosition(entityId);
        if (position is null) return null;

        var rotation = world.FindRotation(entityId);
        var scale = world.FindScale(entityId);

        return new TransformInspectorSnapshot(
            entityId.Value.ToString(),
            position.Value.Value,
            rotation?.Value ?? Vector3d.Zero,
            scale?.Value ?? new Vector3d(1, 1, 1));
    }
}
