using XuanYu.Core.History;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void TryUndoFromShortcut() => TryUndoFromCommand();
    public void TryRedoFromShortcut() => TryRedoFromCommand();

    void TryUndoFromCommand()
    {
        if (_editorState.InteractionSnapshot.Phase != EditorInteractionPhase.Idle)
        {
            FooterMessage = "拖动中不执行撤销；请先取消或提交当前会话。";
            return;
        }

        if (!_historyOwner.TryUndoAny(out var entry))
        {
            FooterMessage = "没有可撤销的编辑历史。";
            return;
        }

        if (!ApplyHistory(entry, undo: true)) return;
        FooterMessage = "撤销完成。";
        OnPropertyChanged(nameof(DebugObjectItems));
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentChanged();
        PublishSceneRenderSnapshot();
    }

    void TryRedoFromCommand()
    {
        if (_editorState.InteractionSnapshot.Phase != EditorInteractionPhase.Idle)
        {
            FooterMessage = "拖动中不执行重做；请先取消或提交当前会话。";
            return;
        }

        if (!_historyOwner.TryRedoAny(out var entry))
        {
            FooterMessage = "没有可重做的编辑历史。";
            return;
        }

        if (!ApplyHistory(entry, undo: false)) return;
        FooterMessage = "重做完成。";
        OnPropertyChanged(nameof(DebugObjectItems));
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentChanged();
        PublishSceneRenderSnapshot();
    }

    void RecordTransformHistory(SceneTransformCommitResult commit)
    {
        if (!commit.Changed) return;
        var entry = new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After);
        _historyOwner.Push(entry);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "编辑历史已记录",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；之前位置={EditorDisplayText.Position(entry.Before.Position)}；之后位置={EditorDisplayText.Position(entry.After.Position)}；历史数量={_historyOwner.Count}");
        RefreshLogBindings();
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentChanged();
    }

    void LogHistoryUndo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "撤销已执行",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；恢复位置={EditorDisplayText.Position(entry.Before.Position)}；撤销后历史数量={_historyOwner.Count}");
        RefreshLogBindings();
    }

    void LogHistoryRedo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "重做已执行",
            $"实体={EditorDisplayText.Entity(entry.EntityKey)}；恢复位置={EditorDisplayText.Position(entry.After.Position)}；撤销数量={_historyOwner.Count}；重做数量={_historyOwner.RedoCount}");
        RefreshLogBindings();
    }
}
