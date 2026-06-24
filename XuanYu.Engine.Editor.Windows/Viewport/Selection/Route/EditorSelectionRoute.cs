using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;

/// <summary>
/// 选择路由。决定“选中谁”，不决定“怎么展示”。
/// 持有选择状态，外部通过 State 属性读取。
/// </summary>
public sealed class EditorSelectionRoute
{
    readonly EditorSelectionState _state = new();
    public EditorSelectionState State => _state;

    public EditorSelectionRouteResult SelectEntity(EditorSelectionRequest req)
    {
        var entity = FindEntity(req.EntityIdStr, req.World);
        _state.Select(entity);
        return new EditorSelectionRouteResult(entity, req.Reason, true);
    }

    public EditorSelectionRouteResult FocusViewport(EditorSelectionRequest req)
    {
        if (_state.SelectedWorldEntity is not null)
            return new EditorSelectionRouteResult(_state.SelectedWorldEntity, req.Reason, false);

        var entities = req.World?.ListEntities() ?? [];
        if (entities.Count > 0)
        {
            _state.Select(entities[0]);
            return new EditorSelectionRouteResult(entities[0], req.Reason, true);
        }

        _state.Select(null);
        return new EditorSelectionRouteResult(null, req.Reason, true);
    }

    public EditorSelectionRouteResult ClearSelection(EditorSelectionReason reason)
    {
        _state.Select(null);
        return new EditorSelectionRouteResult(null, reason, true);
    }

    static WorldEntityInfo? FindEntity(string? entityIdStr, WorldState? world)
    {
        if (entityIdStr is null || world is null) return null;
        if (!int.TryParse(entityIdStr, out var id) || id <= 0) return null;
        return world.ListEntities().FirstOrDefault(e => e.EntityId == EntityId.FromInt(id));
    }
}
