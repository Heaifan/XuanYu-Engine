using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：选择状态（稳定 ID/存在性/不产生 Dirty/规范化）。
public sealed class MapEditSessionSelectionTests
{    static MapDefinition MapWithRegion()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(
            MapRegionId.New(), map.Layers[1].LayerId, "部署区", MapRegionKind.Deployment,
            ImmutableArray.Create(new MapPoint(-100, -100), new MapPoint(100, -100),
                new MapPoint(100, 100), new MapPoint(-100, 100)));
        return map with { Regions = [region] };
    }

    [Fact]
    public void Select_layer_succeeds_and_keeps_id()
    {
        var session = new MapEditSession();
        var layerId = session.CurrentMap.Layers[1].LayerId;
        Assert.True(session.SelectLayer(layerId).IsSuccess);
        Assert.Equal(MapSelectionKind.Layer, session.Selection.Kind);
        Assert.Equal(layerId, session.Selection.LayerId);
    }

    [Fact]
    public void Select_unknown_layer_rejected()
    {
        var session = new MapEditSession();
        var result = session.SelectLayer(MapLayerId.New());
        Assert.True(result.IsFailure);
        Assert.Equal("UnknownLayer", result.Error!.Value.Code);
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind);
    }

    [Fact]
    public void Select_region_succeeds()
    {
        var session = new MapEditSession(MapWithRegion());
        var region = session.CurrentMap.Regions[0];
        Assert.True(session.SelectRegion(region.RegionId).IsSuccess);
        Assert.Equal(MapSelectionKind.Region, session.Selection.Kind);
        Assert.Equal(region.RegionId, session.Selection.RegionId);
        Assert.Equal(region.LayerId, session.Selection.LayerId);
    }

    [Fact]
    public void Selection_does_not_change_map_dirty_or_history()
    {
        var session = new MapEditSession();
        var map = session.CurrentMap;
        var stateId = session.CurrentStateId;
        var sequence = session.ChangeSequence;
        var dirty = session.IsDirty;
        session.SelectLayer(session.CurrentMap.Layers[1].LayerId);
        Assert.Same(map, session.CurrentMap);
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.Equal(sequence, session.ChangeSequence);
        Assert.Equal(dirty, session.IsDirty);
        Assert.False(session.CanUndo);
    }

    [Fact]
    public void Content_edit_keeps_selection()
    {
        var session = new MapEditSession(MapWithRegion());
        session.SelectRegion(session.CurrentMap.Regions[0].RegionId);
        session.RenameMap("测试战场");
        Assert.Equal(MapSelectionKind.Region, session.Selection.Kind);
    }

    [Fact]
    public void Replace_current_map_resets_selection_to_map()
    {
        var withRegion = MapWithRegion();
        var session = new MapEditSession(withRegion);
        session.SelectRegion(session.CurrentMap.Regions[0].RegionId);
        Assert.Equal(MapSelectionKind.Region, session.Selection.Kind);
        var withoutRegion = withRegion with { Regions = [] };
        session.ReplaceCurrentMap(withoutRegion, markSaved: false, path: null);
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind); // D2 合同：Replace 默认选择地图
        // 注：区域删除后的"回退图层"规范化属 D4/D5（删除命令）时验证
    }

    [Fact]
    public void Normalize_to_map_when_layer_removed()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var session = new MapEditSession(map);
        session.SelectLayer(map.Layers[1].LayerId);
        Assert.Equal(MapSelectionKind.Layer, session.Selection.Kind);

        var noRegionLayer = map with { Layers = [map.Layers[0]] };
        session.ReplaceCurrentMap(noRegionLayer, markSaved: false, path: null);
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind);
    }
}
