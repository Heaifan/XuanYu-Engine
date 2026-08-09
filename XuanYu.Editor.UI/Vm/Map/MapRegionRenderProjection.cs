using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

static class MapRegionRenderProjection
{
    public static IReadOnlyList<RenderStaticModelResource> Build(MapDefinition map, RegionDrawingState drawing)
    {
        var result = map.Regions.Where(r => r.IsVisible).Select(r => BuildRegion(r, map.Surface.BaseHeightMeters)).ToList();
        if (drawing.Draft is { } draft)
            result.Add(BuildDraft(draft, drawing.Cursor, drawing.IsCloseCandidate, map.Surface.BaseHeightMeters));
        return result;
    }

    static RenderStaticModelResource BuildRegion(MapRegion region, double z)
    {
        var vertices = new List<RenderStaticModelVertex>(); var indices = new List<uint>();
        AddFill(region.Vertices, z, vertices, indices, new(.20, .55, .90, .20));
        AddRibbon(region.Vertices, true, z, vertices, indices, new(.12, .38, .70, .80));
        return Resource(new($"map-region-{region.RegionId.Value}"), vertices, indices);
    }

    static RenderStaticModelResource BuildDraft(MapRegionDraft draft, MapPoint? cursor, bool close, double z)
    {
        var points = draft.Vertices.ToList(); if (cursor is { } p) points.Add(p);
        var vertices = new List<RenderStaticModelVertex>(); var indices = new List<uint>();
        AddRibbon(points, false, z + .03, vertices, indices, new(.95, .72, .12, .95));
        foreach (var point in draft.Vertices) AddMarker(point, z + .04, vertices, indices);
        if (close && draft.Vertices.Length > 0) AddMarker(draft.Vertices[0], z + .06, vertices, indices);
        return Resource(new("map-region-draft"), vertices, indices);
    }

    static void AddFill(IReadOnlyList<MapPoint> points, double z, List<RenderStaticModelVertex> v, List<uint> i, RenderStaticModelColor color)
    {
        var start = v.Count; foreach (var p in points) v.Add(Vertex(p, z));
        for (var n = 1; n + 1 < points.Count; n++) { i.Add((uint)start); i.Add((uint)(start + n)); i.Add((uint)(start + n + 1)); }
        AddPrimitive(v, i, start, i.Count, color);
    }

    static void AddRibbon(IReadOnlyList<MapPoint> points, bool close, double z, List<RenderStaticModelVertex> v, List<uint> i, RenderStaticModelColor color)
    {
        if (points.Count < 2) return; var start = v.Count; const double width = 8;
        var count = close ? points.Count : points.Count - 1;
        for (var n = 0; n < count; n++) { var a = points[n]; var b = points[(n + 1) % points.Count]; var dx = b.X - a.X; var dy = b.Y - a.Y; var len = Math.Sqrt(dx * dx + dy * dy); if (len < 1e-6) continue; var ox = -dy / len * width; var oy = dx / len * width; v.Add(Vertex(new(a.X + ox, a.Y + oy), z)); v.Add(Vertex(new(a.X - ox, a.Y - oy), z)); v.Add(Vertex(new(b.X + ox, b.Y + oy), z)); v.Add(Vertex(new(b.X - ox, b.Y - oy), z)); var q = (uint)(start + n * 4); i.AddRange([q, q + 1, q + 2, q + 2, q + 1, q + 3]); }
        AddPrimitive(v, i, start, i.Count, color);
    }

    static void AddMarker(MapPoint p, double z, List<RenderStaticModelVertex> v, List<uint> i)
    { const double size = 45; AddRibbon([new(p.X - size, p.Y), new(p.X, p.Y + size), new(p.X + size, p.Y), new(p.X, p.Y - size)], true, z, v, i, new(.98, .30, .08, 1)); }
    static RenderStaticModelVertex Vertex(MapPoint p, double z) => new(new Vector3d(p.X, p.Y, z), new(0, 0, 1), 0, 0);
    static void AddPrimitive(List<RenderStaticModelVertex> v, List<uint> i, int start, int count, RenderStaticModelColor color) { _ = v; _ = start; _ = count; _ = color; }
    static RenderStaticModelResource Resource(RenderStaticModelKey key, List<RenderStaticModelVertex> v, List<uint> i) => new(key, Revision(v, i), v, i, [new(0, i.Count, 0, new(.25, .55, .85, .35))], Bounds(v));
    static int Revision(List<RenderStaticModelVertex> v, List<uint> i)
    {
        var hash = new HashCode(); foreach (var item in v) hash.Add(item.Position); foreach (var item in i) hash.Add(item); return Math.Abs(hash.ToHashCode()) | 1;
    }
    static SpatialAabb Bounds(List<RenderStaticModelVertex> v)
    {
        if (v.Count == 0) return new(Vector3d.Zero, Vector3d.Zero);
        var minX = v.Min(x => x.Position.X); var minY = v.Min(x => x.Position.Y); var minZ = v.Min(x => x.Position.Z);
        var maxX = v.Max(x => x.Position.X); var maxY = v.Max(x => x.Position.Y); var maxZ = v.Max(x => x.Position.Z);
        return new(new(minX, minY, minZ), new(maxX, maxY, maxZ));
    }
}
