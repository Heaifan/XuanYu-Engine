using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionDrawingAdapterTests
{
    [Fact]
    public void Adapter_maps_region_metadata_without_adding_a_closure_vertex()
    {
        var layer = MapLayerId.New();
        var binding = SurfaceBinding.Terrain("terrain-1");
        var request = new DrawingCommitRequest(DrawingPrimitiveKind.Polygon,
            [new(1, 1), new(3, 1), new(3, 3)]);

        var draft = RegionDrawingAdapter.ToDraft(request, layer, "区域 1",
            MapRegionKind.Generic, binding);

        Assert.Equal(layer, draft.LayerId);
        Assert.Equal("区域 1", draft.DisplayName);
        Assert.Equal(MapRegionKind.Generic, draft.Kind);
        Assert.Equal(binding, draft.SurfaceBinding);
        Assert.Equal(3, draft.Vertices.Length);
    }
}
