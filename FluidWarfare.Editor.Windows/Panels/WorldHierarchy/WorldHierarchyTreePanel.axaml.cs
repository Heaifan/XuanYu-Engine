using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed partial class WorldHierarchyTreePanel : UserControl
{
    private TextBox? _searchBox;
    private TreeView? _treeView;
    private WorldHierarchyTree? _currentTree;
    private WorldHierarchyTreeViewState _viewState = new();
    private bool _isInternalUpdate;

    /// <summary>
    /// 实体选择请求事件（EntityId string）。
    /// </summary>
    public event Action<string?>? EntitySelectionRequested;

    public WorldHierarchyTreePanel()
    {
        InitializeComponent();
        _searchBox = this.FindControl<TextBox>("SearchTextBox");
        _treeView = this.FindControl<TreeView>("HierarchyTree");

        if (_searchBox is not null)
            _searchBox.TextChanged += OnSearchTextChanged;

        if (_treeView is not null)
            _treeView.SelectionChanged += OnTreeSelectionChanged;
    }

    /// <summary>
    /// 加载层级树。
    /// </summary>
    public void ShowHierarchy(WorldHierarchyTree tree)
    {
        _currentTree = tree;
        ApplySearchAndRebuild();
    }

    /// <summary>
    /// 定位并选中实体节点。自动展开祖先、滚动到可见位置。
    /// </summary>
    public bool RevealEntity(string entityId)
    {
        if (_currentTree is null) return false;

        var node = _currentTree.FindEntity(entityId);
        if (node is null) return false;

        // 清除搜索（确保目标可见）
        if (!string.IsNullOrWhiteSpace(_viewState.SearchText))
        {
            _viewState.SearchText = string.Empty;
            if (_searchBox is not null)
                _searchBox.Text = string.Empty;
        }

        // 展开祖先
        var ancestors = _currentTree.GetAncestorNodeIds(entityId);
        if (ancestors is not null)
        {
            foreach (var anc in ancestors)
                _viewState.ExpandedNodeIds.Add(anc);
        }

        // 选中目标
        _viewState.SelectedEntityId = entityId;
        ApplySearchAndRebuild();
        return true;
    }

    /// <summary>
    /// 清除选择。
    /// </summary>
    public void ClearEntitySelection()
    {
        _viewState.SelectedEntityId = null;
        ApplySearchAndRebuild();
    }

    /// <summary>
    /// 获取当前 ViewState（展开节点、选择、搜索）。
    /// </summary>
    public WorldHierarchyTreeViewState GetViewState() => _viewState;

    /// <summary>
    /// 恢复 ViewState（树重建后保留状态）。
    /// </summary>
    public void SetViewState(WorldHierarchyTreeViewState state)
    {
        _viewState = state;
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isInternalUpdate) return;
        _viewState.SearchText = _searchBox?.Text ?? string.Empty;
        ApplySearchAndRebuild();
    }

    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isInternalUpdate) return;
        if (_treeView?.SelectedItem is WorldHierarchyNode selectedNode && selectedNode.IsSelectable)
        {
            var entityId = selectedNode.EntityId?.Value.ToString();
            _viewState.SelectedEntityId = entityId;
            EntitySelectionRequested?.Invoke(entityId);
        }
    }

    private void ApplySearchAndRebuild()
    {
        if (_currentTree is null) return;

        // 收集当前展开状态
        if (_treeView is not null)
        {
            foreach (var item in _treeView.Items)
            {
                if (item is TreeViewItem tvi)
                    CollectExpanded(tvi);
            }
        }

        // 搜索过滤
        var displayTree = string.IsNullOrWhiteSpace(_viewState.SearchText)
            ? _currentTree.Root
            : WorldHierarchySearch.Search(_currentTree, _viewState.SearchText);

        // 重建 TreeView
        _isInternalUpdate = true;
        try
        {
            if (_treeView is not null)
            {
                _treeView.Items.Clear();
                if (displayTree is not null)
                {
                    var rootItem = BuildTreeItem(displayTree);
                    _treeView.Items.Add(rootItem);
                    // 默认展开 Root
                    if (_viewState.ExpandedNodeIds.Contains(displayTree.NodeId) || string.IsNullOrWhiteSpace(_viewState.SearchText))
                        rootItem.IsExpanded = true;
                }
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

        // 图标
        var iconText = node.IconKind switch
        {
            "world" => "\U0001F310",  // 🌐
            "group" => "◉",      // ◉
            "entity" => "◼",     // ◼
            _ => ""
        };

        // 显示文本
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

        // 展开状态恢复
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

        // 选中高亮
        if (node.IsSelectable && node.EntityId?.Value.ToString() == _viewState.SelectedEntityId)
            item.IsSelected = true;

        // 子节点
        foreach (var child in node.Children)
            item.Items.Add(BuildTreeItem(child));

        return item;
    }

    private void CollectExpanded(TreeViewItem item)
    {
        if (item.IsExpanded && item.Tag is string nodeId)
            _viewState.ExpandedNodeIds.Add(nodeId);

        foreach (var child in item.Items)
        {
            if (child is TreeViewItem tvi)
                CollectExpanded(tvi);
        }
    }
}
