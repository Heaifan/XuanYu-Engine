using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed class TerrainRenderGroundSurface(
    IReadOnlyList<TerrainRenderResource> resources, int? revision = null) : IGroundSurface
{
    public SurfaceBinding Binding => SurfaceBinding.Terrain("terrain-render-surface");
    public int? Revision => revision;

    public SurfaceQueryResult QuerySurface(MapPoint point)
    {
        var hits = resources.Select(resource => Query(resource, point)).ToArray();
        var valid = hits.Where(hit => hit.IsValid).ToArray();
        return valid.Length > 0 ? valid.MaxBy(hit => hit.SurfaceZ) :
            hits.Any(hit => hit.Status == WorldQueryStatus.NoData)
                ? SurfaceQueryResult.NoData : SurfaceQueryResult.OutOfBounds;
    }

    public bool TryGetElevation(MapPoint point, out double elevation)
    {
        var result = QuerySurface(point);
        elevation = result.IsValid ? result.SurfaceZ : 0;
        return result.IsValid;
    }

    static SurfaceQueryResult Query(TerrainRenderResource resource, MapPoint point)
    {
        var field = resource.Heightfield;
        var x = (point.X - resource.WorldOrigin.X) / resource.CellSizeMeters;
        var y = (point.Y - resource.WorldOrigin.Y) / resource.CellSizeYMeters;
        if (x < 0 || y < 0 || x > field.Width - 1 || y > field.Height - 1)
            return SurfaceQueryResult.OutOfBounds;
        var col = Math.Min((int)x, field.Width - 2);
        var row = Math.Min((int)y, field.Height - 2);
        var fx = x - col; var fy = y - row;
        var a = row * field.Width + col; var b = a + field.Width;
        var c = a + 1; var d = b + 1;
        if (field.NoDataMask[a] || field.NoDataMask[b] || field.NoDataMask[c] || field.NoDataMask[d])
            return SurfaceQueryResult.NoData;
        var z = fx + fy <= 1
            ? field.ElevationMeters[a] * (1 - fx - fy) + field.ElevationMeters[b] * fy + field.ElevationMeters[c] * fx
            : field.ElevationMeters[c] * (1 - fy) + field.ElevationMeters[b] * (1 - fx) +
                field.ElevationMeters[d] * (fx + fy - 1);
        return SurfaceQueryResult.Valid(z);
    }
}
