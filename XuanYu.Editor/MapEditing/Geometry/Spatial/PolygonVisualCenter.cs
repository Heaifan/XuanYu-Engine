using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class PolygonVisualCenter
{
    const double SqrtTwo = 1.4142135623730951;

    public static bool TryFind(IReadOnlyList<MapPoint> polygon, out MapPoint anchor)
    {
        anchor = default;
        if (!PolygonVisualCenterGeometry.IsValid(polygon, out var bounds)) return false;
        var size = Math.Min(bounds.Width, bounds.Height);
        if (size <= 1e-9) return false;
        var best = PolygonVisualCenterGeometry.Evaluate(
            PolygonVisualCenterGeometry.Centroid(polygon, bounds), polygon);
        var queue = new PriorityQueue<Cell, double>();
        for (var x = bounds.MinX; x < bounds.MaxX; x += size)
            for (var y = bounds.MinY; y < bounds.MaxY; y += size)
                Enqueue(queue, new(x + size * .5, y + size * .5), size * .5, polygon);
        var precision = size * 1e-4;
        for (var i = 0; queue.Count > 0 && i < 16384; i++)
        {
            var cell = queue.Dequeue();
            if (cell.MaxDistance - best.Distance <= precision) continue;
            if (cell.Distance > best.Distance) best = new(cell.Center, cell.Distance);
            var half = cell.HalfSize * .5;
            Enqueue(queue, new(cell.Center.X - half, cell.Center.Y - half), half, polygon);
            Enqueue(queue, new(cell.Center.X + half, cell.Center.Y - half), half, polygon);
            Enqueue(queue, new(cell.Center.X - half, cell.Center.Y + half), half, polygon);
            Enqueue(queue, new(cell.Center.X + half, cell.Center.Y + half), half, polygon);
        }
        anchor = best.Center;
        return best.Distance >= 0;
    }

    static void Enqueue(PriorityQueue<Cell, double> queue, MapPoint center, double half,
        IReadOnlyList<MapPoint> polygon)
    {
        var distance = PolygonVisualCenterGeometry.SignedDistance(center, polygon);
        var cell = new Cell(center, half, distance, distance + half * SqrtTwo);
        queue.Enqueue(cell, -cell.MaxDistance);
    }

    readonly record struct Cell(MapPoint Center, double HalfSize, double Distance, double MaxDistance);
}
