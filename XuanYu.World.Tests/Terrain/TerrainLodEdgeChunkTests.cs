using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainLodEdgeChunkTests
{
    [Fact]
    public void Small_edge_chunk_keeps_projected_sample_spacing_within_two_pixels()
    {
        var state = ViewState();
        var bounds = new SpatialAabb(new(0, 0, 0), new(32, 32, 1));

        var selection = TerrainLodSelector.Select(state, bounds, 32, 32);

        var actualCellPixels = selection.ProjectedPixels / 32;
        Assert.True(actualCellPixels * (1 << selection.Lod) <= 2.0,
            $"LOD {selection.Lod} produces {actualCellPixels * (1 << selection.Lod):0.###} px samples.");
    }

    static ViewProjectionState ViewState() => ViewProjectionState.Create(
        new CameraState(new(16, 16, 53), new(0, 0, -1), Vector3d.UnitY,
            90, 1, 100, 0),
        new ViewportState(0, 0, 100, 100, 100, 100, 1, 0));
}
