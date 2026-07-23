namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SynchronizeSelectionProjection()
    {
        if (_isSynchronizingSelectionProjection) return;
        _isSynchronizingSelectionProjection = true;
        _projectionSyncDepth++;
        TraceSelection("SynchronizeSelectionProjection", _projectionSyncDepth,
            $"Selection={_editorState.Snapshot.SelectionKey}");
        try
        {
            var key = _editorState.Snapshot.HasSelection ? _editorState.Snapshot.SelectionKey : "";
            var project = UiText.ProjectTreeItems.FirstOrDefault(item => item.Key == key);
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
            if (Set(ref _selectedProjectItem, project, nameof(SelectedProjectItem))
                && project is not null)
            {
                LeftTabIndex = 0;
            }
            if (Set(ref _selectedHierarchyItem, hierarchy, nameof(SelectedHierarchyItem))
                && hierarchy is not null)
            {
                LeftTabIndex = 1;
            }
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
            TraceSelection("SynchronizeSelectionProjection.End", _projectionSyncDepth,
                $"Selection={_editorState.Snapshot.SelectionKey}");
            _projectionSyncDepth--;
            _isSynchronizingSelectionProjection = false;
        }
    }

    void LogSelectionCommit(SelectEditorItemCommand command, EditorStateChangedResult changed)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            $"【ARCH-C-R3】选择已提交；结果={command.Key}",
            $"来源={command.Source}; Revision={changed.OldRevision}->{changed.NewRevision}");
        RefreshLogBindings();
    }
}
