namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isSynchronizingSelectionProjection;
    int _selectionCommitDepth, _hierarchySelectionDepth, _projectionSyncDepth;

    void ApplySelection(string source, EditorTreeNode node)
    {
        _selectionCommitDepth++;
        TraceSelection("ApplySelection", _selectionCommitDepth, $"Source={source}; Key={node.Key}");
        CancelInteraction("切换选择对象");
        try
        {
            var command = new SelectEditorItemCommand(
                source, node.Key, node.Title, node.Type, node.Path);
            var changed = _editorState.Select(command);
            var activeChanged = TryEntityKey(node.Key, out var key)
                && _sceneState.SetActiveEntity(key, publish: false);
            if (changed is null && !activeChanged) return;
            RaiseSelectionChanged();
            if (changed is not null) LogSelectionCommit(command, changed);
            FooterMessage = $"{source}已选择：{SelectionTitle}";
            FooterState = "状态：就绪";
            OnPropertyChanged(nameof(LogSummary));
        }
        finally
        {
            TraceSelection("ApplySelection.End", _selectionCommitDepth, $"Key={node.Key}");
            _selectionCommitDepth--;
        }
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
        _hierarchySelectionDepth++;
        TraceSelection("SetHierarchySelection", _hierarchySelectionDepth, $"Key={value?.Key ?? "null"}");
        try
        {
            if (!Set(ref _selectedHierarchyItem, value, nameof(SelectedHierarchyItem))) return;
            if (_isSynchronizingSelectionProjection) return;
            if (value is null) { ApplyClearSelection(); return; }
            _selectedProjectItem = null; OnPropertyChanged(nameof(SelectedProjectItem));
            ApplySelection("层级树", value);
        }
        finally
        {
            TraceSelection("SetHierarchySelection.End", _hierarchySelectionDepth, $"Key={value?.Key ?? "null"}");
            _hierarchySelectionDepth--;
        }
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
        LogSelectionCommit(command, changed);
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

}
