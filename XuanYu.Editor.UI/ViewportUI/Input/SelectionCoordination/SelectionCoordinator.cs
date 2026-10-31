using XuanYu.Core.Identity;
using XuanYu.Core.Picking;
using XuanYu.Editor;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.UI;

internal sealed class SelectionCoordinator(
    EditorStateOwner selection,
    SceneStateOwner scene,
    ProjectTreePresenter trees)
{
    bool _synchronizingProjection;
    int _commitDepth, _hierarchySelectionDepth, _projectionSyncDepth;

    public bool IsSynchronizingProjection => _synchronizingProjection;
    public int CommitDepth => _commitDepth;
    public int HierarchySelectionDepth => _hierarchySelectionDepth;
    public int ProjectionSyncDepth => _projectionSyncDepth;

    public void BeginCommit() => _commitDepth++;
    public void EndCommit() => _commitDepth--;
    public void BeginHierarchySelection() => _hierarchySelectionDepth++;
    public void EndHierarchySelection() => _hierarchySelectionDepth--;

    public bool TryBeginProjectionSync()
    {
        if (_synchronizingProjection) return false;
        _synchronizingProjection = true;
        _projectionSyncDepth++;
        return true;
    }

    public void EndProjectionSync()
    {
        _projectionSyncDepth--;
        _synchronizingProjection = false;
    }

    public void PublishSelectionChange(Action syncTrees, Action refreshInspector,
        Action notifySelection, Action notifyInspector, Action notifyCommands, Action publishRender)
    {
        syncTrees();
        refreshInspector();
        notifySelection();
        notifyInspector();
        notifyCommands();
        publishRender();
    }

    public SelectionTransition? Select(SelectEditorItemCommand command, EntityId? activeEntity,
        bool blocked, Action cancelInteraction, Action<int> traceStart, Action<int> traceEnd)
    {
        if (blocked) return null;
        BeginCommit();
        traceStart(CommitDepth);
        try
        {
            cancelInteraction();
            var changed = selection.Select(command);
            var activeChanged = activeEntity is { } key && scene.SetActiveEntity(key, publish: false);
            return new(changed, activeChanged);
        }
        finally { traceEnd(CommitDepth); EndCommit(); }
    }

    public EditorStateChangedResult? Clear(ClearEditorSelectionCommand command) =>
        selection.Clear(command);

    public bool IsEntityValid(EntityId key) => scene.TryGetEntity(key, out _);

    public ViewportSelectionRequest ResolveViewportSelection(ViewportPickingResult result)
    {
        if (!result.HasHit) return new("视口空白", null);
        if (result.EntityKey is not { } key || !scene.TryGetEntity(key, out var entity))
            return new("视口失效命中", null);
        return new("视口", new EditorTreeNode(entity.EntityKey.ToString(), entity.Name,
            entity.Type, $"MainWorld/{entity.EntityKey}", 1, "entity"));
    }

    public EditorStateChangedResult? SynchronizeProjection(string key,
        EditorTreeNode? project, EditorTreeNode? hierarchy, Action<string> setNodeKey,
        Action projectChanged, Action hierarchyChanged, Action selectionChanged)
    {
        if (!TryBeginProjectionSync()) return null;
        try
        {
            setNodeKey(key);
            var projection = hierarchy ?? project;
            var changed = projection is null ? null : selection.Select(new SelectEditorItemCommand(
                "投影刷新", projection.Key, projection.Title, projection.Type, projection.Path));
            if (trees.SetSelectedItem(project)) projectChanged();
            if (trees.SetSelectedHierarchyItem(hierarchy)) hierarchyChanged();
            if (changed is not null) selectionChanged();
            return changed;
        }
        finally { EndProjectionSync(); }
    }
}

internal sealed record SelectionTransition(
    EditorStateChangedResult? Changed,
    bool ActiveEntityChanged);

internal sealed record ViewportSelectionRequest(string Source, EditorTreeNode? Node);
