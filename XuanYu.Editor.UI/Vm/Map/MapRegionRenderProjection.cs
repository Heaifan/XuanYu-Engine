using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public static class MapRegionRenderProjection
{
    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing)
    {
        var builder = new MapVectorOverlayBuilder(map.Surface.BaseHeightMeters);
        foreach (var region in map.Regions.Where(r => r.IsVisible)) builder.AddRegion(region);
        if (drawing.Draft is { } draft)
            builder.AddDraft(draft, drawing.Cursor, drawing.IsCloseCandidate);
        return builder.Build();
    }
}
