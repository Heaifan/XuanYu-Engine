using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

/// <summary>世界层级树选择状态和事件管理。</summary>
sealed class WorldHierarchyTreeSelection
{
    readonly ListBox _list;
    readonly WorldHierarchyTreeViewState _state;
    readonly WorldHierarchyProgrammaticSelection _programmatic;

    public WorldHierarchyTreeSelection(ListBox list, WorldHierarchyTreeViewState state,
        WorldHierarchyProgrammaticSelection programmatic)
    {
        _list = list;
        _state = state;
        _programmatic = programmatic;
    }

    public void Handle(SelectionChangedEventArgs e, Action<string> onSelected,
        Action restoreSelection)
    {
        if (_list.SelectedItem is not WorldHierarchyNodeView selected)
            return;

        if (!selected.IsSelectable || selected.EntityId is null)
        {
            restoreSelection();
            return;
        }

        if (_programmatic.TryConsume(selected.EntityId, 0))
        {
            _state.SelectedEntityId = selected.EntityId;
            return;
        }

        if (_state.SelectedEntityId == selected.EntityId)
            return;

        _state.SelectedEntityId = selected.EntityId;
        onSelected(selected.EntityId);
    }

    public void Clear()
    {
        _programmatic.Begin(null, 0);
        _list.UnselectAll();
        _state.SelectedEntityId = null;
    }

    public void RestoreSelection(WorldHierarchyTreeIndex? index,
        WorldHierarchyTreeExpansion expansion, Action refreshRows)
    {
        var entityId = _state.SelectedEntityId;
        if (entityId is null ||
            index?.EntityViewsById.TryGetValue(entityId, out var selected) != true)
        {
            _list.UnselectAll();
            return;
        }

        expansion.ExpandAncestors(entityId, index);
        refreshRows();

        _programmatic.Begin(entityId, 0);
        _list.SelectedItem = selected;
        _list.ScrollIntoView(selected!);
    }

    public bool TryReveal(string entityId, WorldHierarchyTreeIndex? index,
        WorldHierarchyTreeExpansion expansion, Action refreshRows)
    {
        if (index?.EntityViewsById.TryGetValue(entityId, out var target) != true)
            return false;

        expansion.ExpandAncestors(entityId, index);
        refreshRows();

        _programmatic.Begin(entityId, 0);
        _list.SelectedItem = target;
        _list.ScrollIntoView(target!);
        _state.SelectedEntityId = entityId;

        return true;
    }
}
