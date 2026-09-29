using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;
delegate bool GroundElevationQuery(MapPoint point, out double elevation);

sealed partial class MapVectorOverlayBuilder(double height, double dpiScale = 1.0,
    MapLabelBitmapCache? labelCache = null, GroundElevationQuery? surface = null)
{
    static readonly RenderStaticModelColor RegionStroke = new(.12, .38, .70, .92);
    readonly List<RenderVectorOverlayVertex> _vertices = [];
    readonly List<uint> _indices = [];
    readonly List<RenderVectorOverlayPrimitive> _primitives = [];
    readonly List<RenderVectorOverlayLabel> _labels = [];
    readonly List<RenderLabelBitmap> _labelBitmaps = [];

    public void AddRegion(MapRegion region, bool selected, IReadOnlyList<MapPoint>? preview)
    {
        var points = preview ?? region.Vertices;
        if (!HasSurface(points, region.SurfaceBinding)) return;
        AddFill(points, MapRegionRenderStyle.FillColor(region), region.SurfaceBinding);
        AddLabel(region, points);
        AddStroke(points, true, selected ? new(.98, .75, .12, .98) : RegionStroke,
            selected ? 2.4 : 1.5, 0, region.SurfaceBinding);
        if (selected) foreach (var point in points) AddMarker(point, 6.5, binding: region.SurfaceBinding);
    }

    public void AddMapMarker(MapMarker marker, bool selected, MapPoint? preview)
    {
        AddMarker(preview ?? marker.Position, selected ? 8.5 : 5.5,
            selected ? new(.98, .75, .12, .98) : new(.98, .30, .08, 1));
    }

    public void AddDraft(MapRegionDraft draft, MapPoint? cursor, bool close)
    {
        var points = draft.Vertices.ToList();
        if (cursor is { } point) points.Add(point);
        if (!HasSurface(points, draft.SurfaceBinding)) return;
        AddStroke(points, false, new(.95, .72, .12, .95), 2.0, 0, draft.SurfaceBinding);
        for (var i = 0; i < draft.Vertices.Length; i++)
            AddMarker(draft.Vertices[i], i == 0 ? 6.5 : 5.5, binding: draft.SurfaceBinding);
        if (close && draft.Vertices.Length > 0)
            AddMarker(draft.Vertices[0], 8.5, binding: draft.SurfaceBinding);
    }

    public RenderVectorOverlayResource Build()
    {
        var bounds = Bounds();
        return new(new("map-vector-overlay"), Revision(), _vertices, _indices, _primitives,
            bounds, _labels, _labelBitmaps);
    }

    void AddFill(IReadOnlyList<MapPoint> points, RenderStaticModelColor color, SurfaceBinding? binding = null)
    {
        if (points.Count < 3) return;
        var triangles = MapVectorOverlayTriangulation.Triangulate(points);
        var start = _indices.Count;
        foreach (var point in points) _vertices.Add(Vertex(point, binding));
        foreach (var index in triangles) _indices.Add((uint)(_vertices.Count - points.Count + index));
        AddPrimitive(start, color, RenderVectorOverlayPrimitiveKind.Fill, 0, 0);
    }

    void AddStroke(IReadOnlyList<MapPoint> points, bool close, RenderStaticModelColor color,
        double width, int _, SurfaceBinding? binding = null)
    {
        if (points.Count < 2) return;
        var count = close ? points.Count : points.Count - 1;
        var start = _indices.Count;
        for (var n = 0; n < count; n++)
            AddSegment(points[n], points[(n + 1) % points.Count], binding);
        if (_indices.Count == start) return;
        AddPrimitive(start, color, RenderVectorOverlayPrimitiveKind.Stroke, width, 0);
    }

    void AddSegment(MapPoint a, MapPoint b, SurfaceBinding? binding = null)
    {
        if (a == b) return;
        var q = (uint)_vertices.Count;
        _vertices.Add(LineVertex(a, b, -1, -1, binding)); _vertices.Add(LineVertex(a, b, 1, -1, binding));
        _vertices.Add(LineVertex(a, b, -1, 2, binding)); _vertices.Add(LineVertex(a, b, -1, 2, binding));
        _vertices.Add(LineVertex(a, b, 1, -1, binding)); _vertices.Add(LineVertex(a, b, 1, 2, binding));
        _indices.AddRange([q, q + 1, q + 2, q + 2, q + 4, q + 5]);
    }

    void AddMarker(MapPoint point, double radius, RenderStaticModelColor? color = null,
        SurfaceBinding? binding = null)
    {
        var q = (uint)_vertices.Count; var center = Vertex(point, binding).Position;
        foreach (var offset in new[] { (-1d, -1d), (1d, -1d), (1d, 1d), (-1d, -1d), (1d, 1d), (-1d, 1d) })
            _vertices.Add(new(center, center, offset.Item1, offset.Item2));
        _indices.AddRange([q, q + 1, q + 2, q + 3, q + 4, q + 5]);
        AddPrimitive(_indices.Count - 6, color ?? new(.98, .30, .08, 1),
            RenderVectorOverlayPrimitiveKind.Marker, 0, radius);
    }

    RenderVectorOverlayVertex Vertex(MapPoint p, SurfaceBinding? binding = null) =>
        new(MapCoordinateContract.MapToWorld(p, SurfaceHeight(p, binding)), Vector3d.Zero, 0, 0);
    RenderVectorOverlayVertex LineVertex(MapPoint p, MapPoint other, double side, double along,
        SurfaceBinding? binding = null) =>
        new(MapCoordinateContract.MapToWorld(p, SurfaceHeight(p, binding)),
            MapCoordinateContract.MapToWorld(other, SurfaceHeight(other, binding)), side, along);
}
