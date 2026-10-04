using System.Collections.ObjectModel;
using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed class PointDraft
{
    readonly List<MapPoint> _points = [];
    readonly ReadOnlyCollection<MapPoint> _readOnlyPoints;

    public PointDraft() => _readOnlyPoints = _points.AsReadOnly();

    public IReadOnlyList<MapPoint> Points => _readOnlyPoints;

    public int PointCount => _points.Count;

    public void Add(MapPoint point)
    {
        if (_points.Count == 1) throw new InvalidOperationException("PointDraft accepts at most one point.");
        _points.Add(point);
    }

    public void Clear() => _points.Clear();
}
