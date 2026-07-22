namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ApplySelection(string source, EditorTreeNode node)
    {
        CancelInteraction("切换选择对象");
        ApplySelectionCommand(new SelectEditorItemCommand(
            source, node.Key, node.Title, node.Type, node.Path));
    }

    void ApplyClearSelection()
    {
        if (_selectedProjectItem is not null || _selectedHierarchyItem is not null) return;
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "树节点清空");
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
        SynchronizeSelectionProjection();
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(SelectionPath));
        OnPropertyChanged(nameof(SelectionKey));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(IsEmptySelection));
        OnPropertyChanged(nameof(InspectorFields));
        PublishSceneRenderSnapshot();
    }

    void ApplySelectionCommand(SelectEditorItemCommand command)
    {
        var changed = _editorState.Select(command);
        if (changed is null) return;
        RaiseSelectionChanged();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            $"【ARCH-C-R3】选择已提交；结果={command.Key}",
            $"来源={command.Source}; Revision={changed.OldRevision}->{changed.NewRevision}");
        RefreshLogBindings();
        FooterMessage = $"{command.Source}已选择：{SelectionTitle}";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(LogSummary));
    }

    void ApplySelectionCommand(ClearEditorSelectionCommand command, string source)
    {
        var changed = _editorState.Clear(command);
        if (changed is null) return;
        RaiseSelectionChanged();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "【ARCH-C-R3】选择已清除；结果=None",
            $"来源={source}; Revision={changed.OldRevision}->{changed.NewRevision}");
        RefreshLogBindings();
        FooterMessage = $"{source}已清空选择。";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(LogSummary));
    }

    void SynchronizeSelectionProjection()
    {
        var key = _editorState.Snapshot.HasSelection ? _editorState.Snapshot.SelectionKey : "";
        var project = UiText.ProjectTreeItems.FirstOrDefault(item => item.Key == key);
        var hierarchy = BuildHierarchyItems().FirstOrDefault(item => item.Key == key);
        if (Set(ref _selectedProjectItem, project, nameof(SelectedProjectItem)) && project is not null)
            LeftTabIndex = 0;
        if (Set(ref _selectedHierarchyItem, hierarchy, nameof(SelectedHierarchyItem)) && hierarchy is not null)
            LeftTabIndex = 1;
    }
}
