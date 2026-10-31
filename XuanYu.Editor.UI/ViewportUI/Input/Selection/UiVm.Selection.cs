namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ApplySelection(string source, EditorTreeNode node)
    {
        var command = new SelectEditorItemCommand(source, node.Key, node.Title, node.Type, node.Path);
        XuanYu.Core.Identity.EntityId? activeEntity = TryEntityKey(node.Key, out var key) ? key : null;
        var transition = _selectionCoordinator.Select(command, activeEntity, HasBlockingInput,
            () => CancelInteraction("切换选择对象"),
            depth => TraceSelection("选择提交", depth, $"来源={source}；键={node.Key}"),
            depth => TraceSelection("选择提交完成", depth, $"键={node.Key}"));
        if (transition is null || transition.Changed is null && !transition.ActiveEntityChanged) return;
        RaiseSelectionChanged();
        if (transition.Changed is not null) LogSelectionCommit(command, transition.Changed);
        FooterMessage = $"{source}已选择：{SelectionTitle}";
        FooterState = "状态：就绪";
    }

    void ApplyClearSelection()
    {
        if (SelectedProjectItem is not null || SelectedHierarchyItem is not null) return;
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "树节点清空");
    }
    void SetProjectSelection(EditorTreeNode? value)
    {
        if (HasBlockingInput && !_selectionCoordinator.IsSynchronizingProjection) return;
        var already = SelectedProjectItem == value;
        if (!_projectTreePresenter.SetSelectedItem(value))
        {
            if (!already) return;
        }
        else OnPropertyChanged(nameof(SelectedProjectItem));
        SetSelectedNodeKey(value?.Key ?? "");
        if (value is null)
        {
            if (_projectTreePresenter.SetSelectedHierarchyItem(null)) OnPropertyChanged(nameof(SelectedHierarchyItem));
            ApplyClearSelection();
            return;
        }
        // Selection projection mirrors both trees after the canonical commit.
        ApplySelection("项目树", value);
    }

    void SetHierarchySelection(EditorTreeNode? value)
    {
        if (HasBlockingInput && !_selectionCoordinator.IsSynchronizingProjection) return;
        _selectionCoordinator.BeginHierarchySelection();
        TraceSelection("层级选择", _selectionCoordinator.HierarchySelectionDepth, $"键={value?.Key ?? "空"}");
        try
        {
            var already = SelectedHierarchyItem == value;
            if (!_projectTreePresenter.SetSelectedHierarchyItem(value) && !already) return;
            if (!already) OnPropertyChanged(nameof(SelectedHierarchyItem));
            SetSelectedNodeKey(value?.Key ?? "");
            if (_selectionCoordinator.IsSynchronizingProjection) return;
            if (value is null)
            {
                if (_projectTreePresenter.SetSelectedItem(null)) OnPropertyChanged(nameof(SelectedProjectItem));
                ApplyClearSelection();
                return;
            }
            // Selection projection mirrors both trees after the canonical commit.
            ApplySelection("层级树", value);
        }
        finally
        {
            TraceSelection("层级选择完成", _selectionCoordinator.HierarchySelectionDepth, $"键={value?.Key ?? "空"}");
            _selectionCoordinator.EndHierarchySelection();
        }
    }

    void SetSelectedNodeKey(string key) => Set(ref _selectedNodeKey, key, nameof(SelectedNodeKey));

    void ApplySelectionCommand(ClearEditorSelectionCommand command, string source)
    {
        var changed = _selectionCoordinator.Clear(command);
        if (changed is null) return;
        RaiseSelectionChanged();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "选择已清除；结果=无",
            $"来源={source}；修订={changed.OldRevision}->{changed.NewRevision}");

        FooterMessage = $"{source}已清空选择。";
        FooterState = "状态：就绪";

    }
}
