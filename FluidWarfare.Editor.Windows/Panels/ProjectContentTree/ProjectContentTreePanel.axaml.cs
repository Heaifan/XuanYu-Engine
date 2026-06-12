using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using ProjectContentTreeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTree;
using ProjectContentTreeNodeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTreeNode;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

public sealed partial class ProjectContentTreePanel : UserControl
{
    private TreeView? _treeView;
    private ProjectContentTreeType? _currentTree;
    private ProjectContentTreeIndex? _currentIndex;
    private List<ProjectContentNodeView>? _rootViews;

    public event Action<string?>? ContentSelectionRequested;

    public ProjectContentTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("ContentTree");
        if (_treeView is not null)
        {
            _treeView.SelectionChanged += OnSelectionChanged;
            _treeView.ItemTemplate = new FuncTreeDataTemplate<ProjectContentNodeView>(
                (node, _) => BuildRow(node),
                (node) => node.Children);
        }
    }

    public void ShowContentTree(ProjectContentTreeType tree)
    {
        _currentTree = tree;
        FullRebuild();
    }

    public void ApplySearchFilter(ProjectContentTreeNodeType? filteredRoot)
    {
        if (filteredRoot is not null)
        {
            var dummyTree = new ProjectContentTreeType(
                filteredRoot, 0, 0, 0,
                _currentIndex?.FileViewsByPath
                    .ToDictionary(kv => kv.Key, kv => kv.Value.Node)
                ?? new Dictionary<string, ProjectContentTreeNodeType>());
            FullRebuild(dummyTree);
        }
        else if (_currentTree is not null)
        {
            FullRebuild();
        }
    }

    private Control BuildRow(ProjectContentNodeView node)
    {
        return new HierarchyNodeRow
        {
            BranchInfo = node.BranchInfo,
            IconName = node.IsFile ? "file" : "folder",
            CanExpand = node.Children.Count > 0,
            IsExpanded = node.IsExpanded,
            Primary = node.DisplayName,
            Secondary = node.RelativePath
        };
    }

    private void FullRebuild(ProjectContentTreeType? tree = null)
    {
        var source = tree ?? _currentTree;
        if (source is null || _treeView is null) return;

        var (roots, index) = ProjectContentTreeIndex.Build(source);
        _rootViews = roots;
        _currentIndex = index;

        // 默认全部展开
        ExpandAll(_rootViews);

        _treeView.ItemsSource = _rootViews;
    }

    private static void ExpandAll(List<ProjectContentNodeView> roots)
    {
        void Expand(ProjectContentNodeView v)
        {
            if (v.Children.Count > 0)
                v.IsExpanded = true;
            foreach (var c in v.Children)
                Expand(c);
        }
        foreach (var r in roots) Expand(r);
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_treeView?.SelectedItem is ProjectContentNodeView selectedView)
        {
            var path = selectedView.FileRelativePath;
            if (path is not null)
                ContentSelectionRequested?.Invoke(path);
        }
    }
}
