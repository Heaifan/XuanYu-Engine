using Avalonia.Controls;
using Avalonia.Media;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed partial class WorldHierarchyTreePanel : UserControl
{
    private TreeView? _treeView;
    private WorldHierarchyTree? _currentTree;
    private readonly WorldHierarchyTreeViewState _viewState = new();
    private bool _isInternalUpdate;

    // 索引：NodeId → TreeViewItem，用于 RevealEntity 不重建树
    private readonly Dictionary<string, TreeViewItem> _itemsByNodeId = [];

    // 程序化选中抑制：防止 RevealEntity → SelectionChanged → 反向触发
    private string? _pendingProgrammaticEntityId;

    /// <summary>
    /// 实体选择请求事件（EntityId string）。
    /// </summary>
    public event Action<string?>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("HierarchyTree");
        if (_treeView is not null)
            _treeView.SelectionChanged += OnTreeSelectionChanged;
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
    /// 定位并选中实体节点。只操作现有 TreeViewItem，不重建树。
    /// </summary>
    public bool RevealEntity(string entityId)
    {
        if (_currentTree is null || _treeView is null) return false;

        var node = _currentTree.FindEntity(entityId);
        if (node is null) return false;

        var ancestors = _currentTree.GetAncestorNodeIds(entityId);
        if (ancestors is not null)
        {
            foreach (var anc in ancestors)
            {
                _viewState.ExpandedNodeIds.Add(anc);
                if (_itemsByNodeId.TryGetValue(anc, out var ancItem))
                    ancItem.IsExpanded = true;
            }
        }

        // 通过索引找到目标 TreeViewItem 并选中
        if (_itemsByNodeId.TryGetValue(node.NodeId, out var targetItem))
        {
            _pendingProgrammaticEntityId = entityId;
            _viewState.SelectedEntityId = entityId;
            targetItem.IsSelected = true;
            targetItem.BringIntoView();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 清除选择。
    /// </summary>
    public void ClearEntitySelection()
    {
        _viewState.SelectedEntityId = null;
        FullRebuild();
    }

    /// <summary>
    /// 获取当前 ViewState。
    /// </summary>
    public WorldHierarchyTreeViewState GetViewState() => _viewState;

    /// <summary>
    /// 搜索过滤重建（由外部 ProjectWorldDockPanel 调用）。
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

    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isInternalUpdate) return;

        if (_treeView?.SelectedItem is TreeViewItem selectedItem &&
            selectedItem.Tag is string nodeId)
        {
            var selectedNode = FindNode(nodeId);
            if (selectedNode is null || !selectedNode.IsSelectable) return;

            var entityId = selectedNode.EntityId?.Value.ToString();

            // 程序化选中抑制：本次是 RevealEntity 触发的，消费掉不转发
            if (_pendingProgrammaticEntityId == entityId)
            {
                _pendingProgrammaticEntityId = null;
                _viewState.SelectedEntityId = entityId;
                return;
            }

            // 相同 EntityId 幂等
            if (entityId == _viewState.SelectedEntityId)
                return;

            _viewState.SelectedEntityId = entityId;
            EntitySelectionRequested?.Invoke(entityId);
        }
    }

    /// <summary>
    /// 全量重建（加载新树、搜索过滤、清除选择时调用）。
    /// </summary>
    private void FullRebuild()
    {
        if (_currentTree is null || _treeView is null) return;

        _itemsByNodeId.Clear();

        var displayTree = string.IsNullOrWhiteSpace(_viewState.SearchText)
            ? _currentTree.Root
            : WorldHierarchySearch.Search(_currentTree, _viewState.SearchText);

        _isInternalUpdate = true;
        try
        {
            _treeView.Items.Clear();
            if (displayTree is not null)
            {
                var rootItem = BuildTreeItem(displayTree);
                _treeView.Items.Add(rootItem);
                if (_viewState.ExpandedNodeIds.Contains(displayTree.NodeId)
                    || string.IsNullOrWhiteSpace(_viewState.SearchText))
                    rootItem.IsExpanded = true;
            }
        }
        finally
        {
            _isInternalUpdate = false;
        }
    }

    private TreeViewItem BuildTreeItem(WorldHierarchyNode node)
    {
        var item = new TreeViewItem();
        _itemsByNodeId[node.NodeId] = item;

        var iconText = node.IconKind switch
        {
            "world" => "\U0001F310",
            "group" => "◉",
            "entity" => "◼",
            _ => ""
        };

        var displayPanel = new StackPanel { Margin = new Avalonia.Thickness(4, 2) };
        displayPanel.Children.Add(new TextBlock
        {
            Text = string.IsNullOrEmpty(iconText) ? node.DisplayName : $"{iconText} {node.DisplayName}",
            FontSize = 13
        });

        if (!string.IsNullOrEmpty(node.SecondaryText))
        {
            displayPanel.Children.Add(new TextBlock
            {
                Text = node.SecondaryText,
                Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88)),
                FontSize = 11
            });
        }

        item.Header = displayPanel;
        item.Tag = node.NodeId;

        if (_viewState.ExpandedNodeIds.Contains(node.NodeId))
            item.IsExpanded = true;

        item.PropertyChanged += (_, args) =>
        {
            if (args.Property.Name == nameof(TreeViewItem.IsExpanded))
            {
                if (item.IsExpanded)
                    _viewState.ExpandedNodeIds.Add(node.NodeId);
                else
                    _viewState.ExpandedNodeIds.Remove(node.NodeId);
            }
        };

        if (node.IsSelectable && node.EntityId?.Value.ToString() == _viewState.SelectedEntityId)
            item.IsSelected = true;

        foreach (var child in node.Children)
            item.Items.Add(BuildTreeItem(child));

        return item;
    }

    private WorldHierarchyNode? FindNode(string nodeId)
    {
        if (_currentTree is null) return null;
        return FindNodeRecursive(_currentTree.Root, nodeId);
    }

    private static WorldHierarchyNode? FindNodeRecursive(WorldHierarchyNode root, string nodeId)
    {
        if (root.NodeId == nodeId) return root;
        foreach (var child in root.Children)
        {
            var found = FindNodeRecursive(child, nodeId);
            if (found is not null) return found;
        }
        return null;
    }
}
