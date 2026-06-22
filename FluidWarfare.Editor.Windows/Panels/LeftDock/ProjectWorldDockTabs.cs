using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FluidWarfare.Editor.Windows.Panels.ProjectContentTree;
using FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.LeftDock;

sealed class ProjectWorldDockTabs
{
    readonly Button? _projectTabButton, _worldTabButton;
    readonly Grid? _contentArea;
    readonly TextBox? _searchBox;
    readonly WorldHierarchyTreePanel _worldTree;
    readonly ProjectContentTreePanel _projectTree;
    bool _isProjectTabActive;
    string _worldSearchText = string.Empty, _projectSearchText = string.Empty;
    bool _isUpdatingSearchText;

    public event Action<bool, string>? SearchChanged;
    public bool IsProjectTabActive => _isProjectTabActive;
    public string WorldSearchText { get => _worldSearchText; set => _worldSearchText = value; }

    public ProjectWorldDockTabs(Button? projectTab, Button? worldTab, Grid? contentArea,
        TextBox? searchBox, WorldHierarchyTreePanel worldTree, ProjectContentTreePanel projectTree)
    { _projectTabButton = projectTab; _worldTabButton = worldTab; _contentArea = contentArea; _searchBox = searchBox; _worldTree = worldTree; _projectTree = projectTree; }

    static readonly SolidColorBrush ActiveBg = new(Color.FromRgb(60, 80, 120));
    static readonly SolidColorBrush InactiveBg = new(Color.FromRgb(40, 50, 70));

    public void ShowWorldTab() => SwitchToTab(false);
    public void ShowProjectTab() => SwitchToTab(true);
    public void OnProjectTabClicked(object? sender, RoutedEventArgs e) => SwitchToTab(true);
    public void OnWorldTabClicked(object? sender, RoutedEventArgs e) => SwitchToTab(false);

    public void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdatingSearchText) return;
        var query = _searchBox?.Text ?? string.Empty;
        if (_isProjectTabActive) _projectSearchText = query;
        else _worldSearchText = query;
        SearchChanged?.Invoke(_isProjectTabActive, query);
    }

    public void SetSearchTextWithoutTrigger(string text)
    {
        if (_searchBox is null) return;
        _isUpdatingSearchText = true;
        try { _searchBox.Text = text; }
        finally { _isUpdatingSearchText = false; }
    }

    public void SetTabCount(bool isWorld, int count)
    {
        if (isWorld) { if (_worldTabButton is not null) _worldTabButton.Content = $"世界层级 ({count})"; }
        else { if (_projectTabButton is not null) _projectTabButton.Content = $"项目内容 ({count})"; }
    }

    void SwitchToTab(bool isProject)
    {
        _isProjectTabActive = isProject;
        if (_contentArea is null) return;
        if (isProject) _worldSearchText = _searchBox?.Text ?? string.Empty;
        else _projectSearchText = _searchBox?.Text ?? string.Empty;
        _contentArea.Children.Clear();
        var sb = _searchBox;
        if (sb is not null) { sb.PlaceholderText = isProject ? "搜索项目文件……" : "搜索世界实体……"; SetSearchTextWithoutTrigger(isProject ? _projectSearchText : _worldSearchText); }
        if (isProject) { if (_projectTabButton is not null) _projectTabButton.Background = ActiveBg; if (_worldTabButton is not null) _worldTabButton.Background = InactiveBg; _contentArea.Children.Add(_projectTree); }
        else { if (_worldTabButton is not null) _worldTabButton.Background = ActiveBg; if (_projectTabButton is not null) _projectTabButton.Background = InactiveBg; _contentArea.Children.Add(_worldTree); }
    }
}
