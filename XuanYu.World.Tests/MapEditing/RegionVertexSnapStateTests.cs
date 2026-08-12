using XuanYu.Editor.MapEditing;
using XuanYu.Core.Space;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionVertexSnapStateTests
{
    [Fact]
    public void Acquire_exposes_target()
    {
        var state = new RegionVertexSnapState(); var id = MapRegionId.New();
        state.Acquire(id, 2, new(1, 2));
        Assert.True(state.IsSnapped); Assert.Equal(id, state.TargetRegionId);
        Assert.Equal(2, state.TargetVertexIndex); Assert.Equal(new MapPoint(1, 2), state.TargetPoint);
    }

    [Fact]
    public void Clear_removes_target()
    {
        var state = new RegionVertexSnapState(); state.Acquire(MapRegionId.New(), 0, new(1, 2));
        state.Clear();
        Assert.False(state.IsSnapped); Assert.Null(state.TargetRegionId); Assert.Equal(-1, state.TargetVertexIndex);
    }

    [Fact]
    public void Settings_require_release_radius_not_smaller_than_enter()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RegionVertexSnapSettings(8, 7).Validate());
    }

    [Fact]
    public void Snapped_target_is_held_inside_release_radius()
    {
        var map = MapDefaultDefinition.CreateDefault(); var target = MapRegionId.New();
        var region = new MapRegion(target, map.Layers[2].LayerId, "区域", MapRegionKind.Generic,
            [new(0, 0), new(20, 0), new(0, 20)]);
        map = map with { Regions = [new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "源", MapRegionKind.Generic,
            [new(-100, 0), new(-80, 0), new(-100, 20)]), region] };
        var projection = ViewProjectionState.Create(new XuanYu.Core.Space.CameraState(new(0, 0, 1000), new(0, 0, -1),
            XuanYu.Core.Math.Vector3d.UnitY, 60, 0.1, 5000, 1, XuanYu.Core.Space.ProjectionMode.Orthographic, 1200),
            new XuanYu.Core.Space.ViewportState(0, 0, 800, 600, 800, 600, 1, 1));
        var screen = projection.ProjectWorldPoint(new(0, 0, 0)); var state = new RegionVertexSnapState();
        state.Acquire(target, 0, new(0, 0));
        var result = RegionVertexSnapResolver.Resolve(map.Regions[0].RegionId, new(1, 1), screen.X + 10, screen.Y,
            map, projection, state, _ => [target], id => map.Regions.FirstOrDefault(r => r.RegionId == id),
            RegionVertexSnapSettings.Default);
        Assert.True(result.IsSnapped); Assert.Equal(target, result.TargetRegionId);
    }
}
