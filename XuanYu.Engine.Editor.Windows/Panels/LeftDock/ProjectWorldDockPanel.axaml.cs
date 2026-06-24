using Avalonia.Controls;
using XuanYu.Engine.Editor.WorldHierarchy;
using XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;
using XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;
using ProjectContentTreeModel = XuanYu.Engine.Editor.ProjectContentTreeModel;

using XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;
using XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;
namespace XuanYu.Engine.Editor.Windows.Panels.LeftDock;

public sealed partial class ProjectWorldDockPanel : UserControl
{
    readonly WorldHierarchyTreePanel _worldTree = new();
    readonly ProjectContentTreePanel _projectTree = new();
    readonly WorldHierarchyTreeViewState _worldViewState = new();
    readonly ProjectWorldDockTabs _tabs;

    WorldHierarchyTree? _currentWorldTree;
    ProjectContentTreeModel.ProjectContentTree? _currentProjectTree;

    public event Action<string?>? EntitySelectionRequested;
    public event Action<string?>? ContentSelectionRequested;

    public ProjectWorldDockPanel()
    {
        InitializeComponent();
        var ptb = this.FindControl<Button>("ProjectTabButton");
        var wtb = this.FindControl<Button>("WorldTabButton");
        var sb = this.FindControl<TextBox>("SearchTextBox");
        _tabs = new(ptb, wtb, this.FindControl<Grid>("ContentArea"), sb, _worldTree, _projectTree);
        _worldTree.EntitySelectionRequested += id => EntitySelectionRequested?.Invoke(id);
        _projectTree.ContentSelectionRequested += path => ContentSelectionRequested?.Invoke(path);
        if (sb is not null) { sb.PlaceholderText = "搜索世界实体……"; sb.TextChanged += _tabs.OnSearchTextChanged; }
        _tabs.SearchChanged += OnSearchChanged;
        _tabs.ShowWorldTab();
        if (ptb is not null) ptb.Click += _tabs.OnProjectTabClicked;
        if (wtb is not null) wtb.Click += _tabs.OnWorldTabClicked;
    }

    public void ShowWorldHierarchy(WorldHierarchyTree tree)
    {
        _currentWorldTree = tree;
        _tabs.SetTabCount(false, tree.EntityNodeCount);
        _worldTree.ShowHierarchy(tree);
    }

    public void ShowProjectContent(ProjectContentTreeModel.ProjectContentTree tree)
    {
        _currentProjectTree = tree;
        _tabs.SetTabCount(true, tree.FileCount);
        _projectTree.ShowContentTree(tree);
    }

    public void RevealEntity(string entityId)
    {
        if (_tabs.IsProjectTabActive) _tabs.ShowWorldTab();
        if (!string.IsNullOrWhiteSpace(_tabs.WorldSearchText)) { _tabs.WorldSearchText = string.Empty; _tabs.SetSearchTextWithoutTrigger(string.Empty); }
        _worldTree.RevealEntity(entityId);
    }

    public void ClearEntitySelection() => _worldTree.ClearEntitySelection();

    public void SetWorldViewState(WorldHierarchyTreeViewState state)
    {
        _worldViewState.ExpandedNodeIds.Clear();
        foreach (var id in state.ExpandedNodeIds) _worldViewState.ExpandedNodeIds.Add(id);
        _worldViewState.SelectedEntityId = state.SelectedEntityId;
        _worldViewState.SearchText = state.SearchText;
    }

    public WorldHierarchyTreeViewState GetWorldViewState() => _worldViewState;

    void OnSearchChanged(bool isProject, string query)
    {
        if (isProject) { if (_currentProjectTree is not null) _projectTree.ApplySearchFilter(ProjectContentTreeModel.ProjectContentTreeSearch.Search(_currentProjectTree, query)); }
        else { if (_currentWorldTree is not null) _worldTree.ApplySearchFilter(WorldHierarchySearch.Search(_currentWorldTree, query), _currentWorldTree); }
    }
}
