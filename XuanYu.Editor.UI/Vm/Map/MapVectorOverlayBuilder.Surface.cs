using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    void AddPrimitive(int first, RenderStaticModelColor color, RenderVectorOverlayPrimitiveKind kind,
        double width, double radius) => _primitives.Add(new(
        first, _indices.Count - first, 0, kind, color, width, radius));

    double SurfaceHeight(MapPoint point) => surface is { } query && query(point, out var z) ? z : height;

    bool HasSurface(IReadOnlyList<MapPoint> points) =>
        surface is null || points.All(point => surface(point, out _));
}
