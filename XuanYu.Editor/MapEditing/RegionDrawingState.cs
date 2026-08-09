using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionDrawingState
{
    public MapRegionDraft? Draft { get; private set; }
    public MapPoint? Cursor { get; private set; }
    public bool IsCloseCandidate { get; private set; }
    public bool IsActive => Draft is not null;

    public void Start(MapLayerId layerId, string displayName, MapRegionKind kind) =>
        Draft = new MapRegionDraft(layerId, displayName, kind, ImmutableArray<MapPoint>.Empty);

    public bool AddVertex(MapPoint point)
    {
        if (Draft is null) return false;
        Draft = Draft with { Vertices = Draft.Vertices.Add(point) };
        Cursor = point;
        IsCloseCandidate = false;
        return true;
    }

    public void UpdatePointer(MapPoint point, bool closeCandidate)
    {
        if (Draft is null) return;
        Cursor = point;
        IsCloseCandidate = closeCandidate && Draft.CanClose;
    }

    public MapRegionDraft? TakeDraftForClose()
    {
        if (Draft is not { } draft || !draft.CanClose) return null;
        return draft;
    }

    public void Cancel()
    {
        Draft = null;
        Cursor = null;
        IsCloseCandidate = false;
    }
}
