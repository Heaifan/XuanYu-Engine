using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

static class TerrainWorldBounds
{
    public static IReadOnlyList<Vector3d> Corners(
        IEnumerable<TerrainRenderResource> resources, TerrainRenderTransform transform)
    {
        var bounds = resources.Select(resource => Bounds(resource, transform))
            .Where(item => item is not null).Select(item => item!.Value).ToArray();
        if (bounds.Length == 0) return [];
        var minX = bounds.Min(item => item.Min.X); var maxX = bounds.Max(item => item.Max.X);
        var minY = bounds.Min(item => item.Min.Y); var maxY = bounds.Max(item => item.Max.Y);
        var minZ = bounds.Min(item => item.Min.Z); var maxZ = bounds.Max(item => item.Max.Z);
        return [new(minX, minY, minZ), new(maxX, minY, minZ), new(minX, maxY, minZ),
            new(maxX, maxY, minZ), new(minX, minY, maxZ), new(maxX, minY, maxZ),
            new(minX, maxY, maxZ), new(maxX, maxY, maxZ)];
    }

    static (Vector3d Min, Vector3d Max)? Bounds(
        TerrainRenderResource resource, TerrainRenderTransform transform)
    {
        var field = resource.Heightfield;
        var heights = field.ElevationMeters.Where((_, index) => !field.NoDataMask[index])
            .Select(transform.VisualHeight).Where(double.IsFinite).ToArray();
        if (heights.Length == 0) return null;
        var maxX = (field.Width - 1) * resource.CellSizeMeters;
        var maxY = (field.Height - 1) * resource.CellSizeMeters;
        return (new(resource.WorldOrigin.X, resource.WorldOrigin.Y, heights.Min()),
            new(resource.WorldOrigin.X + maxX, resource.WorldOrigin.Y + maxY, heights.Max()));
    }
}
