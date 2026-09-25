namespace XuanYu.Editor.MapEditing;

public enum MapGeometryContextKind { Empty, Vertex, Edge, Face, Marker }

public readonly record struct MapGeometryContextHit(
    MapGeometryContextKind Kind,
    MapGeometrySelection Selection,
    int VertexIndex,
    int SegmentIndex,
    double DistanceDip)
{
    public static MapGeometryContextHit Empty => new(MapGeometryContextKind.Empty, default, -1, -1, double.MaxValue);
}
