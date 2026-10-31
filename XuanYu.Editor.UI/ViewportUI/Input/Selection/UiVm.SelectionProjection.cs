namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void SynchronizeSelectionProjection()
    {
        TraceSelection("选择投影同步", _selectionCoordinator.ProjectionSyncDepth,
            $"选择={_editorState.Snapshot.SelectionKey}");
        var key = _editorState.Snapshot.HasSelection ? _editorState.Snapshot.SelectionKey : "";
        var project = ProjectItems.FirstOrDefault(item => item.Key == key);
        var hierarchy = BuildHierarchyItems().FirstOrDefault(item => item.Key == key);
        _selectionCoordinator.SynchronizeProjection(key, project, hierarchy, SetSelectedNodeKey,
            () => OnPropertyChanged(nameof(SelectedProjectItem)),
            () => OnPropertyChanged(nameof(SelectedHierarchyItem)), NotifySelectionMetadata);
        TraceSelection("选择投影同步完成", _selectionCoordinator.ProjectionSyncDepth,
            $"选择={_editorState.Snapshot.SelectionKey}");
    }

    void NotifySelectionMetadata()
    {
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(SelectionPath));
        OnPropertyChanged(nameof(SelectionKey));
    }

    void LogSelectionCommit(SelectEditorItemCommand command, EditorStateChangedResult changed)
    {
        var entityKeyText = TryEntityKey(command.Key, out var ek) ? EditorDisplayText.Entity(ek) : command.Key;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            $"选择已提交；结果={command.Title}",
            $"来源={command.Source}；实体={entityKeyText}；修订={changed.OldRevision}->{changed.NewRevision}");

    }

    void RaiseSelectionChanged()
    {
        _selectionCoordinator.PublishSelectionChange(
            SynchronizeSelectionProjection, RefreshInspectorEntityProjection,
            NotifySelectionBindings, RaiseInspectorSelectionBindings,
            NotifySelectionCommands, PublishSceneRenderSnapshot);
    }

    void NotifySelectionBindings()
    {
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
    }

    void NotifySelectionCommands()
    {
        OnPropertyChanged(nameof(HasTransformableSelection));
        OnPropertyChanged(nameof(CanTransformSelectedEntity));
        (SelectToolCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
