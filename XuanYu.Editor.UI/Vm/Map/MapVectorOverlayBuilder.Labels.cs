using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    void AddLabel(MapRegion region, IReadOnlyList<MapPoint> points)
    {
        if (!MapRegionLabelProjection.TryCreate(region, points, height, dpiScale, out var label)) return;
        _labels.Add(label);
        if (labelCache is not null) _labelBitmaps.Add(labelCache.GetOrCreate(label));
    }
}
