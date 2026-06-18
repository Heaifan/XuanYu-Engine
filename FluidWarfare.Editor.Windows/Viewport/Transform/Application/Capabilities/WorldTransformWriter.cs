using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

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

    /// <summary>写入实体位置。返回 true 表示写入成功（位置可能未变化）。</summary>
    public bool TrySetPosition(EntityId entityId, Vector3d position)
    {
        if (!_world.SetPosition(entityId, position)) return false;
        _dirty.MarkDirty(entityId.Value.ToString());
        _statusBar?.SetDirtyState(true);
        return true;
    }
}
