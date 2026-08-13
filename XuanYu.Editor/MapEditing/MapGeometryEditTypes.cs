using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public enum MapGeometryFeatureKind { Region, Road, Marker }

public readonly record struct MapGeometrySelection(
    MapGeometryFeatureKind Kind, string FeatureId);

public readonly record struct MapGeometryPreview(
    MapGeometrySelection Selection, ImmutableArray<MapPoint> Points);

public readonly record struct MapGeometryHit(
    MapGeometrySelection Selection, int VertexIndex, double DistanceDip);
