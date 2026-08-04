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
    }

    void OnMapHistoryAvailabilityChanged(MapHistoryAvailabilityChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    void OnMapContentChanged(MapContentChangedEventArgs e)
    {
        _mapRenderSnapshot = MapRenderSnapshotProjection.Project(e.CurrentMap, e.ChangeSequence);
        _mapWorld.Load(WorldMapState.From(e.CurrentMap));
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"地图渲染快照已发布：Reason={e.Reason}；Sequence={e.ChangeSequence}；" +
            $"MapId={e.CurrentMap.MapId.Value}；Size={e.CurrentMap.SizeMeters.Width:0.####}×" +
            $"{e.CurrentMap.SizeMeters.Depth:0.####}；BaseHeight={e.CurrentMap.Surface.BaseHeightMeters}；" +
            $"Surface={e.CurrentMap.Surface.Kind}",
            "仅地图内容变化时发布（选择/相机/面板不触发）。");
        RefreshLogBindings();
        PublishSceneRenderSnapshot();
        RaiseMapDocumentChanged();
    }
}
