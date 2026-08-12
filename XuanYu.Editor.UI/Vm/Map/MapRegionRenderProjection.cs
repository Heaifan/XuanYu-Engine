using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public static class MapRegionRenderProjection
{
    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing)
        => Build(map, drawing, new RoadDrawingState());

    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing, RoadDrawingState roads)
    {
        var builder = new MapVectorOverlayBuilder(map.Surface.BaseHeightMeters);
        var layers = map.Layers.ToDictionary(layer => layer.LayerId);
        foreach (var region in map.Regions.Where(region =>
                     region.IsVisible && layers.TryGetValue(region.LayerId, out var layer) && layer.IsVisible)
                     .OrderBy(region => layers[region.LayerId].Order))
            builder.AddRegion(region);
        foreach (var road in map.Roads.Where(road => road.IsVisible && layers.TryGetValue(road.LayerId, out var layer) && layer.IsVisible).OrderBy(road => layers[road.LayerId].Order))
            builder.AddRoad(road);
        if (drawing.Draft is { } draft)
            builder.AddDraft(draft, drawing.Cursor, drawing.IsCloseCandidate);
        if (roads.Draft is { } roadDraft) builder.AddRoadDraft(roadDraft, roads.Cursor);
        return builder.Build();
    }
}
