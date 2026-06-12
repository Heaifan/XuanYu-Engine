using Avalonia.Controls;
using ProjectContentTreeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTree;
using ProjectContentTreeNodeType = FluidWarfare.Editor.ProjectContentTreeModel.ProjectContentTreeNode;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

public sealed partial class ProjectContentTreePanel : UserControl
{
    private TreeView? _treeView;
    private ProjectContentTreeType? _currentTree;
    private ProjectContentTreeIndex? _currentIndex;
    private List<ProjectContentNodeView>? _rootViews;

    /// <summary>文件选择请求事件（相对路径）。</summary>
    public event Action<string?>? ContentSelectionRequested;

    public ProjectContentTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("ContentTree");
        if (_treeView is not null)
            _treeView.SelectionChanged += OnSelectionChanged;
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

    private void FullRebuild(ProjectContentTreeType? tree = null)
    {
        var source = tree ?? _currentTree;
        if (source is null || _treeView is null) return;

        var (roots, index) = ProjectContentTreeIndex.Build(source);
        _rootViews = roots;
        _currentIndex = index;
        _treeView.ItemsSource = _rootViews;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_treeView?.SelectedItem is ProjectContentNodeView selectedView)
        {
            var path = selectedView.FileRelativePath;
            if (path is not null)
            {
                System.Diagnostics.Debug.WriteLine($"[ProjectTree] file selected: {path}");
                ContentSelectionRequested?.Invoke(path);
            }
        }
    }
}
