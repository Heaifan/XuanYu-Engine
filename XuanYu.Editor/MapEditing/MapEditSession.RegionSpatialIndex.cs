using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    readonly RegionSpatialIndex _regionSpatialIndex = new();

    internal int IndexedRegionCount => _regionSpatialIndex.Count;

    public bool TryGetRegion(MapRegionId regionId, out MapRegion region)
    {
        region = _currentMap.Regions.FirstOrDefault(item => item.RegionId == regionId)!;
        return region is not null;
    }

    public ImmutableArray<MapRegionId> QueryLocalRegions(RegionSpatialBounds bounds)
    {
        if (!GuardWriteThread()) throw new InvalidOperationException("区域局部查询必须在编辑写线程执行。");
        return _regionSpatialIndex.Query(bounds);
    }

    void RebuildRegionSpatialIndex() => RebuildRegionSpatialIndex(_currentMap);

    void RebuildRegionSpatialIndex(MapDefinition map) => _regionSpatialIndex.Rebuild(map.Regions);

    void UpsertRegionSpatialIndex(MapDefinition map, MapRegionId regionId)
    {
        var region = map.Regions.First(item => item.RegionId == regionId);
        _regionSpatialIndex.Upsert(region);
    }

    void RemoveRegionSpatialIndex(MapRegionId regionId) => _regionSpatialIndex.Remove(regionId);
}
