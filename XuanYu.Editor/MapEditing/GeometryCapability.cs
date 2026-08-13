namespace XuanYu.Editor.MapEditing;

public enum GeometryKind { Point, Polyline, Polygon }

[Flags]
public enum GeometryCapabilities
{
    None = 0,
    Selectable = 1,
    VertexEditable = 2,
    Snappable = 4,
    SnapTarget = 8,
}
