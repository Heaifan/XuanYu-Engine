using System.Collections.ObjectModel;
using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed partial class WorldHierarchyTreePanel : UserControl
{
    readonly ObservableCollection<WorldHierarchyNodeView> _visibleRows = [];
    readonly WorldHierarchyTreeViewState _viewState = new();
    readonly WorldHierarchyProgrammaticSelection _programmatic = new();
    readonly WorldHierarchyTreeExpansion _expansion = null!;
    readonly WorldHierarchyTreeSelection _selection = null!;
    ListBox? _list;
    WorldHierarchyTree? _currentTree;
    WorldHierarchyTreeIndex? _currentIndex;
    List<WorldHierarchyNodeView> _rootViews = [];

    public event Action<string>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();
        _list = this.FindControl<ListBox>("HierarchyList");
        if (_list is null) return;
        _list.ItemsSource = _visibleRows;
        _expansion = new(_viewState);
        _selection = new(_list, _viewState, _programmatic);
        _list.SelectionChanged += OnSelectionChanged;
        _list.AddHandler(HierarchyNodeRow.ExpansionRequestedEvent, OnExpansionRequested);
    }

    public void ShowHierarchy(WorldHierarchyTree tree)
    {
        _currentTree = tree;
        FullRebuild(tree);
    }

    public void ApplySearchFilter(WorldHierarchyNode? filteredRoot, WorldHierarchyTree originalTree)
    {
        _currentTree = originalTree;
        var displayTree = filteredRoot is null
            ? originalTree
            : new WorldHierarchyTree(filteredRoot, originalTree.NodeCount,
                originalTree.EntityNodeCount, originalTree.EntityNodes,
                originalTree.EntityAncestorNodeIds);
        FullRebuild(displayTree);
    }

    public bool RevealEntity(string entityId)
        => _selection.TryReveal(entityId, _currentIndex, _expansion, RefreshVisibleRows);

    public void ShowSelectedEntity(WorldHierarchyNodeView view)
    {
        if (view.EntityId is not null)
            RevealEntity(view.EntityId);
    }

    public void ClearEntitySelection() => _selection.Clear();

    public WorldHierarchyTreeViewState GetViewState() => _viewState;

    void OnExpansionRequested(object? sender, HierarchyExpansionRequestedEventArgs eventArgs)
    {
        if (eventArgs.Node is not WorldHierarchyNodeView node) return;
        _expansion.Toggle(node);
        RefreshVisibleRows();
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selection.Handle(e,
            eid => EntitySelectionRequested?.Invoke(eid),
            () => _selection.RestoreSelection(_currentIndex, _expansion, RefreshVisibleRows));
    }

    void FullRebuild(WorldHierarchyTree tree)
    {
        (_rootViews, _currentIndex) = WorldHierarchyTreeItems.Build(tree);
        _expansion.Restore(_currentIndex);
        RefreshVisibleRows();
        _selection.RestoreSelection(_currentIndex, _expansion, RefreshVisibleRows);
    }

    void RefreshVisibleRows()
    {
        var selected = _list?.SelectedItem;
        _visibleRows.Clear();
        foreach (var row in WorldHierarchyTreeItems.BuildVisibleRows(_rootViews))
            _visibleRows.Add(row);
        if (selected is WorldHierarchyNodeView sv && _visibleRows.Contains(sv))
            _list!.SelectedItem = sv;
    }
}
