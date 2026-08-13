using System.Collections.Immutable;
using XuanYu.Core.Gizmo;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct GeometryFeatureAdapter(
    GeometryFeatureKey Key,
    MapLayerId LayerId,
    GeometryKind Kind,
    GeometryCapabilities Capabilities,
    ImmutableArray<MapPoint> Points,
    ImmutableArray<ScreenPoint> Projected = default)
{
    public bool IsClosed => Kind == GeometryKind.Polygon;
    public int SegmentCount => IsClosed ? Points.Length : Math.Max(0, Points.Length - 1);
}
