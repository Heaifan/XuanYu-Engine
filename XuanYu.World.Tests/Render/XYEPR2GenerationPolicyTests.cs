using XuanYu.Core.Map;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

/// <summary>
/// XYE-PRODUCT-RECOVERY-R2 / WP5-D expected behavior.
/// These are policy-level contract tests only; they do not establish Vulkan resource behavior.
/// </summary>
public sealed class XYEPR2GenerationPolicyTests
{
    static MapRenderSnapshot Map(string id, long sequence) => new(
        id, 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, sequence);

    [Fact]
    public void Clear_from_a_new_lifecycle_must_not_be_rejected_by_the_previous_sessions_sequence()
    {
        var previous = Map("old-session-map", 42);
        var previousKey = MapSurfaceResourceKey.From(previous);

        // Empty is the clear snapshot produced at a lifecycle boundary. Its local sequence
        // must not be compared against the previous session's sequence 42.
        var clear = MapSurfaceResourceUpdatePolicy.Decide(
            MapRenderSnapshot.Empty, lastConsumedSequence: 42, previousKey);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, clear.Kind);
        Assert.False(clear.Key.IsVisible);
    }

    [Fact]
    public void A_new_session_may_rebuild_from_sequence_one_after_clear_was_consumed()
    {
        var newSessionMap = Map("new-session-map", 1);

        // After the clear is consumed, the new lifecycle starts its own sequence domain.
        var update = MapSurfaceResourceUpdatePolicy.Decide(
            newSessionMap, lastConsumedSequence: 0, currentKey: null);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.Equal("new-session-map", update.Key.MapId);
    }

    [Fact]
    public void A_delayed_older_update_within_the_same_lifecycle_must_stay_stale()
    {
        var accepted = Map("current-map", 42);
        var delayed = Map("old-map", 41);

        var update = MapSurfaceResourceUpdatePolicy.Decide(
            delayed, lastConsumedSequence: accepted.SourceChangeSequence,
            MapSurfaceResourceKey.From(accepted));

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, update.Kind);
    }
}
