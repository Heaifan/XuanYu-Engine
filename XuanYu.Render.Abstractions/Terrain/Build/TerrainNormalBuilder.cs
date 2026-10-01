using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

static class TerrainNormalBuilder
{
    public static Vector3d Build(TerrainHeightfield field, double cellSize,
        TerrainRenderTransform transform, int row, int column)
    {
        if (!TryHeight(field, transform, row, column, out var center)) return Vector3d.UnitZ;
        var dx = Gradient(field, transform, row, column, center, cellSize, true);
        var dy = Gradient(field, transform, row, column, center, cellSize, false);
        var normal = new Vector3d(-dx, -dy, 1).Normalize();
        return IsFinite(normal) && !normal.IsZero ? normal : Vector3d.UnitZ;
    }

    static double Gradient(TerrainHeightfield field, TerrainRenderTransform transform,
        int row, int column, double center, double cellSize, bool xAxis)
    {
        var index = xAxis ? column : row;
        var limit = xAxis ? field.Width : field.Height;
        var before = 0.0;
        var after = 0.0;
        var previous = index > 0 && TryNeighbor(field, transform, row, column, index - 1, xAxis, out before);
        var next = index + 1 < limit && TryNeighbor(field, transform, row, column, index + 1, xAxis, out after);
        if (cellSize <= 0 || !double.IsFinite(cellSize)) return 0;
        if (previous && next) return (after - before) / (2 * cellSize);
        if (next) return (after - center) / cellSize;
        if (previous) return (center - before) / cellSize;
        return 0;
    }

    static bool TryNeighbor(TerrainHeightfield field, TerrainRenderTransform transform,
        int row, int column, int index, bool xAxis, out double height)
    {
        var neighborRow = xAxis ? row : index;
        var neighborColumn = xAxis ? index : column;
        return TryHeight(field, transform, neighborRow, neighborColumn, out height);
    }

    static bool TryHeight(TerrainHeightfield field, TerrainRenderTransform transform,
        int row, int column, out double height)
    {
        var index = row * field.Width + column;
        if (field.NoDataMask[index]) { height = 0; return false; }
        height = transform.VisualHeight(field.ElevationAt(row, column));
        return double.IsFinite(height);
    }

    static bool IsFinite(Vector3d value) =>
        double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Z);
}
