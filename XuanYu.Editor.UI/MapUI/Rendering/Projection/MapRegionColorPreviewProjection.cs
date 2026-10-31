using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

static class MapRegionColorPreviewProjection
{
    public static MapDefinition Apply(MapDefinition map, RegionFillColorPreview? preview)
    {
        if (preview is not { } active ||
            active.Target.ObjectKind != InspectorObjectKind.Region ||
            active.Target.PropertyKey != "Region.Style.FillColor" ||
            !MapRegionId.TryParse(active.Target.ObjectId, out var regionId))
            return map;

        var region = map.Regions.FirstOrDefault(item => item.RegionId == regionId);
        if (region is null) return map;
        var next = region with { FillColorRgb = active.PreviewRgb & 0x00FFFFFF };
        return map with { Regions = map.Regions.Replace(region, next) };
    }
}
