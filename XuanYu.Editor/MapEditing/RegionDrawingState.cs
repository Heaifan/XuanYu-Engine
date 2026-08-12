using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RegionDrawingState
{
    readonly Stack<ImmutableArray<MapPoint>> _undo = new();
    readonly Stack<ImmutableArray<MapPoint>> _redo = new();
    public MapRegionDraft? Draft { get; private set; }
    public MapPoint? Cursor { get; private set; }
    public bool IsCloseCandidate { get; private set; }
    public bool IsActive => Draft is not null;
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Start(MapLayerId layerId, string displayName, MapRegionKind kind)
    {
        ClearHistory();
        Draft = new MapRegionDraft(layerId, displayName, kind, ImmutableArray<MapPoint>.Empty);
        Cursor = null;
        IsCloseCandidate = false;
    }

    public bool AddVertex(MapPoint point)
    {
        if (Draft is null) return false;
        _undo.Push(Draft.Vertices);
        _redo.Clear();
        Draft = Draft with { Vertices = Draft.Vertices.Add(point) };
        Cursor = point;
        IsCloseCandidate = false;
        return true;
    }

    public bool UndoVertex()
    {
        if (Draft is not { } draft || _undo.Count == 0) return false;
        _redo.Push(draft.Vertices);
        Draft = draft with { Vertices = _undo.Pop() };
        Cursor = Draft.Vertices.Length == 0 ? null : Draft.Vertices[^1];
        IsCloseCandidate = false;
        return true;
    }

    public bool RedoVertex()
    {
        if (Draft is not { } draft || _redo.Count == 0) return false;
        _undo.Push(draft.Vertices);
        Draft = draft with { Vertices = _redo.Pop() };
        Cursor = Draft.Vertices.Length == 0 ? null : Draft.Vertices[^1];
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
        ClearHistory();
        Draft = null;
        Cursor = null;
        IsCloseCandidate = false;
    }

    void ClearHistory()
    {
        _undo.Clear();
        _redo.Clear();
    }
}
