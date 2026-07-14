namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ApplySelection(string source, EditorTreeNode node)
    {
        var command = new SelectEditorItemCommand(
            source, node.Key, node.Title, node.Type, node.Path);
        if (_editorState.Select(command) is null) return;
        RaiseSelectionChanged();
        FooterMessage = $"{source}已选择：{SelectionTitle}";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(LogSummary));
    }

    void ApplyClearSelection()
    {
        if (_selectedProjectItem is not null || _selectedHierarchyItem is not null) return;
        if (_editorState.Clear(new ClearEditorSelectionCommand()) is null) return;
        RaiseSelectionChanged();
        FooterMessage = "已清空选择。";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(LogSummary));
    }

    void SetProjectSelection(EditorTreeNode? value)
    {
        if (!Set(ref _selectedProjectItem, value, nameof(SelectedProjectItem))) return;
        if (value is null) { ApplyClearSelection(); return; }
        _selectedHierarchyItem = null; OnPropertyChanged(nameof(SelectedHierarchyItem));
        ApplySelection("项目树", value);
    }

    void SetHierarchySelection(EditorTreeNode? value)
    {
        if (!Set(ref _selectedHierarchyItem, value, nameof(SelectedHierarchyItem))) return;
        if (value is null) { ApplyClearSelection(); return; }
        _selectedProjectItem = null; OnPropertyChanged(nameof(SelectedProjectItem));
        ApplySelection("层级树", value);
    }

    void RaiseSelectionChanged()
    {
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(SelectionPath));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(IsEmptySelection));
    }
}
