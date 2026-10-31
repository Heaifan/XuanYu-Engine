using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly RegionSnapState _regionDrawingSnap = new();
    bool _regionDrawingSnapSuppressed;

    public bool IsRegionDrawingSnapActive => _regionDrawingSnap.IsSnapped;
    public MapRegionId? RegionDrawingSnapTargetRegionId => _regionDrawingSnap.TargetRegionId;
    public int RegionDrawingSnapTargetVertexIndex => _regionDrawingSnap.TargetVertexIndex;
    public int RegionDrawingSnapTargetSegmentIndex => _regionDrawingSnap.TargetSegmentIndex;
    public MapPoint? RegionDrawingSnapTargetPoint => IsRegionDrawingSnapActive ? _regionDrawingSnap.TargetPoint : null;
    public MapPoint? RegionDrawingCursor => _regionDrawing.Cursor;
    public string RegionDrawingSnapStatus => _regionDrawingSnap.Kind switch
    {
        RegionSnapKind.Vertex => "顶点吸附",
        RegionSnapKind.Edge => "边吸附",
        _ => _regionDrawingSnapSuppressed ? "Alt 已取消吸附" : "未吸附"
    };

    MapPoint ResolveRegionDrawingPoint(MapPoint raw, double x, double y, ViewportState viewport, bool suppressed)
    {
        if (suppressed || !IsSnapEnabled)
        {
            _regionDrawingSnapSuppressed = suppressed;
            _regionDrawingSnap.Clear();
            return raw;
        }
        _regionDrawingSnapSuppressed = false;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var result = RegionSnapPipeline.Resolve(default, raw, new(x, y), MapSession.CurrentMap, projection,
            _regionDrawingSnap, MapSession.QueryLocalRegions,
            id => MapSession.TryGetRegion(id, out var region) ? region : null,
            RegionEdgeSnapSettings.Default);
        return result.ResolvedPoint;
    }

    void ClearRegionDrawingSnap()
    {
        _regionDrawingSnapSuppressed = false;
        _regionDrawingSnap.Clear();
    }
}
