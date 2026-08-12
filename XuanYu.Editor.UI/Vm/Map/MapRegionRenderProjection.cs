using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public static class MapRegionRenderProjection
{
    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing)
        => Build(map, drawing, new RoadDrawingState());

    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing, RoadDrawingState roads)
        => Build(map, drawing, roads, null);

    public static RenderVectorOverlayResource Build(MapDefinition map, RegionDrawingState drawing,
        RoadDrawingState roads, MapGeometryPreview? geometry)
    {
        var builder = new MapVectorOverlayBuilder(map.Surface.BaseHeightMeters);
        var layers = map.Layers.ToDictionary(layer => layer.LayerId);
        foreach (var region in map.Regions.Where(region =>
                     region.IsVisible && layers.TryGetValue(region.LayerId, out var layer) && layer.IsVisible)
                     .OrderBy(region => layers[region.LayerId].Order))
            builder.AddRegion(region, geometry?.Selection == new MapGeometrySelection(MapGeometryFeatureKind.Region, region.RegionId.ToString()),
                geometry?.Selection.FeatureId == region.RegionId.ToString() && geometry?.Selection.Kind == MapGeometryFeatureKind.Region ? geometry.Value.Points : null);
        foreach (var road in map.Roads.Where(road => road.IsVisible && layers.TryGetValue(road.LayerId, out var layer) && layer.IsVisible).OrderBy(road => layers[road.LayerId].Order))
            builder.AddRoad(road, geometry?.Selection == new MapGeometrySelection(MapGeometryFeatureKind.Road, road.RoadId.ToString()),
                geometry?.Selection.FeatureId == road.RoadId.ToString() && geometry?.Selection.Kind == MapGeometryFeatureKind.Road ? geometry.Value.Points : null);
        if (drawing.Draft is { } draft)
            builder.AddDraft(draft, drawing.Cursor, drawing.IsCloseCandidate);
        if (roads.Draft is { } roadDraft) builder.AddRoadDraft(roadDraft, roads.Cursor);
        return builder.Build();
    }
}
