using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
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
    private bool _isInternalUpdate;

    public event Action<string>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("HierarchyTree");
        if (_treeView is not null)
        {
            _treeView.SelectionChanged += OnTreeSelectionChanged;
            _treeView.ItemTemplate = new FuncTreeDataTemplate<WorldHierarchyNodeView>(
                (node, _) => BuildRow(node),
                (node) => node.Children);
        }
    }

    public void ShowHierarchy(WorldHierarchyTree tree)
    {
        _currentTree = tree;
        FullRebuild();
    }

    public bool RevealEntity(string entityId)
    {
        if (_currentIndex is null) return false;
        if (!_currentIndex.EntityViewsById.TryGetValue(entityId, out var view))
            return false;

        if (_currentIndex.AncestorViewsByEntityId.TryGetValue(entityId, out var ancestors))
            foreach (var anc in ancestors)
                anc.IsExpanded = true;

        // 静默选中
        _programmatic.Begin(entityId, 0);
        _viewState.SelectedEntityId = entityId;
        view.IsSelected = true;
        return true;
    }

    public void ClearEntitySelection()
    {
        if (_currentIndex is null) return;
        foreach (var view in _currentIndex.EntityViewsById.Values)
            view.IsSelected = false;
        _viewState.SelectedEntityId = null;
    }

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

    private Control BuildRow(WorldHierarchyNodeView node)
    {
        var row = new HierarchyNodeRow
        {
            BranchInfo = node.BranchInfo,
            IconName = node.IconKind,
            CanExpand = node.Children.Count > 0,
            IsExpanded = node.IsExpanded,
            Primary = node.DisplayName,
            Secondary = node.SecondaryText
        };

        node.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(WorldHierarchyNodeView.IsExpanded))
                row.IsExpanded = node.IsExpanded;
            if (args.PropertyName == nameof(WorldHierarchyNodeView.IsSelected))
            {
                // 选中高亮通过 TreeView SelectionChanged 处理
            }
        };

        return row;
    }

    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isInternalUpdate) return;
        if (_treeView?.SelectedItem is WorldHierarchyNodeView selectedView)
        {
            var entityId = selectedView.EntityId;
            if (entityId is not null && _programmatic.TryConsume(entityId, 0))
            {
                _viewState.SelectedEntityId = entityId;
                return;
            }

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
            var (roots, index) = WorldHierarchyTreeIndex.Build(_currentTree);
            _rootViews = roots;
            _currentIndex = index;
        }

        // 默认全部展开
        if (_rootViews is not null)
            ExpandAll(_rootViews);

        _treeView.ItemsSource = _rootViews;
    }

    private static void ExpandAll(List<WorldHierarchyNodeView> roots)
    {
        void Expand(WorldHierarchyNodeView v)
        {
            if (v.Children.Count > 0)
                v.IsExpanded = true;
            foreach (var c in v.Children)
                Expand(c);
        }
        foreach (var r in roots) Expand(r);
    }
}
