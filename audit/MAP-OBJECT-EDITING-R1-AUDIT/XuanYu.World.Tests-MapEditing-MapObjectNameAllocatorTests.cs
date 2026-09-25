using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class MapObjectNameAllocatorTests
{
    [Fact]
    public void Placeholder_names_become_unique_domain_names()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var layer = map.ActiveLayer();
        map = map with
        {
            Regions = [new(MapRegionId.New(), layer, "区域1", MapRegionKind.Generic, [new(0, 0), new(1, 0), new(0, 1)])],
            Roads = [new(MapRoadId.New(), layer, "道路1", "generic", [new(0, 0), new(1, 1)])],
            Markers = [new(MapMarkerId.New(), layer, "标记1", new(0, 0))]
        };

        Assert.Equal("区域2", MapObjectNameAllocator.Region(map, "区域"));
        Assert.Equal("道路2", MapObjectNameAllocator.Road(map, "道路"));
        Assert.Equal("标记2", MapObjectNameAllocator.Marker(map, "地图标记"));
    }
}
