using System.Collections.ObjectModel;
using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
using ProjectContentTreeModel = XuanYu.Engine.Editor.ProjectContentTreeModel;

using XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;
using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
namespace XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;

public sealed partial class ProjectContentTreePanel : UserControl
{
    readonly ObservableCollection<ProjectContentNodeView> _visibleRows = [];
    readonly HashSet<string> _expandedNodeIds = [];
    readonly ProjectContentTreeExpansion _expansion = null!;
    readonly ProjectContentTreeSelection _selection = null!;
    ListBox? _list;
    ProjectContentTreeModel.ProjectContentTree? _currentTree;
    ProjectContentTreeIndex? _currentIndex;
    List<ProjectContentNodeView> _rootViews = [];

    public event Action<string>? ContentSelectionRequested;

    public ProjectContentTreePanel()
    {
        InitializeComponent();
        _list = this.FindControl<ListBox>("ContentList");
        if (_list is null) return;
        _list.ItemsSource = _visibleRows;
        _expansion = new(_expandedNodeIds);
        _selection = new(_list);
        _list.SelectionChanged += OnSelectionChanged;
        _list.AddHandler(HierarchyNodeRow.ExpansionRequestedEvent, OnExpansionRequested);
    }

    public void ShowContentTree(ProjectContentTreeModel.ProjectContentTree tree)
    {
        _currentTree = tree;
        FullRebuild(tree);
    }

    public void ApplySearchFilter(ProjectContentTreeModel.ProjectContentTreeNode? filteredRoot)
    {
        if (_currentTree is null) return;
        var displayTree = filteredRoot is null
            ? _currentTree
            : new ProjectContentTreeModel.ProjectContentTree(
                filteredRoot, _currentTree.NodeCount,
                _currentTree.FolderCount, _currentTree.FileCount,
                _currentTree.FileNodes);
        FullRebuild(displayTree);
    }

    void OnExpansionRequested(object? sender, HierarchyExpansionRequestedEventArgs eventArgs)
    {
        if (eventArgs.Node is not ProjectContentNodeView node) return;
        _expansion.Toggle(node);
        RefreshVisibleRows();
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selection.Handle(e,
            p => ContentSelectionRequested?.Invoke(p),
            () => _selection.RestoreSelection());
    }

    void FullRebuild(ProjectContentTreeModel.ProjectContentTree tree)
    {
        (_rootViews, _currentIndex) = ProjectContentTreeItems.Build(tree);
        _selection.SetIndex(_currentIndex);
        _expansion.Restore(_currentIndex);
        RefreshVisibleRows();
        _selection.RestoreSelection();
    }

    void RefreshVisibleRows()
    {
        var selected = _list?.SelectedItem;
        _visibleRows.Clear();
        foreach (var row in ProjectContentTreeItems.BuildVisibleRows(_rootViews))
            _visibleRows.Add(row);
        if (selected is ProjectContentNodeView sv && _visibleRows.Contains(sv))
            _list!.SelectedItem = sv;
    }
}
