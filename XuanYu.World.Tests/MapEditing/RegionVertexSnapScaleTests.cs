using System.Collections.Immutable;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionVertexSnapScaleTests
{
    [Fact]
    public void Ten_thousand_query_candidates_are_the_only_resolver_workset()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var source = MapRegionId.New(); var target = MapRegionId.New(); var lookupCount = 0;
        var candidates = Enumerable.Range(0, 10_000).Select(_ => MapRegionId.New()).ToImmutableArray();
        candidates = candidates.SetItem(9999, target);
        var region = new MapRegion(target, map.Layers[2].LayerId, "目标", MapRegionKind.Generic,
            [new(0, 0), new(20, 0), new(0, 20)]);
        map = map with { Regions = [new MapRegion(source, map.Layers[2].LayerId, "源", MapRegionKind.Generic,
            [new(-100, 0), new(-80, 0), new(-100, 20)]), region] };
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(new CameraState(new Vector3d(0, 0, 1000), new(0, 0, -1),
            Vector3d.UnitY, 60, 0.1, 5000, 1, ProjectionMode.Orthographic, 1200), viewport);
        var screen = projection.ProjectWorldPoint(new(0, 0, 0));
        var result = RegionVertexSnapResolver.Resolve(source, new(10, 10), screen.X, screen.Y, map, projection,
            new RegionVertexSnapState(), _ => candidates, id => { lookupCount++; return id == target ? region : null; },
            RegionVertexSnapSettings.Default);
        Assert.True(result.IsSnapped); Assert.Equal(target, result.TargetRegionId); Assert.Equal(10_000, lookupCount);
    }
}
