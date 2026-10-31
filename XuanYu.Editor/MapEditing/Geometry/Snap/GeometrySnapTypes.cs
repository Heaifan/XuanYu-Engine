using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public enum GeometrySnapKind { None, Vertex, Segment }

public readonly record struct GeometryVertexCandidate(
    GeometryFeatureKey Feature, int Index, MapPoint WorldPoint, ScreenPoint ScreenPoint);

public readonly record struct GeometrySegmentCandidate(
    GeometryFeatureKey Feature, int Index, MapPoint Start, MapPoint End,
    ScreenPoint StartScreen, ScreenPoint EndScreen, bool Closed);

public readonly record struct GeometrySnapResult(
    MapPoint ResolvedPoint, GeometrySnapKind Kind, GeometryFeatureKey? Target,
    int TargetIndex, double DistanceSquared)
{
    public bool IsSnapped => Kind != GeometrySnapKind.None;
    public static GeometrySnapResult Raw(MapPoint point) =>
        new(point, GeometrySnapKind.None, null, -1, double.PositiveInfinity);
}

public sealed class GeometrySnapState
{
    public GeometrySnapKind Kind { get; private set; }
    public GeometryFeatureKey? Target { get; private set; }
    public int TargetIndex { get; private set; } = -1;
    public bool IsSnapped => Kind != GeometrySnapKind.None;

    public void Acquire(GeometrySnapResult result)
    { Kind = result.Kind; Target = result.Target; TargetIndex = result.TargetIndex; }

    public void Clear() { Kind = GeometrySnapKind.None; Target = null; TargetIndex = -1; }
}
