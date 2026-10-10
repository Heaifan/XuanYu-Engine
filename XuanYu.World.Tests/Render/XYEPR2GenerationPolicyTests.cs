using XuanYu.Core.Map;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

/// <summary>
/// XYE-PRODUCT-RECOVERY-R2 / WP5-D expected behavior.
/// These are policy-level contract tests only; they do not establish Vulkan resource behavior.
/// </summary>
public sealed class XYEPR2GenerationPolicyTests
{
    static MapRenderSnapshot Map(string id, long sequence, long generation = 0) => new(
        id, 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, sequence,
        SessionGeneration: generation);

    [Fact]
    public void Clear_from_a_new_lifecycle_must_not_be_rejected_by_the_previous_sessions_sequence()
    {
        var previousSession = new MapEditSession();
        var newSession = new MapEditSession();
        var previous = Map("old-session-map", 42, previousSession.SessionGeneration);
        var previousKey = MapSurfaceResourceKey.From(previous);

        // Empty is the clear snapshot produced at a lifecycle boundary. Its local sequence
        // must not be compared against the previous session's sequence 42. The generation
        // comes from a distinct MapEditSession, not a fabricated sequence sentinel.
        var clearSnapshot = MapRenderSnapshot.Empty with
        {
            SessionGeneration = newSession.SessionGeneration,
            SourceChangeSequence = newSession.ChangeSequence
        };
        var clear = MapSurfaceResourceUpdatePolicy.Decide(
            clearSnapshot,
            lastConsumedGeneration: previousSession.SessionGeneration,
            lastConsumedSequence: 42,
            currentKey: previousKey);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, clear.Kind);
        Assert.False(clear.Key.IsVisible);
    }

    [Fact]
    public void A_new_session_may_rebuild_from_sequence_one_after_clear_was_consumed()
    {
        var session = new MapEditSession();
        var newSessionMap = Map("new-session-map", 1, session.SessionGeneration);

        // After the clear is consumed, the new lifecycle starts its own sequence domain.
        var update = MapSurfaceResourceUpdatePolicy.Decide(
            newSessionMap,
            lastConsumedGeneration: session.SessionGeneration,
            lastConsumedSequence: 0,
            currentKey: null);

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.Equal("new-session-map", update.Key.MapId);
    }

    [Fact]
    public void A_delayed_older_update_within_the_same_lifecycle_must_stay_stale()
    {
        var session = new MapEditSession();
        var accepted = Map("current-map", 42, session.SessionGeneration);
        var delayed = Map("old-map", 41, session.SessionGeneration);

        var update = MapSurfaceResourceUpdatePolicy.Decide(
            delayed,
            lastConsumedGeneration: session.SessionGeneration,
            lastConsumedSequence: accepted.SourceChangeSequence,
            currentKey: MapSurfaceResourceKey.From(accepted));

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, update.Kind);
    }
}
