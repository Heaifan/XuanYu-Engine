using XuanYu.Core.Map;
using XuanYu.Editor.MapEditing;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

/// <summary>
/// Wave 2 contract witness for lifecycle-scoped map sequence numbers.
/// Policy-only: this does not prove Vulkan resource disposal or GPU state.
/// </summary>
public sealed class XYEPR2GenerationWave2EpochPolicyTests
{
    private static MapRenderSnapshot Map(string id, long sequence, long generation) => new(
        id, 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, sequence,
        SessionGeneration: generation);

    [Fact]
    public void A_new_epoch_must_accept_a_low_sequence_after_a_higher_sequence_was_consumed()
    {
        var previousSession = new MapEditSession();
        var newSession = new MapEditSession();
        var previousEpoch = Map("previous-epoch-map", 42, previousSession.SessionGeneration);
        var newEpoch = Map("new-epoch-map", 1, newSession.SessionGeneration);

        // MapEditSession sequence restarts for a new lifecycle. Epoch identity must be
        // compared before sequence ordering; sequence 1 is not stale relative to epoch 42.
        var update = MapSurfaceResourceUpdatePolicy.Decide(
            newEpoch,
            lastConsumedGeneration: previousSession.SessionGeneration,
            lastConsumedSequence: previousEpoch.SourceChangeSequence,
            currentKey: MapSurfaceResourceKey.From(previousEpoch));

        Assert.Equal(MapSurfaceResourceUpdateKind.Recreate, update.Kind);
        Assert.Equal("new-epoch-map", update.Key.MapId);
    }

    [Fact]
    public void A_delayed_packet_from_a_previous_generation_must_be_rejected()
    {
        var previousSession = new MapEditSession();
        var currentSession = new MapEditSession();
        var priorPacket = Map("prior-generation-map", 100, previousSession.SessionGeneration);
        var current = Map("current-generation-map", 1, currentSession.SessionGeneration);

        var update = MapSurfaceResourceUpdatePolicy.Decide(
            priorPacket,
            lastConsumedGeneration: current.SessionGeneration,
            lastConsumedSequence: current.SourceChangeSequence,
            currentKey: MapSurfaceResourceKey.From(current));

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, update.Kind);
    }

    [Fact]
    public void A_lower_sequence_from_the_same_generation_must_remain_stale()
    {
        var session = new MapEditSession();
        var current = Map("current-map", 42, session.SessionGeneration);
        var delayed = Map("delayed-map", 41, session.SessionGeneration);

        var update = MapSurfaceResourceUpdatePolicy.Decide(
            delayed,
            lastConsumedGeneration: session.SessionGeneration,
            lastConsumedSequence: current.SourceChangeSequence,
            currentKey: MapSurfaceResourceKey.From(current));

        Assert.Equal(MapSurfaceResourceUpdateKind.RejectStale, update.Kind);
    }
}
