using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RegionVertexSnapState _regionDrawingSnap = new();
    bool _regionDrawingSnapSuppressed;

    public bool IsRegionDrawingSnapActive => _regionDrawingSnap.IsSnapped;
    public MapRegionId? RegionDrawingSnapTargetRegionId => _regionDrawingSnap.TargetRegionId;
    public int RegionDrawingSnapTargetVertexIndex => _regionDrawingSnap.TargetVertexIndex;
    public MapPoint? RegionDrawingSnapTargetPoint => IsRegionDrawingSnapActive ? _regionDrawingSnap.TargetPoint : null;
    public MapPoint? RegionDrawingCursor => _regionDrawing.Cursor;
    public string RegionDrawingSnapStatus => IsRegionDrawingSnapActive ? "吸附中" :
        _regionDrawingSnapSuppressed ? "Alt 已取消吸附" : "未吸附";

    MapPoint ResolveRegionDrawingPoint(MapPoint raw, double x, double y, ViewportState viewport, bool suppressed)
    {
        if (suppressed)
        {
            _regionDrawingSnapSuppressed = true;
            _regionDrawingSnap.Clear();
            return raw;
        }
        _regionDrawingSnapSuppressed = false;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var result = RegionVertexSnapResolver.Resolve(default, raw, x, y, MapSession.CurrentMap, projection,
            _regionDrawingSnap, MapSession.QueryLocalRegions,
            id => MapSession.TryGetRegion(id, out var region) ? region : null,
            RegionVertexSnapSettings.Default);
        return result.ResolvedPoint;
    }

    void ClearRegionDrawingSnap()
    {
        _regionDrawingSnapSuppressed = false;
        _regionDrawingSnap.Clear();
    }
}
