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

        if (!_historyOwner.TryUndo(out var entry))
        {
            FooterMessage = "没有可撤销的编辑历史。";
            return;
        }

        if (!_sceneState.RestoreTransform(entry.EntityKey, entry.Before)) return;
        FooterMessage = "撤销完成。";
        LogHistoryUndo(entry);
        OnPropertyChanged(nameof(DebugObjectItems));
        PublishSceneRenderSnapshot();
    }

    void TryRedoFromCommand()
    {
        if (_editorState.InteractionSnapshot.Phase != EditorInteractionPhase.Idle)
        {
            FooterMessage = "拖动中不执行重做；请先取消或提交当前会话。";
            return;
        }

        if (!_historyOwner.TryRedo(out var entry))
        {
            FooterMessage = "没有可重做的编辑历史。";
            return;
        }

        if (!_sceneState.RestoreTransform(entry.EntityKey, entry.After)) return;
        FooterMessage = "重做完成。";
        LogHistoryRedo(entry);
        OnPropertyChanged(nameof(DebugObjectItems));
        PublishSceneRenderSnapshot();
    }

    void RecordTransformHistory(SceneTransformCommitResult commit)
    {
        if (!commit.Changed) return;
        var entry = new TransformHistoryEntry(commit.EntityKey, commit.Before, commit.After);
        _historyOwner.Push(entry);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "【ARCH-C-R7】记录编辑历史",
            $"Entity={entry.EntityKey}; Before={entry.Before.Position}; After={entry.After.Position}; Count={_historyOwner.Count}");
        RefreshLogBindings();
    }

    void LogHistoryUndo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "【ARCH-C-R7】执行撤销",
            $"Entity={entry.EntityKey}; Before={entry.Before.Position}; After={entry.After.Position}; Remaining={_historyOwner.Count}");
        RefreshLogBindings();
    }

    void LogHistoryRedo(TransformHistoryEntry entry)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "【WORLD-A-R0-R1】执行重做",
            $"Entity={entry.EntityKey}; Before={entry.Before.Position}; After={entry.After.Position}; Undo={_historyOwner.Count}; Redo={_historyOwner.RedoCount}");
        RefreshLogBindings();
    }
}
