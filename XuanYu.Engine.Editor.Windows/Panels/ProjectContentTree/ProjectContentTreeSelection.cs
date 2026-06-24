using Avalonia.Controls;

namespace XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;

/// <summary>项目内容树选择状态和事件管理。</summary>
sealed class ProjectContentTreeSelection
{
    readonly ListBox _list;
    ProjectContentTreeIndex? _currentIndex;
    string? _selectedFilePath;

    public ProjectContentTreeSelection(ListBox list) => _list = list;

    public void SetIndex(ProjectContentTreeIndex? index) => _currentIndex = index;

    public void Handle(SelectionChangedEventArgs e, Action<string> onSelected,
        Action restoreSelection)
    {
        if (_list.SelectedItem is not ProjectContentNodeView selected)
            return;

        var path = selected.FileRelativePath;
        if (path is null)
        {
            restoreSelection();
            return;
        }

        if (_selectedFilePath == path)
            return;

        _selectedFilePath = path;
        onSelected(path);
    }

    public void RestoreSelection()
    {
        if (_selectedFilePath is null ||
            _currentIndex?.FileViewsByPath.TryGetValue(_selectedFilePath, out var selected) != true)
        {
            _list.UnselectAll();
            return;
        }

        _list.SelectedItem = selected;
        _list.ScrollIntoView(selected!);
    }
}
