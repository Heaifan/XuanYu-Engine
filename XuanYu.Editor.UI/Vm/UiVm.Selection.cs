namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isSynchronizingSelectionProjection;
    int _selectionCommitDepth, _hierarchySelectionDepth, _projectionSyncDepth;

    void ApplySelection(string source, EditorTreeNode node)
    {
        _selectionCommitDepth++;
        TraceSelection("选择提交", _selectionCommitDepth, $"来源={source}；键={node.Key}");
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
            TraceSelection("选择提交完成", _selectionCommitDepth, $"键={node.Key}");
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
        SetSelectedNodeKey(value?.Key ?? "");
        if (value is null) { ApplyClearSelection(); return; }
        if (value.CanToggle) ToggleProjectNode(value);
        _selectedHierarchyItem = null; OnPropertyChanged(nameof(SelectedHierarchyItem));
        ApplySelection("项目树", value);
    }

    void SetHierarchySelection(EditorTreeNode? value)
    {
        _hierarchySelectionDepth++;
        TraceSelection("层级选择", _hierarchySelectionDepth, $"键={value?.Key ?? "空"}");
        try
        {
            if (!Set(ref _selectedHierarchyItem, value, nameof(SelectedHierarchyItem))) return;
            SetSelectedNodeKey(value?.Key ?? "");
            if (_isSynchronizingSelectionProjection) return;
            if (value is null) { ApplyClearSelection(); return; }
            if (value.CanToggle) ToggleHierarchyNode(value);
            _selectedProjectItem = null; OnPropertyChanged(nameof(SelectedProjectItem));
            ApplySelection("层级树", value);
        }
        finally
        {
            TraceSelection("层级选择完成", _hierarchySelectionDepth, $"键={value?.Key ?? "空"}");
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
        SetSelectedNodeKey(_editorState.Snapshot.SelectionKey);
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(IsEmptySelection));
        OnPropertyChanged(nameof(InspectorFields));
        PublishSceneRenderSnapshot();
    }

    void SetSelectedNodeKey(string key) => Set(ref _selectedNodeKey, key, nameof(SelectedNodeKey));

    void ApplySelectionCommand(ClearEditorSelectionCommand command, string source)
    {
        var changed = _editorState.Clear(command);
        if (changed is null) return;
        RaiseSelectionChanged();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "选择已清除；结果=无",
            $"来源={source}；修订={changed.OldRevision}->{changed.NewRevision}");
        RefreshLogBindings();
        FooterMessage = $"{source}已清空选择。";
        FooterState = "状态：就绪";
        OnPropertyChanged(nameof(LogSummary));
    }
}
