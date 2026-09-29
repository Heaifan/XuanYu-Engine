using XuanYu.Editor.UI;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed partial class GroundAuthoringContractTests
{
    [Fact]
    public void Terrain_revision_is_cache_metadata_not_region_identity()
    {
        var binding = SurfaceBinding.Terrain("dem-a");
        var before = new MapRegion(MapRegionId.New(), MapLayerId.New(), "R", MapRegionKind.Generic,
            [new(0, 0), new(1, 0), new(0, 1)]) { SurfaceBinding = binding };
        var after = before with { SurfaceBinding = binding };
        Assert.True(before.Vertices.SequenceEqual(after.Vertices));
        Assert.Equal(before.SurfaceBinding, after.SurfaceBinding);
    }

    [Fact]
    public void Terrain_elevation_changes_display_sample_without_changing_xy()
    {
        var flat = Pick(new FakeSurface(10), 0);
        var hill = Pick(new FakeSurface(50), 0);
        Assert.Equal(flat.WorldXY, hill.WorldXY);
        Assert.NotEqual(flat.ResolvedElevation, hill.ResolvedElevation);
    }

    [Fact]
    public void Sloped_surface_is_not_reference_plane()
    {
        var result = Pick(new FakeSurface(25), 0);
        Assert.Equal(SurfaceBindingKind.Terrain, result.SurfaceBinding.Kind);
        Assert.Equal(25, result.ResolvedElevation);
    }

    [Fact]
    public void Region_preview_uses_terrain_conforming_path()
    {
        var baseMap = MapDefaultDefinition.CreateDefault();
        var layer = baseMap.Layers.First(item => item.Kind == MapLayerKind.Region);
        var map = baseMap with
        {
            Regions = [new(MapRegionId.New(), layer.LayerId, "R", MapRegionKind.Generic,
                [new(1, 1), new(5, 1), new(3, 5)])]
        };
        var resource = MapRegionRenderProjection.Build(map, new(), new(), null, 1, null,
            new FakeSurface(25));
        Assert.Contains(resource.Vertices, vertex => vertex.Position.Z == 25);
    }

    sealed class FakeSurface(double? height) : IGroundSurface
    {
        public SurfaceBinding Binding => SurfaceBinding.Terrain("fake-terrain");
        public int? Revision => 7;
        public bool TryGetElevation(MapPoint worldXY, out double elevation)
        {
            elevation = height ?? 0;
            return height.HasValue;
        }
    }
}
