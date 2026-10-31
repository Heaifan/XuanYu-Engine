using XuanYu.Core.History;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

internal sealed class HistoryCoordinator(
    EditorHistoryOwner owner,
    Func<object, bool, bool> apply,
    Action<object, bool, string> fence,
    Action<string> setFooter)
{
    public void Route(bool undo, bool mapMode, bool regionDraft, bool roadDraft,
        bool interactionActive, Action region, Action road, Action map,
        Action<bool> completed)
    {
        if (mapMode)
        {
            if (regionDraft) { region(); return; }
            if (roadDraft) { road(); return; }
            map();
            return;
        }
        if (interactionActive)
        {
            setFooter(undo ? "拖动中不执行撤销；请先取消或提交当前会话。" :
                "拖动中不执行重做；请先取消或提交当前会话。");
            return;
        }
        var found = undo ? owner.TryPeekUndoAny(out var top) : owner.TryPeekRedoAny(out top);
        if (!found)
        {
            setFooter(undo ? "没有可撤销的编辑历史。" : "没有可重做的编辑历史。");
            return;
        }
        if (ApplyAndComplete(top!, undo)) completed(undo);
    }

    bool ApplyAndComplete(HistorySnapshot top, bool undo)
    {
        try
        {
            if (apply(top.Entry, undo) &&
                (undo ? owner.TryCompleteUndo(top) : owner.TryCompleteRedo(top))) return true;
            fence(top.Entry, undo, "HISTORY_COMPLETION_FAILED");
            setFooter("撤销/重做未能确认；历史栈保持待核验状态。");
        }
        catch (Exception error)
        {
            fence(top.Entry, undo, "HISTORY_APPLY_UNCONFIRMED:" + error.GetType().Name);
            setFooter("撤销/重做异常；历史栈未移动，请检查场景状态。");
        }
        return false;
    }
}
