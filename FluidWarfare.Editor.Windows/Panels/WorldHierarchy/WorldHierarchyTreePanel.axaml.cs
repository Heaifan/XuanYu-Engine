using Avalonia.Controls;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed partial class WorldHierarchyTreePanel : UserControl
{
    private TreeView? _treeView;
    private WorldHierarchyTree? _currentTree;
    private WorldHierarchyTreeIndex? _currentIndex;
    private List<WorldHierarchyNodeView>? _rootViews;

    private readonly WorldHierarchyTreeViewState _viewState = new();
    private readonly WorldHierarchyProgrammaticSelection _programmatic = new();

    /// <summary>
    /// 用户点击实体节点时触发（EntityId string）。
    /// Root 和 Group 不会触发此事件。
    /// </summary>
    public event Action<string>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("HierarchyTree");
        if (_treeView is not null)
        {
            _treeView.SelectionChanged += OnTreeSelectionChanged;
        }
    }

    /// <summary>
    /// 加载层级树（全量重建）。
    /// </summary>
    public void ShowHierarchy(WorldHierarchyTree tree)
    {
        _currentTree = tree;
        FullRebuild();
    }

    /// <summary>
    /// 静默定位并选中实体——不发选择命令。
    /// 只展开祖先、滚动、选中节点。
    /// </summary>
    public bool RevealEntity(string entityId)
    {
        if (_currentIndex is null) return false;
        if (!_currentIndex.EntityViewsById.TryGetValue(entityId, out var view))
            return false;

        // 展开祖先
        if (_currentIndex.AncestorViewsByEntityId.TryGetValue(entityId, out var ancestors))
        {
            foreach (var anc in ancestors)
            {
                anc.IsExpanded = true;
                _viewState.ExpandedNodeIds.Add(anc.NodeId);
            }
        }

        // 静默选中
        ShowSelectedEntityView(view);
        return true;
    }

    /// <summary>
    /// 静默显示选中节点——不发选择命令。
    /// </summary>
    public void ShowSelectedEntity(WorldHierarchyNodeView view)
    {
        ShowSelectedEntityView(view);
    }

    /// <summary>
    /// 清除选择。
    /// </summary>
    public void ClearEntitySelection()
    {
        if (_currentIndex is null) return;
        foreach (var view in _currentIndex.EntityViewsById.Values)
            view.IsSelected = false;
        _viewState.SelectedEntityId = null;
    }

    /// <summary>
    /// 搜索过滤重建（由 ProjectWorldDockPanel 调用）。
    /// </summary>
    public void ApplySearchFilter(WorldHierarchyNode? filteredRoot, WorldHierarchyTree originalTree)
    {
        if (filteredRoot is not null)
        {
            _currentTree = new WorldHierarchyTree(
                filteredRoot, originalTree.NodeCount,
                originalTree.EntityNodeCount,
                originalTree.EntityNodes,
                originalTree.EntityAncestorNodeIds);
        }
        else
        {
            _currentTree = originalTree;
        }
        FullRebuild();
    }

    public WorldHierarchyTreeViewState GetViewState() => _viewState;

    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_treeView?.SelectedItem is WorldHierarchyNodeView selectedView)
        {
            // 程序化选中令牌：如果匹配，消费掉不转发
            var entityId = selectedView.EntityId;
            if (entityId is not null &&
                _programmatic.TryConsume(entityId, 0))
            {
                _viewState.SelectedEntityId = entityId;
                return;
            }

            // 仅 Entity 节点可发出选择命令
            if (selectedView.IsSelectable && entityId is not null)
            {
                _viewState.SelectedEntityId = entityId;
                EntitySelectionRequested?.Invoke(entityId);
            }
        }
    }

    private void FullRebuild()
    {
        if (_currentTree is null || _treeView is null) return;

        var displayTree = string.IsNullOrWhiteSpace(_viewState.SearchText)
            ? _currentTree.Root
            : WorldHierarchySearch.Search(_currentTree, _viewState.SearchText);

        if (displayTree is null)
        {
            // 搜索未命中，显示空状态 Root
            var emptyRoot = new WorldHierarchyNode(
                "world:root", WorldHierarchyNodeKind.WorldRoot,
                "世界", null, null, "world", false, []);
            var emptyTree = new WorldHierarchyTree(
                emptyRoot, 1, 0,
                new Dictionary<string, WorldHierarchyNode>(),
                new Dictionary<string, IReadOnlyList<string>>());
            var (roots, index) = WorldHierarchyTreeIndex.Build(emptyTree);
            _rootViews = roots;
            _currentIndex = index;
        }
        else
        {
            var filteredTree = _currentTree;
            var (roots, index) = WorldHierarchyTreeIndex.Build(filteredTree);
            _rootViews = roots;
            _currentIndex = index;
        }

        _treeView.ItemsSource = _rootViews;

        // 恢复展开状态
        if (_currentIndex is not null)
        {
            foreach (var kvp in _currentIndex.EntityViewsById)
            {
                var view = kvp.Value;
                if (_viewState.ExpandedNodeIds.Contains(view.NodeId))
                    view.IsExpanded = true;
            }

            // 恢复选中
            if (_viewState.SelectedEntityId is not null &&
                _currentIndex.EntityViewsById.TryGetValue(_viewState.SelectedEntityId, out var selectedView))
            {
                _programmatic.Begin(_viewState.SelectedEntityId, 0);
                selectedView.IsSelected = true;
            }
        }
    }

    private void ShowSelectedEntityView(WorldHierarchyNodeView view)
    {
        // 取消旧选中
        if (_viewState.SelectedEntityId is not null &&
            _currentIndex?.EntityViewsById.TryGetValue(_viewState.SelectedEntityId, out var oldView) == true)
        {
            oldView.IsSelected = false;
        }

        _viewState.SelectedEntityId = view.EntityId;
        _programmatic.Begin(view.EntityId, 0);
        view.IsSelected = true;

        // 展开祖先
        if (view.EntityId is not null &&
            _currentIndex?.AncestorViewsByEntityId.TryGetValue(view.EntityId, out var ancestors) == true)
        {
            foreach (var anc in ancestors)
            {
                anc.IsExpanded = true;
                _viewState.ExpandedNodeIds.Add(anc.NodeId);
            }
        }
    }
}
