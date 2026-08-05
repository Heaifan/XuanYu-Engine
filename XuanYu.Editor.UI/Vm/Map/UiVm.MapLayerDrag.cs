using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4-F3：区域图层拖动排序（UI 层入口）。
// 插入线状态为 UI 临时状态（不进历史不 Dirty）；Drop 一次性提交会话命令（单历史节点）。
public sealed partial class UiVm
{
    int? _dropRegionIndex;

    public int? DropRegionIndex => _dropRegionIndex;

    // 拖动悬停时设置插入位置（null=无有效目标）；系统层/未知位置不调用本方法。
    public void SetDropTarget(int? regionIndex)
    {
        if (_dropRegionIndex == regionIndex) return;
        _dropRegionIndex = regionIndex;
        foreach (var row in _layerItems)
        {
            var position = RegionPositionOf(row.LayerId);
            row.IsDropBefore = position >= 0 && position == regionIndex;
        }
    }

    // Drop 最终提交：一次调用一次会话命令；同位置 No-op 不记录。
    public void CommitLayerDrag(string layerIdValue, int targetIndex)
    {
        if (!MapLayerId.TryParse(layerIdValue, out var layerId)) return;
        var layer = MapLayerRules.Find(MapSession.CurrentMap.Layers, layerId);
        if (layer is null) return;
        var before = RegionPositionOf(layerId);
        var result = MapSession.MoveLayerToRegionIndex(layerId, targetIndex);
        if (!result.IsSuccess) { FailLayerEdit("调整图层顺序", result); return; }
        var after = RegionPositionOf(layerId);
        LogLayer($"调整图层顺序：{layer.DisplayName} → 第 {after + 1} 位",
            $"LayerId={layer.LayerId.Value}；顺序：{before + 1} → {after + 1}");
        FooterMessage = $"图层已移动：{layer.DisplayName} → 第 {after + 1} 位。";
    }

    public int RegionPositionOf(MapLayerId layerId)
    {
        var regions = MapLayerStack.RegionLayers(MapSession.CurrentMap.Layers);
        return MapLayerRules.IndexOfId(regions, layerId);
    }
}
