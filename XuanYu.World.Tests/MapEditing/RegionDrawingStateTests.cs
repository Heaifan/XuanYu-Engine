using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionDrawingStateTests
{
    [Fact]
    public void Adds_vertices_and_marks_close_candidate_only_after_three()
    {
        var state = new RegionDrawingState();
        state.Start(MapLayerId.New(), "区域", MapRegionKind.Generic);
        state.AddVertex(new MapPoint(1, 1));
        state.AddVertex(new MapPoint(2, 1));
        state.UpdatePointer(new MapPoint(1, 1), closeCandidate: true);
        Assert.False(state.IsCloseCandidate);

        state.AddVertex(new MapPoint(2, 2));
        state.UpdatePointer(new MapPoint(1, 1), closeCandidate: true);
        Assert.True(state.IsCloseCandidate);
        Assert.Equal(3, state.TakeDraftForClose()!.Vertices.Length);
    }

    [Fact]
    public void Cancel_discards_only_temporary_drawing_state()
    {
        var state = new RegionDrawingState();
        state.Start(MapLayerId.New(), "区域", MapRegionKind.Generic);
        state.AddVertex(new MapPoint(1, 1));
        state.Cancel();
        Assert.False(state.IsActive);
        Assert.Null(state.Cursor);
    }
}
