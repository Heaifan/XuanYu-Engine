using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    public void AddRoad(MapRoad road, bool selected, IReadOnlyList<MapPoint>? preview)
    {
        var points = preview ?? road.Points;
        AddStroke(points, false, selected ? new(.98, .75, .12, .98) : new(.18, .72, .52, .90), selected ? 3.0 : 2.2, 0);
        foreach (var point in points) AddMarker(point, selected ? 6.5 : 4.0);
    }

    public void AddRoadDraft(MapRoadDraft draft, MapPoint? cursor)
    {
        var points = draft.Points.ToList();
        if (cursor is { } cursorPoint) points.Add(cursorPoint);
        AddStroke(points, false, new(.96, .45, .14, .98), 2.4, 0);
        foreach (var point in draft.Points) AddMarker(point, 5.5);
    }
}
