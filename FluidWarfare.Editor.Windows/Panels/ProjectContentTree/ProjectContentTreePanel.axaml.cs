using System.Collections.ObjectModel;
using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using ProjectContentTreeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTree;
using ProjectContentTreeNodeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTreeNode;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

public sealed partial class ProjectContentTreePanel : UserControl
{
    private readonly ObservableCollection<ProjectContentNodeView>
        _visibleRows = [];

    private readonly HashSet<string> _expandedNodeIds = [];

    private ListBox? _list;
    private ProjectContentTreeType? _currentTree;
    private ProjectContentTreeIndex? _currentIndex;
    private List<ProjectContentNodeView> _rootViews = [];
    private string? _selectedFilePath;

    public event Action<string>? ContentSelectionRequested;

    public ProjectContentTreePanel()
    {
        InitializeComponent();

        _list = this.FindControl<ListBox>("ContentList");

        if (_list is null)
            return;

        _list.ItemsSource = _visibleRows;
        _list.SelectionChanged += OnSelectionChanged;
        _list.AddHandler(
            HierarchyNodeRow.ExpansionRequestedEvent,
            OnExpansionRequested);
    }

    public void ShowContentTree(ProjectContentTreeType tree)
    {
        _currentTree = tree;
        FullRebuild(tree);
    }

    public void ApplySearchFilter(
        ProjectContentTreeNodeType? filteredRoot)
    {
        if (_currentTree is null)
            return;

        var displayTree = filteredRoot is null
            ? _currentTree
            : new ProjectContentTreeType(
                filteredRoot,
                _currentTree.NodeCount,
                _currentTree.FolderCount,
                _currentTree.FileCount,
                _currentTree.FileNodes);

        FullRebuild(displayTree);
    }

    private void OnExpansionRequested(
        object? sender,
        HierarchyExpansionRequestedEventArgs eventArgs)
    {
        if (eventArgs.Node is not ProjectContentNodeView node)
            return;

        node.IsExpanded = !node.IsExpanded;

        if (node.IsExpanded)
            _expandedNodeIds.Add(node.NodeId);
        else
            _expandedNodeIds.Remove(node.NodeId);

        RefreshVisibleRows();
    }

    private void OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs eventArgs)
    {
        if (_list?.SelectedItem is not ProjectContentNodeView selected)
            return;

        var path = selected.FileRelativePath;

        if (path is null)
        {
            RestoreFileSelection();
            return;
        }

        if (_selectedFilePath == path)
            return;

        _selectedFilePath = path;
        ContentSelectionRequested?.Invoke(path);
    }

    private void FullRebuild(ProjectContentTreeType displayTree)
    {
        var (roots, index) =
            ProjectContentTreeIndex.Build(displayTree);

        _rootViews = roots;
        _currentIndex = index;

        RestoreExpansionState();
        RefreshVisibleRows();
        RestoreFileSelection();
    }

    private void RestoreExpansionState()
    {
        if (_currentIndex is null)
            return;

        if (_expandedNodeIds.Count == 0)
        {
            foreach (var view in _currentIndex.NodeViewsById.Values)
            {
                if (view.HasChildren)
                    _expandedNodeIds.Add(view.NodeId);
            }
        }

        foreach (var view in _currentIndex.NodeViewsById.Values)
        {
            view.IsExpanded =
                view.HasChildren &&
                _expandedNodeIds.Contains(view.NodeId);
        }
    }

    private void RestoreFileSelection()
    {
        if (_selectedFilePath is null ||
            _currentIndex?.FileViewsByPath.TryGetValue(
                _selectedFilePath,
                out var selected) != true)
        {
            _list?.UnselectAll();
            return;
        }

        _list!.SelectedItem = selected;
        _list.ScrollIntoView(selected!);
    }

    private void RefreshVisibleRows()
    {
        var selected = _list?.SelectedItem;

        _visibleRows.Clear();

        foreach (var row in HierarchyVisibleRows.Build(_rootViews))
            _visibleRows.Add(row);

        if (selected is ProjectContentNodeView selectedView &&
            _visibleRows.Contains(selectedView))
        {
            _list!.SelectedItem = selectedView;
        }
    }
}
