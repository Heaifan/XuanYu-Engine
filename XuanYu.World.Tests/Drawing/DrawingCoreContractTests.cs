using System.Reflection;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingCoreContractTests
{
    [Fact]
    public void Rejected_is_an_input_disposition_not_a_session_state()
    {
        Assert.DoesNotContain("Rejected", Enum.GetNames<DrawingState>());
        var result = DrawingInputResult.Rejected("OutsideSurface", "输入未命中可用表面。");
        Assert.Equal(DrawingInputDisposition.Rejected, result.Disposition);
        Assert.True(result.SessionCanContinue);
    }

    [Fact]
    public void Blocked_is_distinct_from_rejected_and_carries_recovery()
    {
        var result = DrawingInputResult.Blocked("LayerLocked", "图层暂时锁定。", RecoveryPolicy.Retryable);
        Assert.Equal(DrawingInputDisposition.Blocked, result.Disposition);
        Assert.False(result.SessionCanContinue);
        Assert.Equal(RecoveryPolicy.Retryable, result.Recovery);
    }

    [Fact]
    public void Primitives_are_geometry_only()
    {
        var forbidden = new[] { "Region", "Road", "Marker", "Province", "Territory", "Kind" };
        foreach (var type in new[] { typeof(MapPoint), typeof(DrawingPolyline), typeof(DrawingPolygon) })
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            Assert.DoesNotContain(forbidden, token => property.Name.Contains(token, StringComparison.Ordinal));
    }

    [Fact]
    public void Completion_and_post_commit_policies_are_independent()
    {
        Assert.NotEqual(CompletionPolicy.ManualComplete, CompletionPolicy.AutoCommit);
        Assert.NotEqual(PostCommitPolicy.Exit, PostCommitPolicy.RestartArmed);
    }

    [Fact]
    public void Failure_and_recovery_policies_are_classified()
    {
        Assert.Equal(3, Enum.GetValues<DrawingCommitFailurePolicy>().Length);
        Assert.Equal(3, Enum.GetValues<RecoveryPolicy>().Length);
    }

    [Fact]
    public void Snapshot_counts_authoring_points_and_has_no_ui_dependency()
    {
        var snapshot = new DrawingSessionSnapshot(
            DrawingState.Drawing, DrawingPrimitiveKind.Polygon, 5, false, false,
            true, true, true, SnapState.Inactive, null, null);
        Assert.Equal(5, snapshot.PointCount);
        Assert.DoesNotContain(typeof(DrawingSessionSnapshot).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name?.StartsWith("Avalonia", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void Session_and_pointer_gesture_owner_are_separate_contracts()
    {
        Assert.Null(typeof(DrawingSessionSnapshot).GetProperty("PointerGestureOwner"));
        Assert.NotNull(typeof(DrawingPointerGesture).GetProperty("Owner"));
    }
}
