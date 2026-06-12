using Avalonia.Controls;
using Avalonia.Media;
using FluidWarfare.Editor.ProjectContentTreeModel;
using FluidWarfare.Editor.WorldHierarchy;
using FluidWarfare.Editor.Windows.Panels.ProjectContentTree;
using FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.LeftDock;

public sealed partial class ProjectWorldDockPanel : UserControl
{
    private Button? _projectTabButton;
    private Button? _worldTabButton;
    private TextBox? _searchBox;
    private Grid? _contentArea;

    private readonly WorldHierarchyTreePanel _worldTree = new();
    private readonly ProjectContentTreePanel _projectTree = new();
    private readonly WorldHierarchyTreeViewState _worldViewState = new();

    private bool _isProjectTabActive;
    private string _worldSearchText = string.Empty;
    private string _projectSearchText = string.Empty;

    private WorldHierarchyTree? _currentWorldTree;
    private ProjectContentTreeModel.ProjectContentTree? _currentProjectTree;

    // ─── Events ────────────────────────────────────────────────

    /// <summary>实体选择请求（EntityId string）。</summary>
    public event Action<string?>? EntitySelectionRequested;

    /// <summary>项目文件选择请求（相对路径）。</summary>
    public event Action<string?>? ContentSelectionRequested;

    public ProjectWorldDockPanel()
    {
        InitializeComponent();
        _projectTabButton = this.FindControl<Button>("ProjectTabButton");
        _worldTabButton = this.FindControl<Button>("WorldTabButton");
        _searchBox = this.FindControl<TextBox>("SearchTextBox");
        _contentArea = this.FindControl<Grid>("ContentArea");

        // Wire sub-panel events
        _worldTree.EntitySelectionRequested += id => EntitySelectionRequested?.Invoke(id);
        _projectTree.ContentSelectionRequested += path => ContentSelectionRequested?.Invoke(path);

        if (_searchBox is not null)
        {
            _searchBox.PlaceholderText = "搜索世界实体……";
            _searchBox.TextChanged += OnSearchTextChanged;
        }

        // 默认：世界层级
        ShowWorldTab();
    }

    // ─── Public API ────────────────────────────────────────────

    /// <summary>显示世界层级树。</summary>
    public void ShowWorldHierarchy(WorldHierarchyTree tree)
    {
        _currentWorldTree = tree;
        _worldTree.ShowHierarchy(tree);
        if (!_isProjectTabActive)
            ShowWorldTab();
    }

    /// <summary>显示项目内容树。</summary>
    public void ShowProjectContent(ProjectContentTreeModel.ProjectContentTree tree)
    {
        _currentProjectTree = tree;
        _projectTree.ShowContentTree(tree);
    }

    /// <summary>定位到世界实体节点（自动切换到世界层级页签）。</summary>
    public void RevealEntity(string entityId)
    {
        if (_isProjectTabActive)
            SwitchToWorldTab();

        // 清除搜索
        if (!string.IsNullOrWhiteSpace(_worldSearchText))
        {
            _worldSearchText = string.Empty;
            if (_searchBox is not null)
                _searchBox.Text = string.Empty;
        }

        _worldTree.RevealEntity(entityId);
    }

    /// <summary>清除世界实体选择。</summary>
    public void ClearEntitySelection()
    {
        _worldTree.ClearEntitySelection();
    }

    /// <summary>设置世界树的 ViewState（展开/选择/搜索恢复）。</summary>
    public void SetWorldViewState(WorldHierarchyTreeViewState state)
    {
        _worldViewState.ExpandedNodeIds.Clear();
        foreach (var id in state.ExpandedNodeIds)
            _worldViewState.ExpandedNodeIds.Add(id);
        _worldViewState.SelectedEntityId = state.SelectedEntityId;
        _worldViewState.SearchText = state.SearchText;
    }

    public WorldHierarchyTreeViewState GetWorldViewState() => _worldViewState;

    // ─── Tab switching ─────────────────────────────────────────

    private void ShowProjectTab() => SwitchToTab(true);
    private void ShowWorldTab() => SwitchToTab(false);

    private void OnProjectTabClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowProjectTab();
    }

    private void OnWorldTabClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowWorldTab();
    }

    private void SwitchToTab(bool isProject)
    {
        _isProjectTabActive = isProject;
        if (_contentArea is null) return;

        // 保存当前搜索词，切换后恢复
        if (isProject)
        {
            _worldSearchText = _searchBox?.Text ?? string.Empty;
            _worldViewState.SearchText = _worldSearchText;
        }
        else
        {
            _projectSearchText = _searchBox?.Text ?? string.Empty;
        }

        _contentArea.Children.Clear();

        if (isProject)
        {
            if (_searchBox is not null)
            {
                _searchBox.PlaceholderText = "搜索项目文件……";
                _searchBox.Text = _projectSearchText;
            }
            _projectTabButton!.Background = new SolidColorBrush(Color.FromRgb(60, 80, 120));
            _worldTabButton!.Background = new SolidColorBrush(Color.FromRgb(40, 50, 70));
            _contentArea.Children.Add(_projectTree);
        }
        else
        {
            if (_searchBox is not null)
            {
                _searchBox.PlaceholderText = "搜索世界实体……";
                _searchBox.Text = _worldSearchText;
            }
            _worldTabButton!.Background = new SolidColorBrush(Color.FromRgb(60, 80, 120));
            _projectTabButton!.Background = new SolidColorBrush(Color.FromRgb(40, 50, 70));
            _contentArea.Children.Add(_worldTree);
        }
    }

    private void SwitchToWorldTab() => SwitchToTab(false);

    // ─── Search ────────────────────────────────────────────────

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var query = _searchBox?.Text ?? string.Empty;

        if (_isProjectTabActive)
        {
            _projectSearchText = query;
            if (_currentProjectTree is not null)
            {
                var filtered = ProjectContentTreeModel.ProjectContentTreeSearch.Search(_currentProjectTree, query);
                _projectTree.ApplySearchFilter(filtered);
            }
        }
        else
        {
            _worldSearchText = query;
            if (_currentWorldTree is not null)
            {
                var filtered = WorldHierarchySearch.Search(_currentWorldTree, query);
                if (filtered is not null)
                {
                    _worldTree.ShowHierarchy(new WorldHierarchyTree(
                        filtered, _currentWorldTree.NodeCount,
                        _currentWorldTree.EntityNodeCount,
                        _currentWorldTree.EntityNodes,
                        _currentWorldTree.EntityAncestorNodeIds));
                }
                else
                {
                    _worldTree.ShowHierarchy(_currentWorldTree);
                }
            }
        }
    }
}
