using XuanYu.Engine.Core.Identity;
using FluidWarfare.Editor.Transform.Data;
using XuanYu.Engine.World;

namespace FluidWarfare.Editor.Transform.Edit;

/// <summary>
/// 从 Editor 上下文创建 TransformEditSession 的辅助方法。
/// 负责读取当前实体位置并构建初始快照。
/// </summary>
public static class TransformEditSessionStart
{
    public static bool TryBegin(
        WorldState? world,
        EntityId id,
        TransformEditKind kind,
        bool currentDirty,
        TransformEditSession session)
    {
        var transform = EntitySceneTransformAccess.Read(world, id);
        if (transform is null) return false;

        var snapshot = new TransformEditSnapshot(
            id.Value.ToString(),
            transform.Value,
            currentDirty,
            kind);

        session.Begin(snapshot);
        return true;
    }
}
