namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SynchronizeSelectionProjection()
    {
        if (_isSynchronizingSelectionProjection) return;
        _isSynchronizingSelectionProjection = true;
        _projectionSyncDepth++;
        TraceSelection("选择投影同步", _projectionSyncDepth,
            $"选择={_editorState.Snapshot.SelectionKey}");
        try
        {
            var key = _editorState.Snapshot.HasSelection ? _editorState.Snapshot.SelectionKey : "";
            var project = ProjectItems.FirstOrDefault(item => item.Key == key);
            var hierarchy = BuildHierarchyItems().FirstOrDefault(item => item.Key == key);
            var projection = hierarchy ?? project;
            SetSelectedNodeKey(key);
            var changed = projection is null
                ? null
                : _editorState.Select(new SelectEditorItemCommand(
                    "投影刷新",
                    projection.Key,
                    projection.Title,
                    projection.Type,
                    projection.Path));
            Set(ref _selectedProjectItem, project, nameof(SelectedProjectItem));
            Set(ref _selectedHierarchyItem, hierarchy, nameof(SelectedHierarchyItem));
            if (changed is not null)
            {
                OnPropertyChanged(nameof(SelectionTitle));
                OnPropertyChanged(nameof(SelectionSubtitle));
                OnPropertyChanged(nameof(SelectionPath));
                OnPropertyChanged(nameof(SelectionKey));
            }
        }
        finally
        {
            TraceSelection("选择投影同步完成", _projectionSyncDepth,
                $"选择={_editorState.Snapshot.SelectionKey}");
            _projectionSyncDepth--;
            _isSynchronizingSelectionProjection = false;
        }
    }

    void LogSelectionCommit(SelectEditorItemCommand command, EditorStateChangedResult changed)
    {
        var entityKeyText = TryEntityKey(command.Key, out var ek) ? EditorDisplayText.Entity(ek) : command.Key;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            $"选择已提交；结果={command.Title}",
            $"来源={command.Source}；实体={entityKeyText}；修订={changed.OldRevision}->{changed.NewRevision}");
        RefreshLogBindings();
    }

    void RaiseSelectionChanged()
    {
        SynchronizeSelectionProjection();
        RefreshInspectorEntityProjection();
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(SelectionPath));
        OnPropertyChanged(nameof(SelectionKey));
        SetSelectedNodeKey(_editorState.Snapshot.SelectionKey);
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(IsEmptySelection));
        OnPropertyChanged(nameof(HasInspectorSelection));
        OnPropertyChanged(nameof(IsInspectorEmpty));
        OnPropertyChanged(nameof(InspectorSelectionTitle));
        OnPropertyChanged(nameof(InspectorSelectionSubtitle));
        OnPropertyChanged(nameof(InspectorSectionTitle));
        OnPropertyChanged(nameof(InspectorFields));
        RaiseInspectorSelectionBindings();
        OnPropertyChanged(nameof(CanTransformSelectedEntity));
        (SelectToolCommand as RelayCommand)?.RaiseCanExecuteChanged();
        PublishSceneRenderSnapshot();
    }
}
