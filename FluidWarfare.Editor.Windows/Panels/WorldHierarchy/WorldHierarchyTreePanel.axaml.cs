using System.Collections.ObjectModel;
using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed partial class WorldHierarchyTreePanel : UserControl
{
    private readonly ObservableCollection<WorldHierarchyNodeView>
        _visibleRows = [];

    private readonly WorldHierarchyTreeViewState _viewState = new();
    private readonly WorldHierarchyProgrammaticSelection _programmatic = new();

    private ListBox? _list;
    private WorldHierarchyTree? _currentTree;
    private WorldHierarchyTreeIndex? _currentIndex;
    private List<WorldHierarchyNodeView> _rootViews = [];

    public event Action<string>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();

        _list = this.FindControl<ListBox>("HierarchyList");

        if (_list is null)
            return;

        _list.ItemsSource = _visibleRows;
        _list.SelectionChanged += OnSelectionChanged;
        _list.AddHandler(
            HierarchyNodeRow.ExpansionRequestedEvent,
            OnExpansionRequested);
    }

    public void ShowHierarchy(WorldHierarchyTree tree)
    {
        _currentTree = tree;
        FullRebuild(tree);
    }

    public void ApplySearchFilter(
        WorldHierarchyNode? filteredRoot,
        WorldHierarchyTree originalTree)
    {
        _currentTree = originalTree;

        var displayTree = filteredRoot is null
            ? originalTree
            : new WorldHierarchyTree(
                filteredRoot,
                originalTree.NodeCount,
                originalTree.EntityNodeCount,
                originalTree.EntityNodes,
                originalTree.EntityAncestorNodeIds);

        FullRebuild(displayTree);
    }

    public bool RevealEntity(string entityId)
    {
        if (_currentIndex is null ||
            !_currentIndex.EntityViewsById.TryGetValue(
                entityId,
                out var target))
        {
            return false;
        }

        ExpandAncestors(entityId);
        RefreshVisibleRows();

        _programmatic.Begin(entityId, 0);
        _list!.SelectedItem = target;
        _list.ScrollIntoView(target);
        _viewState.SelectedEntityId = entityId;

        return true;
    }

    public void ShowSelectedEntity(WorldHierarchyNodeView view)
    {
        if (view.EntityId is not null)
            RevealEntity(view.EntityId);
    }

    public void ClearEntitySelection()
    {
        _programmatic.Begin(null, 0);
        _list?.UnselectAll();
        _viewState.SelectedEntityId = null;
    }

    public WorldHierarchyTreeViewState GetViewState() => _viewState;

    private void OnExpansionRequested(
        object? sender,
        HierarchyExpansionRequestedEventArgs eventArgs)
    {
        if (eventArgs.Node is not WorldHierarchyNodeView node)
            return;

        node.IsExpanded = !node.IsExpanded;

        if (node.IsExpanded)
            _viewState.ExpandedNodeIds.Add(node.NodeId);
        else
            _viewState.ExpandedNodeIds.Remove(node.NodeId);

        RefreshVisibleRows();
    }

    private void OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs eventArgs)
    {
        if (_list?.SelectedItem is not WorldHierarchyNodeView selected)
            return;

        if (!selected.IsSelectable || selected.EntityId is null)
        {
            RestoreEntitySelection();
            return;
        }

        if (_programmatic.TryConsume(selected.EntityId, 0))
        {
            _viewState.SelectedEntityId = selected.EntityId;
            return;
        }

        if (_viewState.SelectedEntityId == selected.EntityId)
            return;

        _viewState.SelectedEntityId = selected.EntityId;
        EntitySelectionRequested?.Invoke(selected.EntityId);
    }

    private void FullRebuild(WorldHierarchyTree displayTree)
    {
        var (roots, index) =
            WorldHierarchyTreeIndex.Build(displayTree);

        _rootViews = roots;
        _currentIndex = index;

        RestoreExpansionState();
        RefreshVisibleRows();
        RestoreEntitySelection();
    }

    private void RestoreExpansionState()
    {
        if (_currentIndex is null)
            return;

        if (_viewState.ExpandedNodeIds.Count == 0)
        {
            foreach (var view in _currentIndex.NodeViewsById.Values)
            {
                if (view.HasChildren)
                    _viewState.ExpandedNodeIds.Add(view.NodeId);
            }
        }

        foreach (var view in _currentIndex.NodeViewsById.Values)
        {
            view.IsExpanded =
                view.HasChildren &&
                _viewState.ExpandedNodeIds.Contains(view.NodeId);
        }
    }

    private void ExpandAncestors(string entityId)
    {
        if (_currentIndex?.AncestorViewsByEntityId.TryGetValue(
                entityId,
                out var ancestors) != true)
        {
            return;
        }

        foreach (var ancestor in ancestors!)
        {
            ancestor.IsExpanded = true;
            _viewState.ExpandedNodeIds.Add(ancestor.NodeId);
        }
    }

    private void RestoreEntitySelection()
    {
        var entityId = _viewState.SelectedEntityId;

        if (entityId is null ||
            _currentIndex?.EntityViewsById.TryGetValue(
                entityId,
                out var selected) != true)
        {
            _list?.UnselectAll();
            return;
        }

        ExpandAncestors(entityId);
        RefreshVisibleRows();

        _programmatic.Begin(entityId, 0);
        _list!.SelectedItem = selected;
        _list.ScrollIntoView(selected!);
    }

    private void RefreshVisibleRows()
    {
        var selected = _list?.SelectedItem;

        _visibleRows.Clear();

        foreach (var row in HierarchyVisibleRows.Build(_rootViews))
            _visibleRows.Add(row);

        if (selected is WorldHierarchyNodeView selectedView &&
            _visibleRows.Contains(selectedView))
        {
            _list!.SelectedItem = selectedView;
        }
    }
}
