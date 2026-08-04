namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-A1 入口补接：地图撤销/重做（独立历史实例，不触碰场景实体历史）。
// 全局 Ctrl+Z 的"焦点属于地图还是场景"上下文规则尚未设计，D3 用显式按钮最安全。
// 成功后同步属性文本；渲染快照由会话 ContentChanged 自动更新。
public sealed partial class UiVm
{
    public bool CanUndo => MapSession.CanUndo;

    public bool CanRedo => MapSession.CanRedo;

    public void MapUndo()
    {
        var before = FormatMeters(MapSession.CurrentMap.SizeMeters.Width) + "×" +
                     FormatMeters(MapSession.CurrentMap.SizeMeters.Depth);
        var result = MapSession.Undo();
        if (!result.IsSuccess)
        {
            LogMapHistoryResult("撤销", false, before, before, MapSession.CurrentStateId,
                MapSession.ChangeSequence, MapSession.CanUndo, MapSession.CanRedo, result.Error?.Code);
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        var after = FormatMeters(MapSession.CurrentMap.SizeMeters.Width) + "×" +
                    FormatMeters(MapSession.CurrentMap.SizeMeters.Depth);
        LogMapHistoryResult("撤销", true, before, after, MapSession.CurrentStateId,
            MapSession.ChangeSequence, MapSession.CanUndo, MapSession.CanRedo, null);
        SyncPropertyTexts();
        FooterMessage = "已撤销地图修改。";
    }

    public void MapRedo()
    {
        var before = FormatMeters(MapSession.CurrentMap.SizeMeters.Width) + "×" +
                     FormatMeters(MapSession.CurrentMap.SizeMeters.Depth);
        var result = MapSession.Redo();
        if (!result.IsSuccess)
        {
            LogMapHistoryResult("重做", false, before, before, MapSession.CurrentStateId,
                MapSession.ChangeSequence, MapSession.CanUndo, MapSession.CanRedo, result.Error?.Code);
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        var after = FormatMeters(MapSession.CurrentMap.SizeMeters.Width) + "×" +
                    FormatMeters(MapSession.CurrentMap.SizeMeters.Depth);
        LogMapHistoryResult("重做", true, before, after, MapSession.CurrentStateId,
            MapSession.ChangeSequence, MapSession.CanUndo, MapSession.CanRedo, null);
        SyncPropertyTexts();
        FooterMessage = "已重做地图修改。";
    }

    // 从会话当前地图刷新属性输入框（新建/撤销/重做后调用）。
    public void SyncPropertyTexts()
    {
        var map = MapSession.CurrentMap;
        MapWidthText = FormatMeters(map.SizeMeters.Width);
        MapDepthText = FormatMeters(map.SizeMeters.Depth);
        MapBaseHeightText = FormatMeters(map.Surface.BaseHeightMeters);
    }
}
