using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSnapStateTests
{
    [Fact]
    public void State_tracks_vertex_and_edge_targets_separately()
    {
        var state = new RegionSnapState(); var id = MapRegionId.New();
        state.Acquire(new(new(1, 2), RegionSnapKind.Vertex, id, 3, 0));
        Assert.Equal(RegionSnapKind.Vertex, state.Kind); Assert.Equal(3, state.TargetVertexIndex);
        state.Acquire(new(new(4, 5), RegionSnapKind.Edge, id, 7, 0));
        Assert.Equal(RegionSnapKind.Edge, state.Kind); Assert.Equal(7, state.TargetSegmentIndex);
        Assert.Equal(-1, state.TargetVertexIndex);
    }

    [Fact]
    public void Clear_removes_all_snap_target_state()
    {
        var state = new RegionSnapState();
        state.Acquire(new(new(1, 2), RegionSnapKind.Edge, MapRegionId.New(), 2, 0));
        state.Clear();
        Assert.Equal(RegionSnapKind.None, state.Kind); Assert.False(state.IsSnapped);
        Assert.Null(state.TargetRegionId); Assert.Equal(-1, state.TargetSegmentIndex);
    }
}
