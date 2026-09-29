using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    void AddPrimitive(int first, RenderStaticModelColor color, RenderVectorOverlayPrimitiveKind kind,
        double width, double radius) => _primitives.Add(new(
        first, _indices.Count - first, 0, kind, color, width, radius));

    bool UsesTerrain(SurfaceBinding? binding) =>
        binding?.Kind == SurfaceBindingKind.Terrain || binding is null && surface is not null;

    double SurfaceHeight(MapPoint point, SurfaceBinding? binding = null) =>
        UsesTerrain(binding) && surface is { } query && query(point, out var z) ? z : height;

    bool HasSurface(IReadOnlyList<MapPoint> points, SurfaceBinding? binding = null) =>
        !UsesTerrain(binding) || surface is not null && points.All(point => surface(point, out _));
}
