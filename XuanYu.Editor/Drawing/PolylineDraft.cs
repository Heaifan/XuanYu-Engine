using System.Collections.ObjectModel;
using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed class PolylineDraft : IDrawingDraft
{
    readonly List<MapPoint> _points = [];
    readonly ReadOnlyCollection<MapPoint> _readOnlyPoints;

    public PolylineDraft() => _readOnlyPoints = _points.AsReadOnly();

    public IReadOnlyList<MapPoint> Points => _readOnlyPoints;

    public int PointCount => _points.Count;

    public void AddPoint(MapPoint point) => _points.Add(point);

    public void RemoveLast()
    {
        if (_points.Count > 0) _points.RemoveAt(_points.Count - 1);
    }

    public void Clear() => _points.Clear();
}
