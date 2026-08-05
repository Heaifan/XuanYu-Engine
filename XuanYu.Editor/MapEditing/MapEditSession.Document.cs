using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：文档生命周期（新建/替换/标记已保存）。
public sealed partial class MapEditSession
{
    // 新建地图：全新 MapId/LayerId，清空历史与保存状态；不是普通 Undo 命令，
    // 不允许 Undo 回到上一份完全不同的文档。
    public EngineResult CreateNewMap()
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "新建地图必须在编辑写线程执行。");
        _history.Clear();
        _currentMap = MapDefaultDefinition.CreateDefault();
        _currentPath = null;
        _savedStateId = null;
        _changeSequence++;
        SetSelection(MapSelection.Map);
        NormalizeActiveLayer();
        RaiseContentChanged(MapEditReason.NewMap);
        RaiseDirtyChanged();
        RaiseHistoryAvailabilityChanged();
        return Ok();
    }

    // 替换当前地图：只供 D6 候选加载/测试/恢复默认地图。必须完整验证；
    // 是否标记已保存由调用方明确指定。不得开放为无保护 UI 入口。
    public EngineResult ReplaceCurrentMap(MapDefinition candidate, bool markSaved, string? path)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "替换当前地图必须在编辑写线程执行。");
        if (MapDefinitionValidator.Validate(candidate) is { Succeeded: false } validation)
            return Fail("InvalidMap", validation.Message);
        _history.Clear();
        _currentMap = candidate;
        _currentPath = markSaved ? path : null;
        _savedStateId = markSaved ? CurrentStateId : null;
        _changeSequence++;
        SetSelection(MapSelection.Map);
        NormalizeActiveLayer();
        RaiseContentChanged(MapEditReason.Replace);
        RaiseDirtyChanged();
        RaiseHistoryAvailabilityChanged();
        return Ok();
    }

    public EngineResult MarkSaved(string path)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "标记已保存必须在编辑写线程执行。");
        _currentPath = path;
        _savedStateId = CurrentStateId;
        RaiseDirtyChanged();
        return Ok();
    }
}
