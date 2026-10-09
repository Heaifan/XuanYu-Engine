using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

// Policy-only reproduction: sequence numbers have no session/epoch component.
public sealed class CAuditR1GenerationPolicyTests
{
    static MapRenderSnapshot Map(string id, long sequence) => new(
        id, 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, sequence);

    [Fact]
    public void Empty_and_new_session_low_sequence_are_rejected_after_high_sequence()
    {
        var high = Map("old", 42);
        var key = MapSurfaceResourceKey.From(high);

        var clear = MapSurfaceResourceUpdatePolicy.Decide(
            MapRenderSnapshot.Empty, 42, key);
        var newSession = MapSurfaceResourceUpdatePolicy.Decide(
            Map("new", 1), 42, key);
        var delayedOld = MapSurfaceResourceUpdatePolicy.Decide(
            Map("old", 41), 42, key);

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, clear.Kind);
        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, newSession.Kind);
        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, delayedOld.Kind);
        Assert.False(MapRenderSnapshot.Empty.HasMap);
    }

    [Fact]
    public void Newer_empty_snapshot_requests_resource_recreate_and_clear()
    {
        var high = Map("old", 42);
        var newerEmpty = MapRenderSnapshot.Empty with { SourceChangeSequence = 43 };

        var update = MapSurfaceResourceUpdatePolicy.Decide(
            newerEmpty, 42, MapSurfaceResourceKey.From(high));

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.False(update.Key.IsVisible);
    }
}
