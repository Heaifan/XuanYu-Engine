using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>WorldState Transform 写入结果。</summary>
public enum WorldTransformWriteStatus { Changed, NoChange, EntityNotFound }

/// <summary>WorldState Transform 写入能力。负责原子写入 + Dirty 标记。</summary>
public sealed class WorldTransformWriter
{
    readonly WorldState _world;
    readonly EditorWorldDirtyState _dirty;
    readonly StatusBarPanel? _statusBar;

    public WorldTransformWriter(WorldState world, EditorWorldDirtyState dirty, StatusBarPanel? statusBar)
    {
        _world = world; _dirty = dirty; _statusBar = statusBar;
    }

    /// <summary>写入实体位置。返回明确写入状态：Changed / NoChange / EntityNotFound。</summary>
    /// <summary>查询实体是否存在。用于 Commit 预检。</summary>
    public bool EntityExists(EntityId entityId) => _world.FindPosition(entityId) is not null;

    /// <summary>写入实体位置。返回明确写入状态：Changed / NoChange / EntityNotFound。</summary>
    public WorldTransformWriteStatus TrySetPosition(EntityId entityId, Vector3d position)
    {
        if (_world.FindPosition(entityId) is null)
            return WorldTransformWriteStatus.EntityNotFound;
        if (!_world.SetPosition(entityId, position))
            return WorldTransformWriteStatus.NoChange;
        _dirty.MarkDirty(entityId.Value.ToString());
        _statusBar?.SetDirtyState(true);
        return WorldTransformWriteStatus.Changed;
    }
}
