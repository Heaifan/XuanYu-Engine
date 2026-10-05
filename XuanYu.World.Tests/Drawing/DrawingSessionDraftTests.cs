using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingSessionDraftTests
{
    [Fact]
    public void Polyline_requires_two_points_and_manual_completion()
    {
        var session = New(DrawingPrimitiveKind.Polyline);
        session.AcceptInput(new(1, 2));
        Assert.False(session.CanComplete);
        session.AcceptInput(new(3, 4));
        Assert.True(session.CanComplete);
        Assert.Equal(2, session.PointCount);
        Assert.Equal(DrawingState.Drawing, session.State);
    }

    [Fact]
    public void Polyline_complete_is_manual_and_emits_one_request()
    {
        var session = New(DrawingPrimitiveKind.Polyline);
        session.AcceptInput(new(1, 2)); session.AcceptInput(new(3, 4));
        var request = session.Complete();
        Assert.Equal([new MapPoint(1, 2), new MapPoint(3, 4)], request!.Points);
        Assert.Null(session.Complete());
    }

    [Fact]
    public void Polyline_undo_redo_changes_only_authoring_points()
    {
        var session = New(DrawingPrimitiveKind.Polyline);
        session.AcceptInput(new(1, 2)); session.AcceptInput(new(3, 4));
        session.UpdatePreview(new(5, 6), null);
        Assert.True(session.Undo());
        Assert.Equal(1, session.PointCount);
        Assert.True(session.Redo());
        Assert.Equal(2, session.PointCount);
        Assert.Equal(new MapPoint(5, 6), session.Preview!.CursorPosition);
    }

    [Fact]
    public void Polygon_requires_three_points_and_supports_close_preview()
    {
        var session = New(DrawingPrimitiveKind.Polygon);
        session.AcceptInput(new(1, 2)); session.AcceptInput(new(3, 4));
        Assert.False(session.CanComplete);
        session.AcceptInput(new(5, 6));
        session.UpdatePreview(new(1, 2), null);
        Assert.True(session.CanComplete);
        Assert.True(session.Preview!.CloseCandidate.IsCandidate);
    }

    [Fact]
    public void Invalid_or_duplicate_input_is_rejected_without_destroying_session()
    {
        var session = New(DrawingPrimitiveKind.Polygon);
        session.AcceptInput(new(1, 2));
        Assert.Equal(DrawingInputDisposition.Rejected,
            session.AcceptInput(new(1, 2)).Disposition);
        Assert.Equal(DrawingInputDisposition.Rejected,
            session.AcceptInput(new(double.NaN, 4)).Disposition);
        Assert.Equal(1, session.PointCount);
        Assert.True(session.IsActive);
    }

    [Fact]
    public void Terminate_clears_draft_preview_snap_and_history()
    {
        var session = New(DrawingPrimitiveKind.Polyline);
        session.AcceptInput(new(1, 2)); session.AcceptInput(new(3, 4));
        session.UpdatePreview(new(5, 6), new(new(5, 6), DrawingSnapKind.Vertex,
            "v", 0, 1, true));
        session.Undo(); session.Terminate();
        Assert.Equal(0, session.PointCount);
        Assert.Null(session.Preview); Assert.Null(session.SnapCandidate);
        Assert.False(session.CanUndo); Assert.False(session.CanRedo);
    }

    static DrawingSession New(DrawingPrimitiveKind kind) => new(kind, "test", "test");
}
