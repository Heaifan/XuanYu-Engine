using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;

/// <summary>选择状态的唯一所有者。Shell 通过 Route 间接读取。</summary>
public sealed class EditorSelectionState
{
    public WorldEntityInfo? SelectedWorldEntity { get; private set; }
    public EntityId FirstEntityId { get; private set; }

    public void Select(WorldEntityInfo? entity)
    {
        SelectedWorldEntity = entity;
        if (entity is not null) FirstEntityId = entity.EntityId;
    }

    public void SetFirstEntityId(EntityId id) => FirstEntityId = id;
}
