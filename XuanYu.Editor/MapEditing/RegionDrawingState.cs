using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionDrawingState
{
    readonly RegionDrawingController _controller;

    public RegionDrawingState() : this(new RegionDrawingController()) { }
    public RegionDrawingState(RegionDrawingController controller) => _controller = controller;
    public RegionDrawingController Controller => _controller;
    public MapRegionDraft? Draft => _controller.Draft;
    public MapPoint? Cursor => _controller.Cursor;
    public bool IsCloseCandidate => _controller.IsCloseCandidate;
    public bool IsActive => _controller.IsActive;
    public bool CanUndo => _controller.CanUndo;
    public bool CanRedo => _controller.CanRedo;

    public void Start(MapLayerId layerId, string displayName, MapRegionKind kind,
        SurfaceBinding? surfaceBinding = null)
    {
        _controller.Begin(layerId, displayName, kind, surfaceBinding ?? SurfaceBinding.ReferencePlane);
    }

    public bool AddVertex(MapPoint point)
    {
        return _controller.AcceptPoint(point).Disposition == DrawingInputDisposition.Accepted;
    }

    public bool UndoVertex()
    {
        return _controller.Undo();
    }

    public bool RedoVertex()
    {
        return _controller.Redo();
    }

    public void UpdatePointer(MapPoint point, bool closeCandidate,
        DrawingSnapCandidate? candidate = null)
    {
        _controller.UpdatePreview(point, candidate, closeCandidate);
    }

    public MapRegionDraft? TakeDraftForClose()
    {
        return _controller.CanComplete ? _controller.Draft : null;
    }

    public void Cancel()
    {
        _controller.Cancel();
    }
}
