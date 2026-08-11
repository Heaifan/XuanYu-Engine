using XuanYu.Core.Map;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3：MapSession → 渲染快照 适配（唯一渲染输入）。
// 首次组装生成初始快照；后续只响应 ContentChanged（低频事件）。
// 相机移动/Hover/选择/面板/日志均不得重建快照；ChangeSequence 单调递增用于去重。
public sealed partial class UiVm
{
    MapRenderSnapshot _mapRenderSnapshot = MapRenderSnapshot.Empty;

    void AttachMapSession(MapEditSession session)
    {
        _mapRenderSnapshot = MapRenderSnapshotProjection.Project(session.CurrentMap, session.ChangeSequence);
        _mapWorld.Load(WorldMapState.From(session.CurrentMap));
        session.ContentChanged += OnMapContentChanged;
        session.HistoryAvailabilityChanged += OnMapHistoryAvailabilityChanged;
        session.ActiveRegionLayerChanged += OnActiveRegionLayerChanged; // D4：活动图层刷新
        session.DirtyChanged += OnMapDirtyChanged; // D5-FINAL：地图四态状态刷新（应用/图层/Undo/Redo/保存路径）
        RefreshLayerItems(); // D4：首次组装图层列表
    }

    void OnMapDirtyChanged(MapDirtyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(MapStatusText));
    }

    void OnActiveRegionLayerChanged(MapLayerId layerId)
    {
        RefreshLayerItems();
    }

    void OnMapHistoryAvailabilityChanged(MapHistoryAvailabilityChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    void OnMapContentChanged(MapContentChangedEventArgs e)
    {
        if (e.Reason == MapEditReason.NewMap)
            ResetMapManifestFromCurrentMap();
        else if (e.Reason == MapEditReason.Replace && _mapManifestOwner.CurrentPath is null)
            _mapManifestOwner.SetBaseline(MapManifest.FromMap(e.CurrentMap));
        _mapRenderSnapshot = MapRenderSnapshotProjection.Project(e.CurrentMap, e.ChangeSequence);
        _mapWorld.Load(WorldMapState.From(e.CurrentMap));
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图渲染快照已发布：原因={FormatMapEditReason(e.Reason)}；序号={e.ChangeSequence}；" +
            $"地图标识={e.CurrentMap.MapId.Value}；尺寸={e.CurrentMap.SizeMeters.Width:0.####}×" +
            $"{e.CurrentMap.SizeMeters.Depth:0.####}；基础高度={e.CurrentMap.Surface.BaseHeightMeters}；" +
            $"地表={FormatSurfaceKind(e.CurrentMap.Surface.Kind)}",
            "仅地图内容变化时发布（选择/相机/面板不触发）。");
        RefreshLogBindings();
        PublishSceneRenderSnapshot();
        RaiseMapDocumentChanged();
        RefreshLayerItems(); // D4：地图内容变化后重建图层列表
    }
}
