using System.Collections.Immutable;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public static class RegionDrawingAdapter
{
    public static MapRegionDraft ToDraft(DrawingCommitRequest request, MapLayerId layerId,
        string displayName, MapRegionKind kind, SurfaceBinding surfaceBinding)
    {
        if (request.PrimitiveKind != DrawingPrimitiveKind.Polygon)
            throw new ArgumentException("Region authoring requires a polygon request.", nameof(request));
        if (!DrawingValidation.Validate(request.PrimitiveKind, request.Points).IsValid)
            throw new ArgumentException("The polygon request is invalid.", nameof(request));
        return new MapRegionDraft(layerId, displayName, kind, request.Points.ToImmutableArray())
        { SurfaceBinding = surfaceBinding };
    }
}
