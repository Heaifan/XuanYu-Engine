using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RoadDrawingControllerTests
{
    [Fact]
    public void Polyline_session_requires_manual_completion_and_commits_once()
    {
        var map = new MapEditSession(isWriteThread: () => true);
        var controller = NewController(map);

        Assert.True(controller.Begin(new(map.ActiveRegionLayerId, "道路", "arterial", "roads")));
        Assert.Equal(DrawingPrimitiveKind.Polyline, controller.Session!.PrimitiveKind);
        Assert.Equal(DrawingInputDisposition.Accepted, controller.AcceptPoint(new(1, 1)).Disposition);
        Assert.False(controller.CanComplete);
        Assert.Empty(map.CurrentMap.Roads);
        Assert.Equal(DrawingInputDisposition.Accepted, controller.AcceptPoint(new(2, 2)).Disposition);
        Assert.True(controller.CanComplete);
        Assert.Empty(map.CurrentMap.Roads);

        var result = controller.Complete();

        Assert.True(result.IsSuccess);
        Assert.Single(map.CurrentMap.Roads);
        Assert.False(controller.IsActive);
        Assert.False(controller.Complete().IsSuccess);
        Assert.Single(map.CurrentMap.Roads);
    }

    [Fact]
    public void Preview_does_not_change_count_and_undo_redo_stay_in_session()
    {
        var map = new MapEditSession(isWriteThread: () => true);
        var controller = NewController(map);
        controller.Begin(new(map.ActiveRegionLayerId, "道路", "generic", "roads"));
        controller.AcceptPoint(new(1, 1));
        controller.UpdatePreview(new(3, 3), null);

        Assert.Equal(1, controller.PointCount);
        Assert.NotNull(controller.Preview);
        Assert.True(controller.Undo());
        Assert.Equal(0, controller.PointCount);
        Assert.True(controller.Redo());
        Assert.Equal(1, controller.PointCount);
    }

    [Fact]
    public void Invalid_or_duplicate_points_are_rejected_without_commit()
    {
        var map = new MapEditSession(isWriteThread: () => true);
        var controller = NewController(map);
        controller.Begin(new(map.ActiveRegionLayerId, "道路", "generic", "roads"));

        Assert.Equal(DrawingInputDisposition.Rejected,
            controller.AcceptPoint(new(double.NaN, 1)).Disposition);
        Assert.Equal(DrawingInputDisposition.Accepted, controller.AcceptPoint(new(1, 1)).Disposition);
        Assert.Equal(DrawingInputDisposition.Rejected,
            controller.AcceptPoint(new(1, 1)).Disposition);
        Assert.Equal(1, controller.PointCount);
        Assert.Empty(map.CurrentMap.Roads);
    }

    [Fact]
    public void Cancel_clears_session_without_touching_persistent_roads()
    {
        var map = new MapEditSession(isWriteThread: () => true);
        var controller = NewController(map);
        controller.Begin(new(map.ActiveRegionLayerId, "道路", "generic", "roads"));
        controller.AcceptPoint(new(1, 1));

        Assert.True(controller.Cancel());
        Assert.False(controller.IsActive);
        Assert.Equal(0, controller.PointCount);
        Assert.Empty(map.CurrentMap.Roads);
    }

    static RoadDrawingController NewController(MapEditSession map) =>
        new(map);
}
