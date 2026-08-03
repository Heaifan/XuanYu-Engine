using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R2-D1-F1：绘制草稿合同（未闭合草稿 → 提交为天然闭合正式区域）。
public sealed class MapRegionDraftTests
{
    static ImmutableArray<MapPoint> ThreePoints() => ImmutableArray.Create(
        new MapPoint(-100, -100), new MapPoint(100, -100), new MapPoint(0, 100));

    [Fact]
    public void Draft_with_two_points_cannot_close()
    {
        var draft = new MapRegionDraft(MapLayerId.New(), "草稿",
            MapRegionKind.Generic,
            ImmutableArray.Create(new MapPoint(0, 0), new MapPoint(10, 10)));
        Assert.False(draft.CanClose);
    }

    [Fact]
    public void Draft_with_three_points_can_close()
    {
        var draft = new MapRegionDraft(MapLayerId.New(), "草稿",
            MapRegionKind.Generic, ThreePoints());
        Assert.True(draft.CanClose);
    }

    [Fact]
    public void Close_produces_valid_region()
    {
        var layers = MapDefaultDefinition.CreateDefault().Layers;
        var draft = new MapRegionDraft(layers[1].LayerId, "部署区A",
            MapRegionKind.Deployment, ThreePoints());
        var region = draft.Close(MapRegionId.New());
        var result = MapRegionValidator.Validate(
            ImmutableArray.Create(region), layers, new MapSize(10000, 10000));
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Close_keeps_metadata()
    {
        var layers = MapDefaultDefinition.CreateDefault().Layers;
        var draft = new MapRegionDraft(layers[1].LayerId, "部署区A",
            MapRegionKind.Deployment, ThreePoints());
        var regionId = MapRegionId.New();
        var region = draft.Close(regionId);
        Assert.Equal(regionId, region.RegionId);
        Assert.Equal(layers[1].LayerId, region.LayerId);
        Assert.Equal("部署区A", region.DisplayName);
        Assert.Equal(MapRegionKind.Deployment, region.Kind);
        Assert.Equal(3, region.Vertices.Length);
    }
}
