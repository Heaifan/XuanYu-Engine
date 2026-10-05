using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionDrawingControllerTests
{
    [Fact]
    public void Polygon_session_keeps_preview_and_close_candidate_out_of_point_count()
    {
        var controller = NewController();
        controller.AcceptPoint(new(1, 1));
        controller.AcceptPoint(new(3, 1));
        controller.UpdatePreview(new(1, 1), null, true);

        Assert.Equal(2, controller.PointCount);
        Assert.False(controller.CanComplete);
        Assert.False(controller.IsCloseCandidate);

        controller.AcceptPoint(new(3, 3));
        controller.UpdatePreview(new(1, 1), null, true);

        Assert.Equal(3, controller.PointCount);
        Assert.True(controller.CanComplete);
        Assert.True(controller.IsCloseCandidate);
    }

    [Fact]
    public void Complete_emits_one_region_request_and_clears_only_after_success()
    {
        var controller = NewController();
        controller.AcceptPoint(new(1, 1));
        controller.AcceptPoint(new(3, 1));
        controller.AcceptPoint(new(3, 3));

        var request = controller.Complete();

        Assert.NotNull(request);
        Assert.Equal(DrawingPrimitiveKind.Polygon, request!.PrimitiveKind);
        Assert.Equal(3, request.Points.Count);
        Assert.Null(controller.Complete());
        Assert.Equal(3, controller.PointCount);
        controller.ClearAfterCommit();
        Assert.False(controller.IsActive);
        Assert.Equal(0, controller.PointCount);
    }

    [Fact]
    public void Undo_redo_and_invalid_points_preserve_the_session()
    {
        var controller = NewController();
        controller.AcceptPoint(new(1, 1));
        Assert.Equal(DrawingInputDisposition.Rejected,
            controller.AcceptPoint(new(1, 1)).Disposition);
        Assert.Equal(DrawingInputDisposition.Rejected,
            controller.AcceptPoint(new(double.NaN, 2)).Disposition);
        controller.AcceptPoint(new(2, 1));

        Assert.True(controller.Undo());
        Assert.Equal(1, controller.PointCount);
        Assert.True(controller.Redo());
        Assert.Equal(2, controller.PointCount);
        Assert.True(controller.IsActive);
    }

    static RegionDrawingController NewController()
    {
        var controller = new RegionDrawingController();
        controller.Begin(MapLayerId.New(), "区域", MapRegionKind.Generic, SurfaceBinding.ReferencePlane);
        return controller;
    }
}
