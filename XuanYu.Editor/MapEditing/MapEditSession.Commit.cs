using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：统一提交管线。所有地图内容修改必须经过本方法：
// 纯修改函数 → 候选 → No-op 检测 → 领域校验 → 记录历史 → 替换 CurrentMap。
// 失败不产生任何状态变化（候选/历史/Dirty/选择/ChangeSequence 全部不变）。
public sealed partial class MapEditSession
{
    EngineResult CommitMapChange(Func<MapDefinition, MapDefinition> mutation, MapEditReason reason)
    {
        var candidate = mutation(_currentMap);
        if (candidate == _currentMap) return Ok(); // No-op：成功但无状态变化
        var validation = MapDefinitionValidator.Validate(candidate);
        if (!validation.Succeeded) return MapValidationFailure(validation);
        _history.PushEntry(new MapHistoryEntry(_currentMap, candidate, reason));
        ApplyMapContent(candidate, reason);
        return Ok();
    }

    EngineResult MapValidationFailure(MapValidationResult validation) =>
        validation.ErrorCode switch
        {
            "InvalidSize" => Fail("InvalidMapSize", validation.Message),
            "RegionVertexOutOfBounds" => Fail("RegionWouldBeOutOfBounds", "地图缩小会导致区域越界，已整体拒绝。"),
            _ => Fail("InvalidMap", validation.Message)
        };

    void ApplyMapContent(MapDefinition map, MapEditReason reason)
    {
        _currentMap = map;
        _changeSequence++;
        NormalizeSelection();
        RaiseContentChanged(reason);
        RaiseDirtyChanged();
        RaiseHistoryAvailabilityChanged();
    }
}
