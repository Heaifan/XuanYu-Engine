using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public interface IDrawingDraft
{
    IReadOnlyList<MapPoint> Points { get; }
    int PointCount { get; }
    void AddPoint(MapPoint point);
    void RemoveLast();
    void Clear();
}
