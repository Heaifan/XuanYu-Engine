using XuanYu.Core.History;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly HistoryCoordinator _historyCoordinator;
    public void TryUndoFromShortcut() => TryUndoFromCommand();
    public void TryRedoFromShortcut() => TryRedoFromCommand();

    void TryUndoFromCommand() => RouteHistory(undo: true);
    void TryRedoFromCommand() => RouteHistory(undo: false);

    void RouteHistory(bool undo) => _historyCoordinator.Route(undo, IsRegionEditMode,
        IsRegionDrawingDraftActive, IsRoadDrawingDraftActive,
        _editorState.InteractionSnapshot.Phase != EditorInteractionPhase.Idle,
        () => { if (undo) UndoRegionDrawingVertex(); else RedoRegionDrawingVertex(); },
        () => { if (undo) UndoRoadDrawingVertex(); else RedoRoadDrawingVertex(); },
        () => { if (undo) MapUndo(); else MapRedo(); }, CompleteHistoryCommand);

    void CompleteHistoryCommand(bool undo)
    {
        FooterMessage = undo ? "撤销完成。" : "重做完成。";
        OnPropertyChanged(nameof(DebugObjectItems));
        OnPropertyChanged(nameof(InspectorFields)); RefreshInspectorEntityProjection();
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentChanged();
        PublishSceneRenderSnapshot();
    }

    void RecordTransformHistory(TransformCommit commit)
    {
        if (!commit.Changed) return;
        var entry = new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After);
        _historyOwner.Push(entry);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "编辑历史已记录",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；之前位置={EditorDisplayText.Position(entry.Before.Position)}；之后位置={EditorDisplayText.Position(entry.After.Position)}；历史数量={_historyOwner.Count}");

        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentChanged();
    }

    void LogHistoryUndo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "撤销已执行",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；恢复位置={EditorDisplayText.Position(entry.Before.Position)}；撤销后历史数量={_historyOwner.Count}");

    }

    void LogHistoryRedo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "重做已执行",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；恢复位置={EditorDisplayText.Position(entry.After.Position)}；撤销数量={_historyOwner.Count}；重做数量={_historyOwner.RedoCount}");

    }
}
