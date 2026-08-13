using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder(double height)
{
    readonly List<RenderVectorOverlayVertex> _vertices = [];
    readonly List<uint> _indices = [];
    readonly List<RenderVectorOverlayPrimitive> _primitives = [];

    public void AddRegion(MapRegion region, bool selected, IReadOnlyList<MapPoint>? preview)
    {
        var points = preview ?? region.Vertices;
        AddFill(points, new(.20, .55, .90, .20));
        AddStroke(points, true, selected ? new(.98, .75, .12, .98) : new(.12, .38, .70, .80), selected ? 2.4 : 1.5, 0);
        if (selected) foreach (var point in points) AddMarker(point, 6.5);
    }

    public void AddMapMarker(MapMarker marker, bool selected, MapPoint? preview)
    {
        AddMarker(preview ?? marker.Position, selected ? 8.5 : 5.5, selected ? new(.98, .75, .12, .98) : new(.98, .30, .08, 1));
    }

    public void AddDraft(MapRegionDraft draft, MapPoint? cursor, bool close)
    {
        var points = draft.Vertices.ToList();
        if (cursor is { } point) points.Add(point);
        AddStroke(points, false, new(.95, .72, .12, .95), 2.0, 0);
        for (var i = 0; i < draft.Vertices.Length; i++)
            AddMarker(draft.Vertices[i], i == 0 ? 6.5 : 5.5);
        if (close && draft.Vertices.Length > 0) AddMarker(draft.Vertices[0], 8.5);
    }

    public RenderVectorOverlayResource Build()
    {
        var bounds = Bounds();
        return new(new("map-vector-overlay"), Revision(), _vertices, _indices, _primitives, bounds);
    }

    void AddFill(IReadOnlyList<MapPoint> points, RenderStaticModelColor color)
    {
        if (points.Count < 3) return;
        var triangles = MapVectorOverlayTriangulation.Triangulate(points);
        var start = _indices.Count;
        foreach (var point in points) _vertices.Add(Vertex(point));
        foreach (var index in triangles) _indices.Add((uint)(_vertices.Count - points.Count + index));
        AddPrimitive(start, color, RenderVectorOverlayPrimitiveKind.Fill, 0, 0);
    }

    void AddStroke(IReadOnlyList<MapPoint> points, bool close, RenderStaticModelColor color,
        double width, int _)
    {
        if (points.Count < 2) return;
        var count = close ? points.Count : points.Count - 1;
        var start = _indices.Count;
        for (var n = 0; n < count; n++) AddSegment(points[n], points[(n + 1) % points.Count]);
        if (_indices.Count == start) return;
        AddPrimitive(start, color, RenderVectorOverlayPrimitiveKind.Stroke, width, 0);
    }

    void AddSegment(MapPoint a, MapPoint b)
    {
        if (a == b) return;
        var q = (uint)_vertices.Count;
        _vertices.Add(LineVertex(a, b, -1)); _vertices.Add(LineVertex(a, b, 1));
        _vertices.Add(LineVertex(b, a, -1)); _vertices.Add(LineVertex(b, a, -1));
        _vertices.Add(LineVertex(a, b, 1)); _vertices.Add(LineVertex(b, a, 1));
        _indices.AddRange([q, q + 1, q + 2, q + 2, q + 4, q + 5]);
    }

    void AddMarker(MapPoint point, double radius, RenderStaticModelColor? color = null)
    {
        var q = (uint)_vertices.Count; var center = Vertex(point).Position;
        foreach (var offset in new[] { (-1d, -1d), (1d, -1d), (1d, 1d), (-1d, -1d), (1d, 1d), (-1d, 1d) })
            _vertices.Add(new(center, center, offset.Item1, offset.Item2));
        _indices.AddRange([q, q + 1, q + 2, q + 3, q + 4, q + 5]);
        AddPrimitive(_indices.Count - 6, color ?? new(.98, .30, .08, 1),
            RenderVectorOverlayPrimitiveKind.Marker, 0, radius);
    }

    RenderVectorOverlayVertex Vertex(MapPoint p) =>
        new(MapCoordinateContract.MapToWorld(p, height), Vector3d.Zero, 0, 0);
    RenderVectorOverlayVertex LineVertex(MapPoint p, MapPoint other, double side) =>
        new(MapCoordinateContract.MapToWorld(p, height),
            MapCoordinateContract.MapToWorld(other, height), side, 0);

    void AddPrimitive(int first, RenderStaticModelColor color, RenderVectorOverlayPrimitiveKind kind,
        double width, double radius) => _primitives.Add(new(
            first, _indices.Count - first, 0, kind, color, width, radius));

}
