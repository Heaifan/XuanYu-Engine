using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

static class MapRegionRenderStyle
{
    public static RenderStaticModelColor FillColor(MapRegion region)
    {
        var rgb = region.FillColorRgb;
        return new(
            ((rgb >> 16) & 0xFF) / 255d,
            ((rgb >> 8) & 0xFF) / 255d,
            (rgb & 0xFF) / 255d,
            .32);
    }
}
