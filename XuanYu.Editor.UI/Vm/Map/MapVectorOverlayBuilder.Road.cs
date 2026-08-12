using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    public void AddRoad(MapRoad road)
    {
        AddStroke(road.Points, false, new(.18, .72, .52, .90), 2.2, 0);
        foreach (var point in road.Points) AddMarker(point, 4.0);
    }

    public void AddRoadDraft(MapRoadDraft draft, MapPoint? cursor)
    {
        var points = draft.Points.ToList();
        if (cursor is { } cursorPoint) points.Add(cursorPoint);
        AddStroke(points, false, new(.96, .45, .14, .98), 2.4, 0);
        foreach (var point in draft.Points) AddMarker(point, 5.5);
    }
}
