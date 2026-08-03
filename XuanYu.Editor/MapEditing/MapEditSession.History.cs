using XuanYu.Core.Results;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：Undo/Redo 与事件广播。历史游标移动恢复对应 MapDefinition；
// ChangeSequence 每次变化（含 Undo/Redo）单调递增，不随旧状态倒退。
public sealed partial class MapEditSession
{
    public EngineResult Undo()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "撤销必须在编辑写线程执行。");
        if (!_history.TryUndoAny(out var entry) || entry is not MapHistoryEntry mapEntry)
            return Fail("NoUndoAvailable", "没有可撤销的编辑历史。");
        ApplyMapContent(mapEntry.Before, MapEditReason.Undo);
        return Ok();
    }

    public EngineResult Redo()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "重做必须在编辑写线程执行。");
        if (!_history.TryRedoAny(out var entry) || entry is not MapHistoryEntry mapEntry)
            return Fail("NoRedoAvailable", "没有可重做的编辑历史。");
        ApplyMapContent(mapEntry.After, MapEditReason.Redo);
        return Ok();
    }

    void RaiseContentChanged(MapEditReason reason) =>
        ContentChanged?.Invoke(new MapContentChangedEventArgs(_currentMap, _changeSequence, reason));

    void RaiseDirtyChanged() => DirtyChanged?.Invoke(new MapDirtyChangedEventArgs(IsDirty));

    void RaiseHistoryAvailabilityChanged() =>
        HistoryAvailabilityChanged?.Invoke(new MapHistoryAvailabilityChangedEventArgs(CanUndo, CanRedo));
}
