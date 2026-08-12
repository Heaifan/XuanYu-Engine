using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed class RoadDrawingState
{
    readonly Stack<ImmutableArray<MapPoint>> _undo = new();
    readonly Stack<ImmutableArray<MapPoint>> _redo = new();
    public MapRoadDraft? Draft { get; private set; }
    public MapPoint? Cursor { get; private set; }
    public bool IsActive => Draft is not null;
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;
    public void Start(MapLayerId layerId, string name, string kind) { Clear(); Draft = new(layerId, name, kind, []); }
    public bool AddVertex(MapPoint point) { if (Draft is null) return false; _undo.Push(Draft.Points); _redo.Clear(); Draft = Draft with { Points = Draft.Points.Add(point) }; Cursor = point; return true; }
    public bool UndoVertex() => Move(_undo, _redo);
    public bool RedoVertex() => Move(_redo, _undo);
    bool Move(Stack<ImmutableArray<MapPoint>> from, Stack<ImmutableArray<MapPoint>> to)
    {
        if (Draft is not { } draft || from.Count == 0) return false;
        to.Push(draft.Points); Draft = draft with { Points = from.Pop() }; Cursor = Draft.Points.LastOrDefault(); return true;
    }
    public void UpdatePointer(MapPoint point) { if (Draft is not null) Cursor = point; }
    public MapRoadDraft? TakeDraft() => Draft is { CanComplete: true } draft ? draft : null;
    public void Cancel() => Clear();
    void Clear() { _undo.Clear(); _redo.Clear(); Draft = null; Cursor = null; }
}
