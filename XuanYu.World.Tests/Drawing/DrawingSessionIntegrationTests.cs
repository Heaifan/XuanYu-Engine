using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingSessionIntegrationTests
{
    [Fact]
    public void Begin_point_creates_one_active_session()
    {
        var coordinator = new DrawingCoordinator();
        var session = coordinator.Begin(DrawingPrimitiveKind.Point, "标记放置", "MapMarker");

        Assert.Same(session, coordinator.ActiveSession);
        Assert.Equal(DrawingPrimitiveKind.Point, session.PrimitiveKind);
        Assert.Equal(DrawingState.Armed, session.State);
    }

    [Fact]
    public void Preview_and_snap_do_not_change_point_draft()
    {
        var coordinator = NewCoordinator();
        var snap = new DrawingSnapCandidate(new(2, 3), DrawingSnapKind.Surface,
            "surface", 0, 1, true);

        coordinator.UpdatePreview(new(2, 3), snap);

        Assert.Equal(0, coordinator.ActiveSession!.Draft.PointCount);
        Assert.NotNull(coordinator.ActiveSession.Preview);
        Assert.Equal(snap, coordinator.ActiveSession.SnapCandidate);
    }

    [Fact]
    public void Valid_point_auto_completes_and_produces_one_commit_request()
    {
        var coordinator = NewCoordinator();
        Assert.Equal(DrawingInputDisposition.Accepted,
            coordinator.AcceptInput(new(4, 5)).Disposition);

        var request = coordinator.Complete();

        Assert.NotNull(request);
        Assert.Equal([new MapPoint(4, 5)], request!.Points);
        Assert.Null(coordinator.Complete());
    }

    [Fact]
    public void Invalid_point_is_rejected_without_world_commit_request()
    {
        var coordinator = NewCoordinator();
        var result = coordinator.AcceptInput(new(double.NaN, 1));

        Assert.Equal(DrawingInputDisposition.Rejected, result.Disposition);
        Assert.Equal(0, coordinator.ActiveSession!.Draft.PointCount);
        Assert.Null(coordinator.Complete());
    }

    [Fact]
    public void Cancel_terminates_session_and_clears_transient_state()
    {
        var coordinator = NewCoordinator();
        coordinator.UpdatePreview(new(1, 2), null);

        Assert.True(coordinator.Cancel());
        Assert.Null(coordinator.ActiveSession);
    }

    static DrawingCoordinator NewCoordinator()
    {
        var coordinator = new DrawingCoordinator();
        coordinator.Begin(DrawingPrimitiveKind.Point, "标记放置", "MapMarker");
        return coordinator;
    }
}
